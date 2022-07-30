namespace TinyCmds.Commands;

using TinyCmds.Attributes;
using TinyCmds.Chat;
using TinyCmds.Utils;

[Command("/echoerr")]
[Summary("Like /echo, but to the error channel")]
[Aliases("/echoerror", "/error", "/eerr", "/err", "/ee")]
[HelpMessage(
	"Functionally identical to the built-in /echo command, except that the output text is sent to the \"error\" chat channel instead.",
	"Mostly useful with the conditional chat commands to allow, for instance, an emote macro to warn you when you use it wrong."
)]
public class EchoError: PluginCommand {
	protected override void Execute(string? command, string args, FlagMap flags, bool verbose, bool dryRun, ref bool showHelp)
		=> ChatUtil.ShowError(args.Trim());
}
