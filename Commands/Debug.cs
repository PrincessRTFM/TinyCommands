#if DEBUG
using TinyCmds.Attributes;
using TinyCmds.Utils;

namespace TinyCmds {
	public static partial class PluginCommands {
		[Command("/tinydebug")]
		[HelpMessage("Specifically for dev use")]
		[DoNotShowInHelp]
		[HideInCommandListing]
		public static void PluginDebugCommand(string? command, string argline, FlagMap flags, ref bool showHelp) {
			// nop until I need to debug something lol
		}
	}
}
#endif