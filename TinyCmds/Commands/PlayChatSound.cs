using PrincessRTFM.TinyCmds.Attributes;
using PrincessRTFM.TinyCmds.Chat;
using PrincessRTFM.TinyCmds.Utils;

namespace PrincessRTFM.TinyCmds.Commands;

[Command("/playsound", "/playsfx")]
[Summary("Plays one of the sixteen <se.##> sound effects")]
[HelpText(
	"This command lets you play the <se.##> sound effects in your chat without needing to /echo them, in order to keep things a little cleaner."
)]
public class PlayChatSound: PluginCommand {
	protected override void Execute(string? command, string rawArguments, FlagMap flags, bool verbose, bool dryRun, ref bool showHelp) {
		if (!Plugin.Sfx.Valid) {
			ChatUtil.ShowPrefixedError("Unable to play sounds, the relevant game function couldn't be located");
			return;
		}
		if (!int.TryParse(rawArguments, out int idx)) {
			ChatUtil.ShowPrefixedError("Invalid value, must provide a sound ID from 1-16, inclusive");
			return;
		}
		if (idx is < 1 or > 16) {
			ChatUtil.ShowPrefixedError("Invalid sound ID, must be 1-16 inclusive");
			return;
		}
		Plugin.Sfx.Play(SoundsExtensions.FromGameIndex(idx));
	}
}
