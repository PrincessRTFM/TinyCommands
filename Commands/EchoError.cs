
using TinyCmds.Attributes;
using TinyCmds.Chat;
using TinyCmds.Utils;

namespace TinyCmds {
	public static partial class PluginCommands {
		[Command("/echoerr")]
		[Summary("Like /echo, but to the error channel")]
		[Aliases("/echoerror", "/error", "/eerr", "/err", "/ee")]
		[HelpMessage(
			"Functionally identical to the built-in /echo command, except that the output text is sent to the \"error\" chat channel instead.",
			"Mostly useful with the conditional chat commands to allow, for instance, an emote macro to warn you when you use it wrong.",
			"If you use the -p flag, the error message will be prefixed as coming from this plugin, instead of being a bare message."
		)]
		public static void EchoToErrorChannel(string? command, string args, FlagMap flags, ref bool showHelp) {
			if (flags["p"])
				ChatUtil.ShowPrefixedError(args.Trim());
			else
				ChatUtil.ShowError(args.Trim());
		}
	}
}
