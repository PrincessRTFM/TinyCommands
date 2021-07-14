using System.Linq;

using Dalamud.Plugin;

using TinyCmds.Attributes;

namespace TinyCmds {
	public partial class TinyCmdsPlugin: IDalamudPlugin {
		[Command("/tinyhelp")]
		[Arguments("command...?")]
		[Summary("Displays usage/help for the plugin's commands")]
		[Aliases("/ptinyhelp", "/thelp", "/pthelp", "/tinycmd", "/ptinycmd", "/tcmd", "/ptcmd")]
		[HelpMessage("This command displays the extended usage and help for plugin commands.", "Run it alone for general/basic information.")]
		public void DisplayPluginCommandHelp(string command, string[] args, FlagMap flags) {
			if (args.Length < 1) {
				this.SendPrefixedChat($"{this.Name} uses a custom command parser that accepts single-character boolean flags starting with a hyphen.");
				this.SendPrefixedChat(
					"These flags can be bundled into one argument, such that ",
					ChatColour.YELLOW,
					"-va",
					ChatColour.NONE,
					" will set both the ",
					ChatColour.BROWN,
					"v",
					ChatColour.NONE,
					" and ",
					ChatColour.BROWN,
					"a",
					ChatColour.NONE,
					" flags."
				);
				this.SendPrefixedChat(
					"All plugin commands accept ",
					ChatColour.YELLOW,
					"-h",
					ChatColour.NONE,
					" to display their built-in help message."
				);
				this.SendPrefixedChat(
					"To list all commands, use ",
					ChatColour.TEAL,
					"/tinycmds",
					ChatColour.NONE,
					", optionally with ",
					ChatColour.YELLOW,
					"-a",
					ChatColour.NONE,
					" to show their aliases and/or ",
					ChatColour.YELLOW,
					"-v",
					ChatColour.NONE,
					" to show their help messages."
				);
				return;
			}
			foreach (string listing in args) {
				string wanted = listing.TrimStart('/').ToLower();
				foreach (TinyCmdPluginCommandManager.PluginCommand cmd in this.CommandManager.Commands) {
					if (cmd.CommandComparable.Equals(wanted) || cmd.AliasesComparable.Contains(wanted)) {
						this.SendPrefixedChat(ChatColour.LIGHTBLUE, cmd.Usage, ChatColour.NONE);
						if (flags["a"] && cmd.Aliases.Length > 0) {
							this.SendPrefixedChat(
								ChatColour.GREY,
								string.Join(", ", cmd.Aliases),
								ChatColour.NONE
							);
						}
						foreach (string line in cmd.HelpLines) {
							this.SendPrefixedChat(
								ChatColour.PALEGREEN,
								line,
								ChatColour.NONE
							);
						}
						return;
					}
				}
				this.SendChatError($"Couldn't find plugin command '/{wanted}'");
			}
		}
	}
}
