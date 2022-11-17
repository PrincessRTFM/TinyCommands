namespace PrincessRTFM.TinyCmds.Commands;

using PrincessRTFM.TinyCmds.Attributes;
using PrincessRTFM.TinyCmds.Utils;

[Command("/noop")]
[Summary("Does absolutely nothing. At all. Literally not a single thing.")]
[Aliases("/nop", "/null")]
[HelpMessage(
	"This command does literally nothing at all."
	+ " Its only purpose is for use with Macrology or similar plugins, to allow pseudo-macros to use <wait.(delay)> without doing anything."
)]
public class NoOp: PluginCommand {
	protected override void Execute(string? command, string args, FlagMap flags, bool verbose, bool dryRun, ref bool showHelp) { }
}
