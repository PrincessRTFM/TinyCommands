using VariableVixen.TinyCmds.Attributes;
using VariableVixen.TinyCmds.Chat;
using VariableVixen.TinyCmds.Utils;

namespace VariableVixen.TinyCmds.Commands;

[Command("/all", "/ap", "/pa", "/duty")]
[Arguments("message to send...?")]
[Summary("Acts as either /alliance or /party, depending on whether you're in an alliance")]
[HelpText(
	"This command checks whether you are currently in an alliance in order to determine whether to simulate /alliance or /party.",
	"If any additional (non-flag) arguments are provided, they are passed through directly to the chosen command.",
	"This allows you to use this command to send a single message, as well as to change your active chat channel."
)]
public class AllianceOrPartyChat: PluginCommand {
	protected override void Execute(string? command, string rawArguments, FlagMap flags, bool verbose, bool dryRun, ref bool showHelp) {
		if (Plugin.Party.IsAlliance)
			ChatUtil.SendChatlineToServer($"/alliance {rawArguments}".Trim(), verbose, dryRun);
		else
			ChatUtil.SendChatlineToServer($"/party {rawArguments}".Trim(), verbose, dryRun);
	}
}
