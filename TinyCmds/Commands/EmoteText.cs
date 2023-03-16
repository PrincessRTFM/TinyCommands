namespace PrincessRTFM.TinyCmds.Commands;

using PrincessRTFM.TinyCmds.Attributes;
using PrincessRTFM.TinyCmds.Chat;
using PrincessRTFM.TinyCmds.Utils;

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
		string emote = Plugin.common.Functions.Chat.SanitiseText($"/{emoteName} motion");
		string action = Plugin.common.Functions.Chat.SanitiseText($"/em {actionText}");
		ChatUtil.SendChatlineToServer(emote, verbose, dryRun);
		ChatUtil.SendChatlineToServer(action, verbose, dryRun);
	}
}
