namespace TinyCmds;

using System;
using System.Linq;
using System.Reflection;

using Dalamud.Game.Command;

using TinyCmds.Attributes;
using TinyCmds.Chat;
using TinyCmds.Utils;

public abstract class PluginCommand: IDisposable {
	protected bool Disposed = false;

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
	public string[] InvocationNames => (new string[] { this.Command }).Concat(this.Aliases).ToArray();
	public string[] InvocationNamesComparable => (new string[] { this.CommandComparable }).Concat(this.AliasesComparable).ToArray();
	public string Command { get; }
	public string Summary { get; }
	public string Help { get; }
	public string Usage { get; }
	public string[] Aliases { get; }
	public bool ShowInDalamud { get; }
	public bool ShowInListing { get; }

	public readonly string InternalName;

	public PluginCommand() {
		Type t = this.GetType();
		CommandAttribute? attrCommand = t.GetCustomAttribute<CommandAttribute>();
		if (attrCommand is null) {
			throw new NullReferenceException("Cannot construct PluginCommand from type without CommandAttribute");
		}
		ArgumentsAttribute? args = t.GetCustomAttribute<ArgumentsAttribute>();
		this.Command = $"/{attrCommand.Command.TrimStart('/')}";
		this.Summary = t.GetCustomAttribute<SummaryAttribute>()?.Summary ?? "";
		this.Help = this.ModifyHelpMessage(t.GetCustomAttribute<HelpMessageAttribute>()?.HelpMessage ?? "");
		this.Usage = $"{this.Command} {args?.ArgumentDescription}".Trim();
		this.Aliases = t.GetCustomAttribute<AliasesAttribute>()?.Aliases ?? Array.Empty<string>();
		this.ShowInListing = t.GetCustomAttribute<HideInCommandListingAttribute>() is null;
		this.ShowInDalamud = this.ShowInListing && (t.GetCustomAttribute<DoNotShowInHelpAttribute>() is null || string.IsNullOrEmpty(this.Summary));

		this.InternalName = this.GetType().Name;
	}

	protected abstract void Execute(string? command, string rawArguments, FlagMap flags, bool verbose, bool dryRun, ref bool showHelp);
	protected virtual string ModifyHelpMessage(string original) => original;

	public void Dispatch(string command, string argline) {
		if (this.Disposed)
			throw new ObjectDisposedException(this.InternalName, "Plugin command has already been disposed");

		try {
			(FlagMap flags, string rawArgs) = ArgumentParser.ExtractFlags(argline);
			bool showHelp = false;
			bool verbose = flags['?'];
			bool dryRun = flags['!'];
			if (flags["h"]) {
				Plugin.commandManager.HelpHandler?.Execute(null, command, flags, verbose, dryRun, ref showHelp);
				return;
			}
			this.Execute(command, rawArgs, flags, verbose, dryRun, ref showHelp);
			if (showHelp)
				Plugin.commandManager.HelpHandler?.Execute(null, command, flags, verbose, dryRun, ref showHelp);
		}
		catch (Exception e) {
			if (Plugin.commandManager.ErrorHandler is not null) {
				while (e is not null) {
					Plugin.commandManager.ErrorHandler.Invoke(
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

	#region IDisposable
	protected virtual void Dispose(bool disposing) {
		if (this.Disposed)
			return;
		this.Disposed = true;
	}

	public void Dispose() {
		this.Dispose(true);
		GC.SuppressFinalize(this);
	}
	#endregion
}
