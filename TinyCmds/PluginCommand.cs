namespace PrincessRTFM.TinyCmds;

using System;
using System.Linq;
using System.Reflection;

using Dalamud.Game.Command;
using Dalamud.Logging;

using PrincessRTFM.TinyCmds.Attributes;
using PrincessRTFM.TinyCmds.Chat;
using PrincessRTFM.TinyCmds.Utils;

public abstract class PluginCommand: IDisposable {
	protected bool Disposed { get; set; } = false;

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

	protected Plugin Plugin { get; private set; }

	public string InternalName { get; init; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
	public PluginCommand() {
		Type t = this.GetType();
		CommandAttribute attrCommand = t.GetCustomAttribute<CommandAttribute>() ?? throw new InvalidOperationException("Cannot construct PluginCommand from type without CommandAttribute");
		ArgumentsAttribute? args = t.GetCustomAttribute<ArgumentsAttribute>();
		this.Command = $"/{attrCommand.Command.TrimStart('/')}";
		this.Summary = t.GetCustomAttribute<SummaryAttribute>()?.Summary ?? "";
		this.Help = this.ModifyHelpMessage(t.GetCustomAttribute<HelpTextAttribute>()?.HelpMessage ?? "");
		this.Usage = $"{this.Command} {args?.ArgumentDescription}".Trim();
		this.Aliases = (new string[] { "9999p" + this.Command.TrimStart('/') })
			.Concat(
				attrCommand.Aliases
					.Select(s => s.TrimStart('/'))
					.SelectMany(a => new string[] { "0000" + a, "9999p" + a })
			)
			.OrderBy(s => s)
			.Select(s => "/" + s[4..])
			.ToArray();
		this.ShowInListing = t.GetCustomAttribute<HideInCommandListingAttribute>() is null;
		this.ShowInDalamud = this.ShowInListing && (t.GetCustomAttribute<DoNotShowInHelpAttribute>() is null || string.IsNullOrEmpty(this.Summary));

		this.InternalName = t.Name;

		if (this.Plugin is null)
			PluginLog.Warning($"{this.InternalName}.Plugin is null in constructor - this should not happen!");
	}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

	protected abstract void Execute(string? command, string rawArguments, FlagMap flags, bool verbose, bool dryRun, ref bool showHelp);
	protected virtual string ModifyHelpMessage(string original) => original;
	protected virtual void Initialise() { }
	internal void setup() {
		PluginLog.Information($"Initialising {this.InternalName}");
		this.Initialise();
	}

	protected static void Assert(bool succeeds, string message) {
		if (!succeeds)
			throw new CommandAssertionFailureException(message);
	}

	public void Dispatch(string command, string argline) {
		if (this.Disposed)
			throw new ObjectDisposedException(this.InternalName, "Plugin command has already been disposed");

		PluginLog.Information($"Command invocation: [{command}] [{argline}]");
		try {
			(FlagMap flags, string rawArguments) = ArgumentParser.ExtractFlags(argline);
			PluginLog.Information($"Parsed flags: {flags}");
			PluginLog.Information($"Remaining argument line: [{rawArguments}]");
			bool showHelp = false;
			bool verbose = flags['?'];
			bool dryRun = flags['!'];
			if (flags["h"]) {
				Plugin.commandManager.HelpHandler?.Execute(null, command, flags, verbose, dryRun, ref showHelp);
				return;
			}
			this.Execute(command, rawArguments, flags, verbose, dryRun, ref showHelp);
			if (showHelp)
				Plugin.commandManager.HelpHandler?.Execute(null, command, flags, verbose, dryRun, ref showHelp);
		}
		catch (CommandAssertionFailureException e) {
			PluginLog.Error(e, $"Command assert failed: {this.Command}: {e.Message}");
			Plugin.commandManager.ErrorHandler?.Invoke($"Internal assertion check failed:\n{e.Message}");
		}
		catch (Exception e) {
			PluginLog.Error(e, "Command invocation failed");
			if (Plugin.commandManager.ErrorHandler is not null) {
				while (e is not null) {
					Plugin.commandManager.ErrorHandler.Invoke(
						$"{e.GetType().Name}: {e.Message}\n",
						ChatColour.QUIET,
						e.TargetSite?.DeclaringType is not null ? $"at {e.TargetSite.DeclaringType.FullName} in {e.TargetSite.DeclaringType.Assembly.GetName().Name}" : "at unknown location",
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

		this.Plugin = null!;
	}

	public void Dispose() {
		this.Dispose(true);
		GC.SuppressFinalize(this);
	}
	#endregion
}
