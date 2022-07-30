#if DEBUG
namespace TinyCmds.Commands;

using TinyCmds.Attributes;
using TinyCmds.Utils;

[Command("/tinydebug")]
[HelpMessage("Specifically for dev use")]
[DoNotShowInHelp]
[HideInCommandListing]
public class Debug: PluginCommand {
	protected override void Execute(string? command, string argline, FlagMap flags, bool verbose, bool dryRun, ref bool showHelp) {
		// nop until I need to debug something lol
	}
}
#endif
