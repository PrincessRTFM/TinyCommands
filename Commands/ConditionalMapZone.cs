namespace TinyCmds;

using System;
using System.Linq;

using TinyCmds.Attributes;
using TinyCmds.Chat;
using TinyCmds.Utils;


public static partial class PluginCommands {
	[Command("/ifzone")]
	[Arguments("'-n'?", "zone IDs to match against", "command to run...?")]
	[Summary("Run a chat command (or directly send a message) only when in a certain map zone")]
	[Aliases("/ifmap", "/ifmapzone")]
	[HelpMessage(
		"Much like /ifcmd, this command executes a given command when the condition is met.",
		"In this case, the condition is whether or not your current zone ID is one of the given set.",
		"Use the numeric ID, and if you want to check against more than one, separate them with commas but NOT spaces.",
		"If you pass the -n flag, the match will be inverted so the command runs only when you AREN'T one of those jobs.",
		"Using -g will print your current zone ID, to make it easier to find the one you want."
	)]
	public static void RunIfInMapZone(string? command, string args, FlagMap flags, ref bool showHelp) {
		string arg = args ?? string.Empty;
		ushort territory = Plugin.client.TerritoryType;
		if (territory == 0) {
			ChatUtil.ShowPrefixedError("Cannot identify current area");
			return;
		}
		else if (flags['g']) {
			ChatUtil.ShowPrefixedMessage($"You are in map zone {territory}");
			return;
		}
		string wantedMapZones = arg.Split()[0];
		if (string.IsNullOrEmpty(wantedMapZones)) {
			showHelp = true;
			return;
		}
		string cmd = arg.Contains(' ')
			? arg[(wantedMapZones.Length + 1)..]
			: string.Empty;
		bool invert = flags["n"];
		bool match = wantedMapZones.Split(',').Contains(territory.ToString());
		if (match ^ invert) {
			if (cmd.Length > 0) {
				ChatUtil.SendChatlineToServer(cmd, flags["d"] || flags['v'], flags['d']);
			}
			else {
				ChatUtil.ShowPrefixedMessage(
					ChatColour.CONDITION_PASSED,
					"You are currently in zone ",
					ChatGlow.CONDITION_PASSED,
					territory,
					ChatGlow.RESET,
					ChatColour.RESET
				);
			}
		}
		else if (cmd.Length < 1) {
			ChatUtil.ShowPrefixedMessage(
				ChatColour.CONDITION_FAILED,
				"You are currently in zone ",
				ChatGlow.CONDITION_FAILED,
				territory,
				ChatGlow.RESET,
				ChatColour.RESET
			);
		}
	}
}
