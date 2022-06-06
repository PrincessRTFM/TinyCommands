namespace TinyCmds;

using System.Diagnostics.CodeAnalysis;
using System.Linq;

using TinyCmds.Attributes;
using TinyCmds.Chat;
using TinyCmds.Utils;


[SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Plugin command methods are delegates")]
public static partial class PluginCommands {
	[Command("/tinyhelp")]
	[Arguments("command...?")]
	[Summary("Displays usage/help for the plugin's commands")]
	[Aliases("/ptinyhelp", "/thelp", "/pthelp", "/tinycmd", "/ptinycmd", "/tcmd", "/ptcmd")]
	[HelpMessage("This command displays the extended usage and help for plugin commands.", "Run it alone for general/basic information.")]
	[PluginCommandHelpHandler]
	public static void DisplayPluginCommandHelp(string? command, string args, FlagMap flags, ref bool showHelp) {
		if (args.Length < 1) {
			ChatUtil.ShowPrefixedMessage($"{Plugin.PluginName} uses a custom command parser that accepts single-character boolean flags starting with a hyphen.");
			ChatUtil.ShowPrefixedMessage(
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
			ChatUtil.ShowPrefixedMessage(
				"All plugin commands accept ",
				ChatColour.HIGHLIGHT,
				"-h",
				ChatColour.RESET,
				" to display their built-in help message."
			);
			ChatUtil.ShowPrefixedMessage(
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
			foreach (PluginCommand cmd in Plugin.commandManager.commands) {
				if (cmd.CommandComparable.Equals(wanted) || cmd.AliasesComparable.Contains(wanted)) {
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
					foreach (string line in cmd.HelpLines) {
						ChatUtil.ShowPrefixedMessage(
							ChatColour.HELP_TEXT,
							line,
							ChatColour.RESET
						);
					}
					return;
				}
			}
			ChatUtil.ShowPrefixedError($"Couldn't find plugin command '/{wanted}'");
		}
	}
}