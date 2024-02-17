using PrincessRTFM.TinyCmds.Utils;

namespace PrincessRTFM.TinyCmds.Internal;

internal delegate ulong PlaySoundDelegate(int soundId, ulong unknown1, ulong unknown2);
internal class PlaySound: GameFunctionBase<PlaySoundDelegate> {
	internal PlaySound() : base("E8 ?? ?? ?? ?? 4D 39 BE") { }
	internal void Play(Sound sound) {
		if (this.Valid && sound.IsSound()) {
			Logger.Verbose($"Playing sound {sound.ToSoundName()}");
			this.Invoke((int)sound, 0ul, 0ul);
		}
		else if (this.Valid) {
			Logger.Warning($"Something tried to play an invalid sound effect");
		}
		else {
			Logger.Warning("Unable to play sounds, game function couldn't be located");
		}
	}
}
