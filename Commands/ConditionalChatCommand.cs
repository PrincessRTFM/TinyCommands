using Dalamud.Game.ClientState;
using Dalamud.Game.ClientState.Actors;
using Dalamud.Plugin;

using TinyCmds.Attributes;

namespace TinyCmds {
	public partial class TinyCmdsPlugin: IDalamudPlugin {
		[Command("/ifcmd")]
		[Summary("Run a chat command (or directly send a message) only if a condition is met")]
		[Aliases("/ifthen")]
		[HelpMessage(
			"If the condition indicated by the flags is met, then all of the arguments will be executed as if entered into the chatbox manually. If no command/message is given, the test will print the result to your chatlog.",
			"Lowercase flags require that their condition be met, uppercase flags require that their condition NOT be met. Available flags are:",
			"-t has target, -f has focus, -o has mouseover, -c in combat, -p target is player, -n target is NPC, -m target is minion"
		)]
		[RawArgs]
		public void RunChatIfCond(string command, string[] args, FlagMap flags) {
			//PlayerCharacter player = this.Interface.ClientState.LocalPlayer;
			//PartyList party = this.Interface.ClientState.PartyList;
			Targets targets = this.Interface.ClientState.Targets;
			Condition cond = this.Interface.ClientState.Condition;
			PluginUtil.Colour msgCol = PluginUtil.Colour.ORANGE;
			string msg = "Test passed but no command given";
			if (flags["t"] && targets.CurrentTarget is null)
				msg = "No target";
			else if (flags["T"] && targets.CurrentTarget is not null)
				msg = "Target present";
			else if (flags["p"] && targets.CurrentTarget?.ObjectKind is not ObjectKind.Player)
				msg = "Target is not player";
			else if (flags["P"] && targets.CurrentTarget?.ObjectKind is ObjectKind.Player)
				msg = "Target is player";
			else if (flags["n"] && targets.CurrentTarget?.ObjectKind is not ObjectKind.BattleNpc or ObjectKind.EventNpc or ObjectKind.Retainer)
				msg = "Target is not NPC";
			else if (flags["N"] && targets.CurrentTarget?.ObjectKind is ObjectKind.BattleNpc or ObjectKind.EventNpc or ObjectKind.Retainer)
				msg = "Target is NPC";
			else if (flags["m"] && targets.CurrentTarget?.ObjectKind is not ObjectKind.Companion)
				msg = "Target is not minion";
			else if (flags["M"] && targets.CurrentTarget?.ObjectKind is ObjectKind.Companion)
				msg = "Target is minion";
			else if (flags["f"] && targets.FocusTarget is null)
				msg = "No focus target";
			else if (flags["F"] && targets.FocusTarget is not null)
				msg = "Focus target present";
			else if (flags["o"] && targets.MouseOverTarget is null)
				msg = "No mouseover target";
			else if (flags["O"] && targets.MouseOverTarget is not null)
				msg = "Mouseover target present";
			else if (flags["c"] && !cond[ConditionFlag.InCombat])
				msg = "Not in combat";
			else if (flags["C"] && cond[ConditionFlag.InCombat])
				msg = "In combat";
			else
				msgCol = PluginUtil.Colour.GREEN;
			if (args.Length > 0) {
				if (msgCol == PluginUtil.Colour.GREEN) {
					this.Util.SendServerChat(args[0]);
				}
			}
			else {
				this.Util.SendPrefixedChat(msgCol, msg, PluginUtil.Colour.NONE);
			}
		}
	}
}
