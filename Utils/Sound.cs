namespace TinyCmds.Utils {
	public enum Sound: byte {
		None    = 0x00,
		Unknown = 0x01,
		Sound01 = 0x25,
		Sound02 = 0x26,
		Sound03 = 0x27,
		Sound04 = 0x28,
		Sound05 = 0x29,
		Sound06 = 0x2A,
		Sound07 = 0x2B,
		Sound08 = 0x2C,
		Sound09 = 0x2D,
		Sound10 = 0x2E,
		Sound11 = 0x2F,
		Sound12 = 0x30,
		Sound13 = 0x31,
		Sound14 = 0x32,
		Sound15 = 0x33,
		Sound16 = 0x34,
	}

	public static class SoundsExtensions {
		public static string? ToGameString(this Sound value) {
			if (value > Sound.Unknown)
				return $"<se.{value.ToGameIndex()}>";
			return null;
		}
		public static string? ToSoundName(this Sound value) {
			if (value > Sound.Unknown)
				return $"SFX#{value.ToGameIndex()}";
			return null;
		}
		public static bool IsValid(this Sound value) => value.ToGameIndex() >= 0;
		public static bool IsSound(this Sound value) => value.ToGameIndex() > 0;
		public static int ToGameIndex(this Sound value) {
			return value switch {
				Sound.None => 0,
				Sound.Sound01 => 1,
				Sound.Sound02 => 2,
				Sound.Sound03 => 3,
				Sound.Sound04 => 4,
				Sound.Sound05 => 5,
				Sound.Sound06 => 6,
				Sound.Sound07 => 7,
				Sound.Sound08 => 8,
				Sound.Sound09 => 9,
				Sound.Sound10 => 10,
				Sound.Sound11 => 11,
				Sound.Sound12 => 12,
				Sound.Sound13 => 13,
				Sound.Sound14 => 14,
				Sound.Sound15 => 15,
				Sound.Sound16 => 16,
				_ => -1,
			};
		}
		public static Sound FromGameIndex(int idx) {
			return idx switch {
				0 => Sound.None,
				1 => Sound.Sound01,
				2 => Sound.Sound02,
				3 => Sound.Sound03,
				4 => Sound.Sound04,
				5 => Sound.Sound05,
				6 => Sound.Sound06,
				7 => Sound.Sound07,
				8 => Sound.Sound08,
				9 => Sound.Sound09,
				10 => Sound.Sound10,
				11 => Sound.Sound11,
				12 => Sound.Sound12,
				13 => Sound.Sound13,
				14 => Sound.Sound14,
				15 => Sound.Sound15,
				16 => Sound.Sound16,
				_ => Sound.Unknown,
			};
		}
	}
}
