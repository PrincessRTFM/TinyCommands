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
		public void DisplayPluginCommandHelp(string command, string args, FlagMap flags, ref bool showHelp) {
			if (args.Length < 1) {
				this.SendPrefixedChat($"{this.Name} uses a custom command parser that accepts single-character boolean flags starting with a hyphen.");
				this.SendPrefixedChat(
					"These flags can be bundled into one argument, such that ",
					ChatColour.HIGHLIGHT,
					"-va",
					ChatColour.RESET,
					" will set both the ",
					ChatColour.HIGHLIGHT,
					"v",
					ChatColour.RESET,
					" and ",
					ChatColour.HIGHLIGHT,
					"a",
					ChatColour.RESET,
					" flags."
				);
				this.SendPrefixedChat(
					"All plugin commands accept ",
					ChatColour.HIGHLIGHT,
					"-h",
					ChatColour.RESET,
					" to display their built-in help message."
				);
				this.SendPrefixedChat(
					"To list all commands, use ",
					ChatColour.COMMAND,
					"/tinycmds",
					ChatColour.RESET,
					", optionally with ",
					ChatColour.HIGHLIGHT,
					"-a",
					ChatColour.RESET,
					" to show their aliases and/or ",
					ChatColour.HIGHLIGHT,
					"-v",
					ChatColour.RESET,
					" to show their help messages."
				);
				return;
			}
			foreach (string listing in ShellParse(args)) {
				string wanted = listing.TrimStart('/').ToLower();
				foreach (TinyCmdPluginCommandManager.PluginCommand cmd in this.CommandManager.Commands) {
					if (cmd.CommandComparable.Equals(wanted) || cmd.AliasesComparable.Contains(wanted)) {
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
						foreach (string line in cmd.HelpLines) {
							this.SendPrefixedChat(
								ChatColour.HELP_TEXT,
								line,
								ChatColour.RESET
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
