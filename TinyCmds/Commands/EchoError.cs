using VariableVixen.TinyCmds.Attributes;
using VariableVixen.TinyCmds.Chat;
using VariableVixen.TinyCmds.Utils;

namespace VariableVixen.TinyCmds.Commands;

[Command("/echoerr", "/echoerror", "/error", "/eerr", "/err", "/ee")]
[Arguments("text to echo...")]
[Summary("Like /echo, but to the error channel")]
[HelpText(
	"This command is functionally identical to the built-in /echo command, except that the output text is sent to the \"error\" chat channel instead.",
	"Mostly useful with the conditional chat commands to allow, for instance, an emote macro to warn you when you use it wrong."
)]
public class EchoError: PluginCommand {
	protected override void Execute(string? command, string rawArguments, FlagMap flags, bool verbose, bool dryRun, ref bool showHelp)
		=> ChatUtil.ShowError(rawArguments.Trim());
}
