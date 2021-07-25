using Dalamud.Plugin;

using TinyCmds.Attributes;

namespace TinyCmds {
	public partial class TinyCmdsPlugin: IDalamudPlugin {
		[Command("/tinycmds")]
		[Arguments()]
		[Summary("List all plugin commands, along with their help messages")]
		[Aliases("/ptinycmds", "/tcmds", "/ptcmds")]
		[HelpMessage(
			"Lists all plugin commands.",
			"Use \"-a\" to include command aliases, \"-v\" to include help messages, or both (\"-av\" or \"-va\" or separately) for both."
		)]
		public void ListPluginCommands(string command, string args, FlagMap flags, ref bool showHelp) {
			foreach (TinyCmdPluginCommandManager.PluginCommand cmd in this.CommandManager.Commands) {
				this.ShowPrefixedChatMessage(
					ChatColour.USAGE_TEXT,
					cmd.Usage,
					ChatColour.RESET
				);
				if (flags["a"] && cmd.Aliases.Length > 0) {
					this.ShowPrefixedChatMessage(
						ChatColour.QUIET,
						string.Join(", ", cmd.Aliases),
						ChatColour.RESET
					);
				}
				if (flags["v"]) {
					foreach (string line in cmd.HelpLines) {
						this.ShowPrefixedChatMessage(
							ChatColour.HELP_TEXT,
							line,
							ChatColour.RESET
						);
					}
				}
			}
		}
	}
}