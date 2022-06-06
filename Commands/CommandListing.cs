namespace TinyCmds;

using TinyCmds.Attributes;
using TinyCmds.Chat;
using TinyCmds.Utils;


public static partial class PluginCommands {
	[Command("/tinycmds")]
	[Arguments()]
	[Summary("List all plugin commands, along with their help messages")]
	[Aliases("/ptinycmds", "/tcmds", "/ptcmds")]
	[HelpMessage(
		"Lists all plugin commands.",
		"Use \"-a\" to include command aliases, \"-v\" to include help messages, or both (\"-av\" or \"-va\" or separately) for both."
	)]
	public static void ListPluginCommands(string? command, string args, FlagMap flags, ref bool showHelp) {
		foreach (PluginCommand cmd in Plugin.commandManager.commands) {
			ChatUtil.ShowPrefixedMessage(
				ChatColour.USAGE_TEXT,
				cmd.Usage,
				ChatColour.RESET
			);
			if (flags["a"] && cmd.Aliases.Length > 0) {
				ChatUtil.ShowPrefixedMessage(
					ChatColour.QUIET,
					string.Join(", ", cmd.Aliases),
					ChatColour.RESET
				);
			}
			if (flags["v"]) {
				foreach (string line in cmd.HelpLines) {
					ChatUtil.ShowPrefixedMessage(
						ChatColour.HELP_TEXT,
						line,
						ChatColour.RESET
					);
				}
			}
		}
	}
}