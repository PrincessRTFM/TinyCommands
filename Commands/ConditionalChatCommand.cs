using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.Enums;

using TinyCmds.Attributes;
using TinyCmds.Chat;
using TinyCmds.Utils;

namespace TinyCmds {
	public static partial class PluginCommands {
		[Command("/ifcmd")]
		[Arguments("condition flags", "command to run...?")]
		[Summary("Run a chat command (or directly send a message) only if a condition is met")]
		[Aliases("/ifthen")]
		[HelpMessage(
			"If the condition indicated by the flags is met, then all of the arguments will be executed as if entered into the chatbox manually. If no command/message is given, the test will print the result to your chatlog.",
			"Lowercase flags require that their condition be met, uppercase flags require that their condition NOT be met. Available flags are:",
			"-t has target, -f has focus, -o has mouseover, -c in combat, -p target is player, -n target is NPC, -m target is minion, -w unmounted, -s swimming, -d diving, -u flying, -i in duty, -l using fashion accessory"
		)]
		public static void RunChatIfCond(string? command, string args, FlagMap flags, ref bool showHelp) {
			if (TinyCmds.client.LocalPlayer is null) {
				ChatUtil.ShowPrefixedError("Can't find player object - this should be impossible unless you're not logged in.");
				return;
			}
			ChatColour msgCol = ChatColour.CONDITION_FAILED;
			string msg = "Test passed but no command given";
			if (flags["t"] && TinyCmds.targets.Target is null)
				msg = "No target";
			else if (flags["T"] && TinyCmds.targets.Target is not null)
				msg = "Target present";
			else if (flags["p"] && TinyCmds.targets.Target?.ObjectKind is not ObjectKind.Player)
				msg = "Target is not player";
			else if (flags["P"] && TinyCmds.targets.Target?.ObjectKind is ObjectKind.Player)
				msg = "Target is player";
			else if (flags["n"] && TinyCmds.targets.Target?.ObjectKind is not ObjectKind.BattleNpc or ObjectKind.EventNpc or ObjectKind.Retainer)
				msg = "Target is not NPC";
			else if (flags["N"] && TinyCmds.targets.Target?.ObjectKind is ObjectKind.BattleNpc or ObjectKind.EventNpc or ObjectKind.Retainer)
				msg = "Target is NPC";
			else if (flags["m"] && TinyCmds.targets.Target?.ObjectKind is not ObjectKind.Companion)
				msg = "Target is not minion";
			else if (flags["M"] && TinyCmds.targets.Target?.ObjectKind is ObjectKind.Companion)
				msg = "Target is minion";
			else if (flags["f"] && TinyCmds.targets.FocusTarget is null)
				msg = "No focus target";
			else if (flags["F"] && TinyCmds.targets.FocusTarget is not null)
				msg = "Focus target present";
			else if (flags["o"] && TinyCmds.targets.MouseOverTarget is null)
				msg = "No mouseover target";
			else if (flags["O"] && TinyCmds.targets.MouseOverTarget is not null)
				msg = "Mouseover target present";
			else if (flags["c"] && !TinyCmds.conditions[ConditionFlag.InCombat])
				msg = "Not in combat";
			else if (flags["C"] && TinyCmds.conditions[ConditionFlag.InCombat])
				msg = "In combat";
			else if (flags["w"] && TinyCmds.conditions[ConditionFlag.Mounted])
				msg = "Not unmounted";
			else if (flags["W"] && !TinyCmds.conditions[ConditionFlag.Mounted])
				msg = "Unmounted";
			else if (flags["s"] && !TinyCmds.conditions[ConditionFlag.Swimming])
				msg = "Not swimming";
			else if (flags["S"] && TinyCmds.conditions[ConditionFlag.Swimming])
				msg = "Swimming";
			else if (flags["d"] && !TinyCmds.conditions[ConditionFlag.Diving])
				msg = "Not diving";
			else if (flags["D"] && TinyCmds.conditions[ConditionFlag.Diving])
				msg = "Diving";
			else if (flags["u"] && !TinyCmds.conditions[ConditionFlag.InFlight])
				msg = "Not flying";
			else if (flags["U"] && TinyCmds.conditions[ConditionFlag.InFlight])
				msg = "Flying";
			else if (flags["i"] && !TinyCmds.conditions[ConditionFlag.BoundByDuty])
				msg = "Not in duty";
			else if (flags["I"] && TinyCmds.conditions[ConditionFlag.BoundByDuty])
				msg = "In duty";
			else if (flags["l"] && !TinyCmds.conditions[ConditionFlag.UsingParasol])
				msg = "Not using fashion accessory";
			else if (flags["L"] && TinyCmds.conditions[ConditionFlag.UsingParasol])
				msg = "Using fashion accessory";
			//else if (flags["a"] && !XXXXX)
			//	msg = "Not in an alliance";
			//else if (flags["A"] && XXXXX)
			//	msg = "In an alliance";
			else
				msgCol = ChatColour.CONDITION_PASSED;
			if (args.Length > 0) {
				if (msgCol == ChatColour.CONDITION_PASSED) {
					ChatUtil.SendChatlineToServer(args);
				}
			}
			else {
				ChatUtil.ShowPrefixedMessage(msgCol, msg, ChatColour.RESET);
			}
		}
	}
}
