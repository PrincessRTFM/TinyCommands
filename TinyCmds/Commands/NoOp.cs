using VariableVixen.TinyCmds.Attributes;
using VariableVixen.TinyCmds.Utils;

namespace VariableVixen.TinyCmds.Commands;

[Command("/noop", "/nop", "/null")]
[Summary("Does absolutely nothing. At all. Literally not a single thing.")]
[HelpText(
	"This command does literally nothing at all."
	+ " Its only purpose is for use with Macrology or similar plugins, to allow pseudo-macros to use <wait.(delay)> without doing anything."
)]
public class NoOp: PluginCommand {
	protected override void Execute(string? command, string rawArguments, FlagMap flags, bool verbose, bool dryRun, ref bool showHelp) { }
}
