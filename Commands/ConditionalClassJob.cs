using System.Linq;

using TinyCmds.Attributes;
using TinyCmds.Chat;
using TinyCmds.Utils;

namespace TinyCmds {
	public static partial class PluginCommands {
		[Command("/ifjob")]
		[Arguments("'-n'?", "jobs to match against", "command to run...?")]
		[Summary("Run a chat command (or directly send a message) only when playing certain classes/jobs")]
		[Aliases("/ifclass", "/whenjob", "/whenclass", "/job", "/class")]
		[HelpMessage(
			"Much like /ifcmd and /ifgp, this command executes a given command when the condition is met.",
			"In this case, the condition is whether or not your current class/job is one of the given set.",
			"Use the three-letter abbreviation, and if you want to check against more than one, separate them with commas but NOT spaces.",
			"If you pass the -n flag, the match will be inverted so the command runs only when you AREN'T one of those jobs."
		)]
		public static void RunIfJobMatches(string? command, string args, FlagMap flags, ref bool showHelp) {
			if (TinyCmds.client.LocalPlayer is null) {
				ChatUtil.ShowPrefixedError("Can't find player object - this should be impossible unless you're not logged in.");
				return;
			}
			string arg = args ?? string.Empty;
			string currentJobName = TinyCmds.client.LocalPlayer.ClassJob.GameData!.Abbreviation.ToString().ToUpper();
			string wantedJobNames = arg.Split()[0].ToUpper();
			if (string.IsNullOrEmpty(wantedJobNames)) {
				showHelp = true;
				return;
			}
			string cmd = arg.Contains(' ')
				? arg[(wantedJobNames.Length + 1)..]
				: string.Empty;
			bool invert = flags["n"];
			bool match = wantedJobNames.Split(',').Contains(currentJobName);
			if (match ^ invert) {
				if (cmd.Length > 0) {
					ChatUtil.SendChatlineToServer(cmd, flags["d"] || flags['v'], flags['d']);
				}
				else {
					ChatUtil.ShowPrefixedMessage(
						ChatColour.CONDITION_PASSED,
						"You are currently a ",
						ChatGlow.CONDITION_PASSED,
						currentJobName,
						ChatGlow.RESET,
						ChatColour.RESET
					);
				}
			}
			else if (cmd.Length < 1) {
				ChatUtil.ShowPrefixedMessage(
					ChatColour.CONDITION_FAILED,
					"You are currently a ",
					ChatGlow.CONDITION_FAILED,
					currentJobName,
					ChatGlow.RESET,
					ChatColour.RESET
				);
			}
		}
	}
}