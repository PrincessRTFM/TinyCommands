namespace PrincessRTFM.TinyCmds.Commands.Conditional;

using Dalamud.Game.ClientState.Objects.SubKinds;

using PrincessRTFM.TinyCmds.Attributes;
using PrincessRTFM.TinyCmds.Chat;
using PrincessRTFM.TinyCmds.Utils;

[Command("/ifgp", "/gp", "/whengp")]
[Arguments("condition flag", "GP to compare?", "command to run...?")]
[Summary("Run a chat command (or directly send a message) only if GP meets condition")]
[HelpText(
	"This command's test uses numeric inequality conditions against your GP."
	+ " There are three possible tests: at least (-g), less than (-l), and a simple at capacity (-c)."
	+ " If using -g or -l, the first argument should be a number to compare against. If using -c, ALL arguments are the command to run when your GP passes the check."
)]
public class ConditionalGatherPoints: BaseConditionalCommand {
	protected override bool TryExecute(string? command, string args, FlagMap flags, bool verbose, bool dryRun, ref bool showHelp) {
		string arg = args ?? string.Empty;
		PlayerCharacter player = Plugin.client.LocalPlayer!;
		uint gp = player.CurrentGp;
		if (player.MaxGp < 1) { // presumably not a gathering job, or maybe they have none unlocked - is MaxGp >0 when DoL is unlocked but current job is different?
			ChatUtil.Debug("You have no GP");
			return false;
		}

		if (flags["c"]) {

			if (player.CurrentGp >= player.MaxGp) {
				if (arg.Length > 0) {
					ChatUtil.SendChatlineToServer(arg, dryRun || verbose, dryRun);
				}
				else {
					ChatUtil.ShowPrefixedMessage(
						ChatColour.CONDITION_PASSED,
						"GP is at capacity (",
						ChatGlow.CONDITION_PASSED,
						gp,
						ChatGlow.RESET,
						")",
						ChatColour.RESET
					);
				}

				return true;
			}

			if (arg.Length < 1) {
				ChatUtil.ShowPrefixedMessage(
					ChatColour.CONDITION_FAILED,
					"GP is below capacity (",
					ChatGlow.CONDITION_FAILED,
					gp,
					ChatGlow.RESET,
					")",
					ChatColour.RESET
				);
			}

			return false;
		}

		if (flags["g"] || flags["l"]) {

			if (arg.Length < 1) {
				ChatUtil.ShowPrefixedError("-g and -l both require a number to compare your current GP against");
				return false;
			}

			string num = arg.Split()[0];
			string cmd = arg[num.Length..].Trim();

			if (int.TryParse(num, out int compareTo)) {

				if (flags["g"]) {

					if (gp >= compareTo) {
						if (cmd.Length > 0) {
							ChatUtil.SendChatlineToServer(cmd, dryRun || verbose, dryRun);
						}
						else {
							ChatUtil.ShowPrefixedMessage(
								ChatColour.CONDITION_PASSED,
								$"GP is at least {compareTo} (",
								ChatGlow.CONDITION_PASSED,
								gp,
								ChatGlow.RESET,
								")",
								ChatColour.RESET
							);
						}

						return true;
					}

					if (cmd.Length < 1) {
						ChatUtil.ShowPrefixedMessage(
							ChatColour.CONDITION_FAILED,
							$"GP is below {compareTo} (",
							ChatGlow.CONDITION_FAILED,
							gp,
							ChatGlow.RESET,
							")",
							ChatColour.RESET
						);
					}

					return false;
				}

				if (flags["l"]) {
					if (gp < compareTo) {
						if (cmd.Length > 0) {
							ChatUtil.SendChatlineToServer(cmd, dryRun || verbose, dryRun);
						}
						else {
							ChatUtil.ShowPrefixedMessage(
								ChatColour.CONDITION_PASSED,
								$"GP is below {compareTo} (",
								ChatGlow.CONDITION_PASSED,
								gp,
								ChatGlow.RESET,
								")",
								ChatColour.RESET
							);
						}

						return true;
					}

					if (cmd.Length < 1) {
						ChatUtil.ShowPrefixedMessage(
							ChatColour.CONDITION_FAILED,
							$"GP is above {compareTo} (",
							ChatGlow.CONDITION_FAILED,
							gp,
							ChatGlow.RESET,
							")",
							ChatColour.RESET
						);
					}

					return false;
				}

			}

			ChatUtil.ShowPrefixedError($"Couldn't parse \"{num}\" as an integer");
			showHelp = true;
			return false;
		}

		ChatUtil.ShowPrefixedError("Expected one of -c, -g, or -l, but found none");
		showHelp = true;
		return false;
	}
}
