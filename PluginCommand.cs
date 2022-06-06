namespace TinyCmds;

using System;
using System.Linq;
using System.Reflection;

using Dalamud.Game.Command;

using TinyCmds.Attributes;
using TinyCmds.Chat;
using TinyCmds.Utils;

internal delegate void PluginCommandDelegate(string? command, string rawArguments, FlagMap flags, ref bool showHelp);
internal delegate void PluginCommandInvocationErrorHandlerDelegate(params object[] payloads);

internal class PluginCommand {
	private readonly PluginCommandDelegate handler;
	private readonly PluginCommandDelegate? helper;
	private readonly PluginCommandInvocationErrorHandlerDelegate error;
	public CommandInfo MainCommandInfo => new(this.Dispatch) {
		HelpMessage = this.Summary,
		ShowInHelp = this.ShowInDalamud,
	};
	public CommandInfo AliasCommandInfo => new(this.Dispatch) {
		HelpMessage = this.Summary,
		ShowInHelp = false,
	};
	public string CommandComparable => this.Command.TrimStart('/').ToLower();
	public string[] AliasesComparable => this.Aliases.Select(s => s.TrimStart('/').ToLower()).ToArray();
	public string[] HelpLines => this.Help.Split('\r', '\n').Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();
	public string Command { get; }
	public string Summary { get; }
	public string Help { get; }
	public string Usage { get; }
	public string[] Aliases { get; }
	public bool ShowInDalamud { get; }
	public bool ShowInListing { get; }
	public PluginCommand(MethodInfo method, PluginCommandDelegate? printHelp, PluginCommandInvocationErrorHandlerDelegate onError) {
		CommandAttribute? attrCommand = method.GetCustomAttribute<CommandAttribute>();
		if (attrCommand is null) {
			throw new NullReferenceException("Cannot construct PluginCommand from method without CommandAttribute");
		}
		ArgumentsAttribute? args = method.GetCustomAttribute<ArgumentsAttribute>();
		this.Command = $"/{attrCommand.Command.TrimStart('/')}";
		this.Summary = method.GetCustomAttribute<SummaryAttribute>()?.Summary ?? "";
		this.Help = method.GetCustomAttribute<HelpMessageAttribute>()?.HelpMessage ?? "";
		this.Usage = $"{this.Command} {args?.ArgumentDescription}".Trim();
		this.Aliases = method.GetCustomAttribute<AliasesAttribute>()?.Aliases ?? Array.Empty<string>();
		this.ShowInListing = method.GetCustomAttribute<HideInCommandListingAttribute>() is null;
		this.ShowInDalamud = this.ShowInListing && (method.GetCustomAttribute<DoNotShowInHelpAttribute>() is null || string.IsNullOrEmpty(this.Summary));
		this.handler = (PluginCommandDelegate)Delegate.CreateDelegate(typeof(PluginCommandDelegate), null, method);
		this.helper = printHelp;
		this.error = onError;
	}
	public void Dispatch(string command, string argline) {
		try {
			(FlagMap flags, string rawArgs) = ArgumentParser.ExtractFlags(argline);
			bool showHelp = false;
			if (flags["h"]) {
				if (this.helper is not null)
					this.helper(null, command, flags, ref showHelp);
				return;
			}
			this.handler(command, rawArgs, flags, ref showHelp);
			if (showHelp && this.helper is not null)
				this.helper(null, command, flags, ref showHelp);
		}
		catch (Exception e) {
			while (e is not null) {
				this.error(
					$"{e.GetType().Name}: {e.Message}\n",
					ChatColour.QUIET,
					e.TargetSite?.DeclaringType is not null ? $"at {e.TargetSite.DeclaringType.FullName} in {e.TargetSite.DeclaringType.Assembly}" : "at unknown location",
					ChatColour.RESET
				);
				e = e.InnerException!;
			}
		}
	}
}
