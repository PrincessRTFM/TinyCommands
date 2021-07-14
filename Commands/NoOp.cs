using Dalamud.Plugin;

using TinyCmds.Attributes;

namespace TinyCmds {
	public partial class TinyCmdsPlugin: IDalamudPlugin {
		[Command("/noop")]
		[Summary("Does absolutely nothing. At all. Literally not a single thing.")]
		[Aliases("/nop", "/null")]
		[HelpMessage(
			"This command does literally nothing at all.",
			"Its only purpose is for use with Macrology, to allow macro-macros to use <wait.(delay)> without doing anything."
		)]
		public void EmptyCommand(string command, string args, FlagMap flags, ref bool showHelp) { }
	}
}