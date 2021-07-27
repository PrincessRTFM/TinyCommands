using Dalamud.Plugin;

using TinyCmds.Attributes;
using TinyCmds.Utils;

namespace TinyCmds {
	public partial class TinyCmds: IDalamudPlugin {
		[Command("/playsound")]
		[Summary("Plays one of the sixteen <se.##> sound effects")]
		[Aliases("/playsfx")]
		[HelpMessage(
			"This lets you play the <se.##> sound effects in your chat without needing to /echo them.",
			"It just helps keep things a little cleaner."
		)]
		public void PlayChatSound(string command, string args, FlagMap flags, ref bool showHelp) {
			if (!this.SoundEffect.Valid) {
				this.ShowPrefixedChatError("Unable to play sounds, the relevant game function couldn't be located");
				return;
			}
			if (!int.TryParse(args, out int idx)) {
				this.ShowPrefixedChatError("Invalid value, must provide a sound ID from 1-16, inclusive");
				return;
			}
			if (idx is < 1 or > 16) {
				this.ShowPrefixedChatError("Invalid sound ID, must be 1-16 inclusive");
				return;
			}
			this.SoundEffect.Play(SoundsExtensions.FromGameIndex(idx));
		}
	}
}