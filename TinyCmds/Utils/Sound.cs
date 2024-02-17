namespace PrincessRTFM.TinyCmds.Utils;

public enum Sound: byte {
	None = 0x00,
	Unknown = 0x01,
	SFX01 = 0x25,
	SFX02 = 0x26,
	SFX03 = 0x27,
	SFX04 = 0x28,
	SFX05 = 0x29,
	SFX06 = 0x2A,
	SFX07 = 0x2B,
	SFX08 = 0x2C,
	SFX09 = 0x2D,
	SFX10 = 0x2E,
	SFX11 = 0x2F,
	SFX12 = 0x30,
	SFX13 = 0x31,
	SFX14 = 0x32,
	SFX15 = 0x33,
	SFX16 = 0x34,
}

public static class SoundsExtensions {
	public static string? ToGameString(this Sound value) => value > Sound.Unknown ? $"<se.{value.ToGameIndex()}>" : null;
	public static string? ToSoundName(this Sound value) => value > Sound.Unknown ? $"SFX#{value.ToGameIndex()}" : null;
	public static bool IsValid(this Sound value) => value.ToGameIndex() >= 0;
	public static bool IsSound(this Sound value) => value.ToGameIndex() > 0;
	public static int ToGameIndex(this Sound value) {
		return value switch {
			Sound.None => 0,
			Sound.SFX01 => 1,
			Sound.SFX02 => 2,
			Sound.SFX03 => 3,
			Sound.SFX04 => 4,
			Sound.SFX05 => 5,
			Sound.SFX06 => 6,
			Sound.SFX07 => 7,
			Sound.SFX08 => 8,
			Sound.SFX09 => 9,
			Sound.SFX10 => 10,
			Sound.SFX11 => 11,
			Sound.SFX12 => 12,
			Sound.SFX13 => 13,
			Sound.SFX14 => 14,
			Sound.SFX15 => 15,
			Sound.SFX16 => 16,
			_ => -1,
		};
	}
	public static Sound FromGameIndex(int idx) {
		return idx switch {
			0 => Sound.None,
			1 => Sound.SFX01,
			2 => Sound.SFX02,
			3 => Sound.SFX03,
			4 => Sound.SFX04,
			5 => Sound.SFX05,
			6 => Sound.SFX06,
			7 => Sound.SFX07,
			8 => Sound.SFX08,
			9 => Sound.SFX09,
			10 => Sound.SFX10,
			11 => Sound.SFX11,
			12 => Sound.SFX12,
			13 => Sound.SFX13,
			14 => Sound.SFX14,
			15 => Sound.SFX15,
			16 => Sound.SFX16,
			_ => Sound.Unknown,
		};
	}
}
