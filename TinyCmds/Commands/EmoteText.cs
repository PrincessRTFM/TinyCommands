using VariableVixen.TinyCmds.Attributes;
using VariableVixen.TinyCmds.Chat;
using VariableVixen.TinyCmds.Utils;

namespace VariableVixen.TinyCmds.Commands;

[Command("/emotetext", "/emotion", "/customemote", "/cemote", "/cem", "/ce")]
[Summary("Perform an emote without text, and simultaneously perform a custom-text emote to go with it")]
[HelpText(
	"This command condenses the given emote command (with the \"motion\" argument to silence it) and the \"/em\" command to simulate a custom emote macro in one command.",
	"For example, you can do \"/cem pat gives <t> some headpats\" to play the /pat animation with the custom text \"<Your Character> gives <Target Name> some headpats\" without needing a whole macro."
)]
public class EmoteText: PluginCommand {
	protected override void Execute(string? command, string rawArguments, FlagMap flags, bool verbose, bool dryRun, ref bool showHelp) {
		if (string.IsNullOrWhiteSpace(rawArguments) || !rawArguments.Contains(' ')) {
			showHelp = true;
			return;
		}
		string emoteName = rawArguments[..rawArguments.IndexOf(' ')].Replace("/", "").Trim();
		string actionText = rawArguments[rawArguments.IndexOf(' ')..].Trim();
		if (string.IsNullOrWhiteSpace(emoteName) || string.IsNullOrWhiteSpace(actionText)) {
			showHelp = true;
			return;
		}
		ChatUtil.SendChatlineToServer($"/{emoteName} motion", verbose, dryRun);
		ChatUtil.SendChatlineToServer($"/em {actionText}", verbose, dryRun);
	}
}
