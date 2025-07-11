#if DEBUG

using VariableVixen.TinyCmds.Attributes;

using VariableVixen.TinyCmds.Utils;

namespace VariableVixen.TinyCmds.Commands;

[Command("/tinydebug")]
[Summary("Do not touch")]
[HelpText("This command is specifically for dev use. It does whatever is needed at the moment, and is neither consistent nor stable.", "", "It also doesn't exist in non-debug builds.")]
[DoNotShowInHelp]
[HideInCommandListing]
public class Debug: PluginCommand {
	protected override void Execute(string? command, string rawArguments, FlagMap flags, bool verbose, bool dryRun, ref bool showHelp) {
		// nop until I need to debug something lol
	}
}
#endif
