using System.Linq;

using Dalamud.Plugin;

using TinyCmds.Attributes;
using TinyCmds.Chat;
using TinyCmds.Utils;

namespace TinyCmds {
	public partial class TinyCmds: IDalamudPlugin {
		[Command("/tinyhelp")]
		[Arguments("command...?")]
		[Summary("Displays usage/help for the plugin's commands")]
		[Aliases("/ptinyhelp", "/thelp", "/pthelp", "/tinycmd", "/ptinycmd", "/tcmd", "/ptcmd")]
		[HelpMessage("This command displays the extended usage and help for plugin commands.", "Run it alone for general/basic information.")]
		public void DisplayPluginCommandHelp(string command, string args, FlagMap flags, ref bool showHelp) {
			if (args.Length < 1) {
				this.ShowPrefixedChatMessage($"{this.Name} uses a custom command parser that accepts single-character boolean flags starting with a hyphen.");
				this.ShowPrefixedChatMessage(
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
				this.ShowPrefixedChatMessage(
					"All plugin commands accept ",
					ChatColour.HIGHLIGHT,
					"-h",
					ChatColour.RESET,
					" to display their built-in help message."
				);
				this.ShowPrefixedChatMessage(
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
			foreach (string listing in ArgumentParser.ShellParse(args)) {
				string wanted = listing.TrimStart('/').ToLower();
				foreach (PluginCommand cmd in this.CommandManager.Commands) {
					if (cmd.CommandComparable.Equals(wanted) || cmd.AliasesComparable.Contains(wanted)) {
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
						foreach (string line in cmd.HelpLines) {
							this.ShowPrefixedChatMessage(
								ChatColour.HELP_TEXT,
								line,
								ChatColour.RESET
							);
						}
						return;
					}
				}
				this.ShowPrefixedChatError($"Couldn't find plugin command '/{wanted}'");
			}
		}
	}
}
