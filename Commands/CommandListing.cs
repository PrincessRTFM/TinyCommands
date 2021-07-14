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
		public void ListPluginCommands(string command, string[] args, FlagMap flags) {
			foreach (TinyCmdPluginCommandManager.PluginCommand cmd in this.CommandManager.Commands) {
				this.SendPrefixedChat(
					ChatColour.USAGE_TEXT,
					cmd.Usage,
					ChatColour.RESET
				);
				if (flags["a"] && cmd.Aliases.Length > 0) {
					this.SendPrefixedChat(
						ChatColour.QUIET,
						string.Join(", ", cmd.Aliases),
						ChatColour.RESET
					);
				}
				if (flags["v"]) {
					foreach (string line in cmd.HelpLines) {
						this.SendPrefixedChat(
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