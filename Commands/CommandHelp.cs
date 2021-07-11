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
				this.Util.SendPrefixedChat($"{this.Name} uses a custom command parser that accepts single-character boolean flags starting with a hyphen.");
				this.Util.SendPrefixedChat(
					"These flags can be bundled into one argument, such that ",
					PluginUtil.Colour.YELLOW,
					"-va",
					PluginUtil.Colour.NONE,
					" will set both the ",
					PluginUtil.Colour.BROWN,
					"v",
					PluginUtil.Colour.NONE,
					" and ",
					PluginUtil.Colour.BROWN,
					"a",
					PluginUtil.Colour.NONE,
					" flags."
				);
				this.Util.SendPrefixedChat(
					"All plugin commands accept ",
					PluginUtil.Colour.YELLOW,
					"-h",
					PluginUtil.Colour.NONE,
					" to display their built-in help message."
				);
				this.Util.SendPrefixedChat(
					"To list all commands, use ",
					PluginUtil.Colour.TEAL,
					"/tinycmds",
					PluginUtil.Colour.NONE,
					", optionally with ",
					PluginUtil.Colour.YELLOW,
					"-a",
					PluginUtil.Colour.NONE,
					" to show their aliases and/or ",
					PluginUtil.Colour.YELLOW,
					"-v",
					PluginUtil.Colour.NONE,
					" to show their help messages."
				);
				return;
			}
			foreach (string listing in args) {
				string wanted = listing.TrimStart('/').ToLower();
				foreach (TinyCmdPluginCommandManager.PluginCommand cmd in this.CommandManager.Commands) {
					if (cmd.CommandComparable.Equals(wanted) || cmd.AliasesComparable.Contains(wanted)) {
						this.Util.SendPrefixedChat(PluginUtil.Colour.LIGHTBLUE, cmd.Usage, PluginUtil.Colour.NONE);
						if (flags["a"] && cmd.Aliases.Length > 0) {
							this.Util.SendPrefixedChat(
								PluginUtil.Colour.GREY,
								string.Join(", ", cmd.Aliases),
								PluginUtil.Colour.NONE
							);
						}
						foreach (string line in cmd.HelpLines) {
							this.Util.SendPrefixedChat(
								PluginUtil.Colour.PALEGREEN,
								line,
								PluginUtil.Colour.NONE
							);
						}
						return;
					}
				}
				this.Util.SendChatError($"Couldn't find plugin command '/{wanted}'");
			}
		}
	}
}
