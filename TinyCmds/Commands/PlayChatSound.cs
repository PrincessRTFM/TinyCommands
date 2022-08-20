namespace TinyCmds.Commands;

using TinyCmds.Attributes;
using TinyCmds.Chat;
using TinyCmds.Utils;

[Command("/playsound")]
[Summary("Plays one of the sixteen <se.##> sound effects")]
[Aliases("/playsfx")]
[HelpMessage(
	"This command lets you play the <se.##> sound effects in your chat without needing to /echo them, in order to keep things a little cleaner."
)]
public class PlayChatSound: PluginCommand {
	protected override void Execute(string? command, string args, FlagMap flags, bool verbose, bool dryRun, ref bool showHelp) {
		if (!Plugin.sfx.Valid) {
			ChatUtil.ShowPrefixedError("Unable to play sounds, the relevant game function couldn't be located");
			return;
		}
		if (!int.TryParse(args, out int idx)) {
			ChatUtil.ShowPrefixedError("Invalid value, must provide a sound ID from 1-16, inclusive");
			return;
		}
		if (idx is < 1 or > 16) {
			ChatUtil.ShowPrefixedError("Invalid sound ID, must be 1-16 inclusive");
			return;
		}
		Plugin.sfx.play(SoundsExtensions.FromGameIndex(idx));
	}
}
