using Dalamud.Plugin;

using TinyCmds.Attributes;
using TinyCmds.Utils;

namespace TinyCmds {
	public partial class TinyCmds: IDalamudPlugin {
		[Command("/echoerr")]
		[Summary("Like /echo, but to the error channel")]
		[Aliases("/echoerror", "/error")]
		[HelpMessage(
			"Functionally identical to the built-in /echo command, except that the output text is sent to the \"error\" chat channel instead.",
			"Mostly useful with the conditional chat commands to allow, for instance, an emote macro to warn you when you use it wrong.",
			"If you use the -p flag, the error message will be prefixed as coming from this plugin, instead of being a bare message."
		)]
		public void EchoToErrorChannel(string command, string args, FlagMap flags, ref bool showHelp) {
			if (flags["p"])
				this.ShowPrefixedChatError(args.Trim());
			else
				this.ShowChatError(args.Trim());
		}
	}
}
