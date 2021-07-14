using System.Linq;

using Dalamud.Plugin;

using TinyCmds.Attributes;

namespace TinyCmds {
	public partial class TinyCmdsPlugin: IDalamudPlugin {
		[Command("/ifjob")]
		[Arguments("'-n'?", "jobs to match against", "command to run...?")]
		[RawArgs]
		[Summary("Run a chat command (or directly send a message) only when playing certain classes/jobs")]
		[Aliases("/ifclass", "/whenjob", "/whenclass")]
		[HelpMessage(
			"Much like /ifcmd and /ifgp, this command executes a given command when the condition is met.",
			"In this case, the condition is whether or not your current class/job is one of the given set.",
			"Use the three-letter abbreviation, and if you want to check against more than one, separate them with commas but NOT spaces.",
			"If you pass the -n flag, the match will be inverted so the command runs only when you AREN'T one of those jobs."
		)]
		public void RunIfJobMatches(string command, string[] args, FlagMap flags) {
			string arg = args[0] ?? string.Empty;
			string currentJobName = this.Interface.ClientState.LocalPlayer.ClassJob.GameData.Abbreviation.ToString().ToUpper();
			string wantedJobNames = arg.Split()[0].ToUpper();
			string cmd = arg.Substring(wantedJobNames.Length).Trim();
			bool invert = flags["n"];
			bool match = wantedJobNames.Split(',').Contains(currentJobName);
			if (match ^ invert) {
				if (cmd.Length > 0) {
					this.SendServerChat(cmd);
				}
				else {
					this.SendPrefixedChat(
						ChatColour.CONDITION_PASSED,
						"You are currently a ",
						this.Glow(ChatColour.HIGHLIGHT_PASSED),
						currentJobName,
						this.Glow(ChatColour.RESET),
						ChatColour.RESET
					);
				}
			}
			else if (cmd.Length < 1) {
				this.SendPrefixedChat(
					ChatColour.CONDITION_FAILED,
					"You are currently a ",
					this.Glow(ChatColour.HIGHLIGHT_FAILED),
					currentJobName,
					this.Glow(ChatColour.RESET),
					ChatColour.RESET
				);
			}
		}
	}
}