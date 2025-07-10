using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.Enums;

using VariableVixen.TinyCmds.Attributes;

using VariableVixen.TinyCmds.Chat;
using VariableVixen.TinyCmds.Utils;

namespace VariableVixen.TinyCmds.Commands.Conditional;

[Command("/ifcmd", "/ifthen", "/ifcondition", "/ifcond", "/ifstate")]
[Arguments("condition flags", "command to run...?")]
[Summary("Run a chat command (or directly send a message) only if a condition is met")]
[HelpText(
	"This command's test is based on game state, as described by the flags you use."
	+ " Lowercase flags require that their condition be met, uppercase flags require that their condition NOT be met. The available flags are:",
	"-t has target",
	"-f has focus",
	"-o has mouseover",
	"-c in combat",
	"-p target is player",
	"-n target is NPC",
	"-m target is minion",
	"-w unmounted",
	"-s swimming",
	"-d diving",
	"-u flying",
	"-i in duty",
	"-l using fashion accessory",
	"-r weapon drawn",
	"-a in alliance",
	"-g has party members"
)]
public class ConditionalGameFlags: BaseConditionalCommand {
	protected override bool TryExecute(string? command, string rawArguments, FlagMap flags, bool verbose, bool dryRun, ref bool showHelp) {
		ChatColour msgCol = ChatColour.CONDITION_FAILED;
		string msg = "Test passed but no command given";

		if (flags["t"] && Plugin.Targets.Target is null)
			msg = "No target";
		else if (flags["T"] && Plugin.Targets.Target is not null)
			msg = "Target present";
		else if (flags["p"] && Plugin.Targets.Target?.ObjectKind is not ObjectKind.Player)
			msg = "Target is not player";
		else if (flags["P"] && Plugin.Targets.Target?.ObjectKind is ObjectKind.Player)
			msg = "Target is player";
		else if (flags["n"] && Plugin.Targets.Target?.ObjectKind is not (ObjectKind.BattleNpc or ObjectKind.EventNpc or ObjectKind.Retainer))
			msg = "Target is not NPC";
		else if (flags["N"] && Plugin.Targets.Target?.ObjectKind is ObjectKind.BattleNpc or ObjectKind.EventNpc or ObjectKind.Retainer)
			msg = "Target is NPC";
		else if (flags["m"] && Plugin.Targets.Target?.ObjectKind is not ObjectKind.Companion)
			msg = "Target is not minion";
		else if (flags["M"] && Plugin.Targets.Target?.ObjectKind is ObjectKind.Companion)
			msg = "Target is minion";
		else if (flags["f"] && Plugin.Targets.FocusTarget is null)
			msg = "No focus target";
		else if (flags["F"] && Plugin.Targets.FocusTarget is not null)
			msg = "Focus target present";
		else if (flags["o"] && Plugin.Targets.MouseOverTarget is null)
			msg = "No mouseover target";
		else if (flags["O"] && Plugin.Targets.MouseOverTarget is not null)
			msg = "Mouseover target present";
		else if (flags["c"] && !Plugin.Conditions[ConditionFlag.InCombat])
			msg = "Not in combat";
		else if (flags["C"] && Plugin.Conditions[ConditionFlag.InCombat])
			msg = "In combat";
		else if (flags["w"] && Plugin.Conditions[ConditionFlag.Mounted])
			msg = "Not unmounted";
		else if (flags["W"] && !Plugin.Conditions[ConditionFlag.Mounted])
			msg = "Unmounted";
		else if (flags["s"] && !Plugin.Conditions[ConditionFlag.Swimming])
			msg = "Not swimming";
		else if (flags["S"] && Plugin.Conditions[ConditionFlag.Swimming])
			msg = "Swimming";
		else if (flags["d"] && !Plugin.Conditions[ConditionFlag.Diving])
			msg = "Not diving";
		else if (flags["D"] && Plugin.Conditions[ConditionFlag.Diving])
			msg = "Diving";
		else if (flags["u"] && !Plugin.Conditions[ConditionFlag.InFlight])
			msg = "Not flying";
		else if (flags["U"] && Plugin.Conditions[ConditionFlag.InFlight])
			msg = "Flying";
		else if (flags["i"] && !Plugin.Conditions[ConditionFlag.BoundByDuty])
			msg = "Not in duty";
		else if (flags["I"] && Plugin.Conditions[ConditionFlag.BoundByDuty])
			msg = "In duty";
		else if (flags["l"] && !Plugin.Conditions[ConditionFlag.UsingFashionAccessory])
			msg = "Not using fashion accessory";
		else if (flags["L"] && Plugin.Conditions[ConditionFlag.UsingFashionAccessory])
			msg = "Using fashion accessory";
		else if (flags["r"] && !Plugin.Client.LocalPlayer!.StatusFlags.HasFlag(StatusFlags.WeaponOut))
			msg = "No weapon drawn";
		else if (flags["R"] && Plugin.Client.LocalPlayer!.StatusFlags.HasFlag(StatusFlags.WeaponOut))
			msg = "Weapon drawn";
		else if (flags["a"] && !Plugin.Party.IsAlliance)
			msg = "Not in an alliance";
		else if (flags["A"] && Plugin.Party.IsAlliance)
			msg = "In an alliance";
		else if (flags['g'] && Plugin.Party.Length < 1)
			msg = "No party members present";
		else if (flags['G'] && Plugin.Party.Length > 0)
			msg = "Party members present";
		else
			msgCol = ChatColour.CONDITION_PASSED;

		if (msgCol == ChatColour.CONDITION_PASSED) {
			if (rawArguments.Length > 0)
				ChatUtil.SendChatlineToServer(rawArguments, dryRun || verbose, dryRun);
			else
				ChatUtil.ShowPrefixedMessage(msgCol, msg, ChatColour.RESET);
			return true;
		}

		if (rawArguments.Length == 0)
			ChatUtil.ShowPrefixedMessage(msgCol, msg, ChatColour.RESET);
		return false;
	}
}
