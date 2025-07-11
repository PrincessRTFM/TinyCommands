namespace VariableVixen.TinyCmds.Chat;

public enum ChatColourKey: ushort {
	NONE = 0,
	WHITE = 1,
	GREY = 4,
	LAVENDER = 9,
	RED = 17,
	YELLOW = 25,
	LIGHTBLUE = 34,
	BLUE = 37,
	LIGHTGREEN = 43,
	GREEN = 45,
	PURPLE = 48,
	INDIGO = 49,
	TEAL = 52,
	BROWN = 54,
	CYAN = 57,
	BRIGHTGREEN = 60,
	ORANGE = 65,
	PALEGREEN = 67,
}
public enum ChatColour: ushort {
	RESET = ChatColourKey.NONE,
	ERROR = ChatColourKey.RED,
	CONDITION_FAILED = ChatColourKey.ORANGE,
	CONDITION_PASSED = ChatColourKey.GREEN,
	HIGHLIGHT_FAILED = ChatColourKey.YELLOW,
	HIGHLIGHT_PASSED = ChatColourKey.BRIGHTGREEN,
	PREFIX = ChatColourKey.LAVENDER,
	OUTGOING_TEXT = ChatColourKey.TEAL,
	QUIET = ChatColourKey.GREY,
	HELP_TEXT = ChatColourKey.PALEGREEN,
	USAGE_TEXT = ChatColourKey.LIGHTBLUE,
	HIGHLIGHT = ChatColourKey.CYAN,
	DEBUG = ChatColourKey.BROWN,
	JOB = ChatColourKey.INDIGO,
	COMMAND = ChatColourKey.PURPLE,
}
public enum ChatGlow: ushort {
	RESET = ChatColourKey.NONE,
	CONDITION_FAILED = ChatColourKey.YELLOW,
	CONDITION_PASSED = ChatColourKey.BRIGHTGREEN,
}
