namespace TinyCmds;

using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.Enums;

using TinyCmds.Attributes;
using TinyCmds.Chat;
using TinyCmds.Utils;

public static partial class PluginCommands {
	[Command("/ifcmd")]
	[Arguments("condition flags", "command to run...?")]
	[Summary("Run a chat command (or directly send a message) only if a condition is met")]
	[Aliases("/ifthen")]
	[HelpMessage(
		"If the condition indicated by the flags is met, then all of the arguments will be executed as if entered into the chatbox manually. If no command/message is given, the test will print the result to your chatlog.",
		"Lowercase flags require that their condition be met, uppercase flags require that their condition NOT be met. Available flags are:",
		"-t has target, -f has focus, -o has mouseover, -c in combat, -p target is player, -n target is NPC, -m target is minion, -w unmounted, -s swimming, -d diving, -u flying, -i in duty, -l using fashion accessory, -r weapon drawn, -a in alliance"
	)]
	public static void RunChatIfCond(string? command, string args, FlagMap flags, ref bool showHelp) {
		if (Plugin.client.LocalPlayer is null) {
			ChatUtil.ShowPrefixedError("Can't find player object - this should be impossible unless you're not logged in.");
			return;
		}
		ChatColour msgCol = ChatColour.CONDITION_FAILED;
		string msg = "Test passed but no command given";
		if (flags["t"] && Plugin.targets.Target is null)
			msg = "No target";
		else if (flags["T"] && Plugin.targets.Target is not null)
			msg = "Target present";
		else if (flags["p"] && Plugin.targets.Target?.ObjectKind is not ObjectKind.Player)
			msg = "Target is not player";
		else if (flags["P"] && Plugin.targets.Target?.ObjectKind is ObjectKind.Player)
			msg = "Target is player";
		else if (flags["n"] && Plugin.targets.Target?.ObjectKind is not ObjectKind.BattleNpc or ObjectKind.EventNpc or ObjectKind.Retainer)
			msg = "Target is not NPC";
		else if (flags["N"] && Plugin.targets.Target?.ObjectKind is ObjectKind.BattleNpc or ObjectKind.EventNpc or ObjectKind.Retainer)
			msg = "Target is NPC";
		else if (flags["m"] && Plugin.targets.Target?.ObjectKind is not ObjectKind.Companion)
			msg = "Target is not minion";
		else if (flags["M"] && Plugin.targets.Target?.ObjectKind is ObjectKind.Companion)
			msg = "Target is minion";
		else if (flags["f"] && Plugin.targets.FocusTarget is null)
			msg = "No focus target";
		else if (flags["F"] && Plugin.targets.FocusTarget is not null)
			msg = "Focus target present";
		else if (flags["o"] && Plugin.targets.MouseOverTarget is null)
			msg = "No mouseover target";
		else if (flags["O"] && Plugin.targets.MouseOverTarget is not null)
			msg = "Mouseover target present";
		else if (flags["c"] && !Plugin.conditions[ConditionFlag.InCombat])
			msg = "Not in combat";
		else if (flags["C"] && Plugin.conditions[ConditionFlag.InCombat])
			msg = "In combat";
		else if (flags["w"] && Plugin.conditions[ConditionFlag.Mounted])
			msg = "Not unmounted";
		else if (flags["W"] && !Plugin.conditions[ConditionFlag.Mounted])
			msg = "Unmounted";
		else if (flags["s"] && !Plugin.conditions[ConditionFlag.Swimming])
			msg = "Not swimming";
		else if (flags["S"] && Plugin.conditions[ConditionFlag.Swimming])
			msg = "Swimming";
		else if (flags["d"] && !Plugin.conditions[ConditionFlag.Diving])
			msg = "Not diving";
		else if (flags["D"] && Plugin.conditions[ConditionFlag.Diving])
			msg = "Diving";
		else if (flags["u"] && !Plugin.conditions[ConditionFlag.InFlight])
			msg = "Not flying";
		else if (flags["U"] && Plugin.conditions[ConditionFlag.InFlight])
			msg = "Flying";
		else if (flags["i"] && !Plugin.conditions[ConditionFlag.BoundByDuty])
			msg = "Not in duty";
		else if (flags["I"] && Plugin.conditions[ConditionFlag.BoundByDuty])
			msg = "In duty";
		else if (flags["l"] && !Plugin.conditions[ConditionFlag.UsingParasol])
			msg = "Not using fashion accessory";
		else if (flags["L"] && Plugin.conditions[ConditionFlag.UsingParasol])
			msg = "Using fashion accessory";
		else if (flags["r"] && !Plugin.client.LocalPlayer.StatusFlags.HasFlag(StatusFlags.WeaponOut))
			msg = "No weapon drawn";
		else if (flags["R"] && Plugin.client.LocalPlayer.StatusFlags.HasFlag(StatusFlags.WeaponOut))
			msg = "Weapon drawn";
		else if (flags["a"] && !Plugin.party.IsAlliance)
			msg = "Not in an alliance";
		else if (flags["A"] && Plugin.party.IsAlliance)
			msg = "In an alliance";
		else
			msgCol = ChatColour.CONDITION_PASSED;
		if (args.Length > 0) {
			if (msgCol == ChatColour.CONDITION_PASSED) {
				ChatUtil.SendChatlineToServer(args, flags["d"] || flags['v'], flags['d']);
			}
		}
		else {
			ChatUtil.ShowPrefixedMessage(msgCol, msg, ChatColour.RESET);
		}
	}
}
