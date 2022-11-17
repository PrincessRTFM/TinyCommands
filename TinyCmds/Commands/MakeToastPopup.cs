namespace PrincessRTFM.TinyCmds.Commands;

using Dalamud.Game.Gui.Toast;

using PrincessRTFM.TinyCmds.Attributes;
using PrincessRTFM.TinyCmds.Utils;

[Command("/popup")]
[Arguments("type flag", "content")]
[Summary("Creates a popup (\"toast\") message on your screen, as if the game itself had done so")]
[Aliases("/toastmsg")]
[HelpMessage(
	"This command lets you create your own custom toast message popups, like the ones with sub-area names."
	+ " The toast \"type\" can be set by flag: -q for quests, -e for errors, and -n for \"normal\" toasts."
	+ " At least one type flag is required, and you can use more than one to show the same content in multiple toasts at once.",
	"",
	"There are also type-specific options for normal and quest toasts:",
	"- Normal toasts can use -s OR -f for slow (~4 seconds) or fast (~2)",
	"- Quest toasts can use -c and -p for checkmark (quest objective completion) and playing a sound (only if used with -c)"
)]
public class MakeToastPopup: PluginCommand {
	protected override unsafe void Execute(string? command, string argline, FlagMap flags, bool verbose, bool dryRun, ref bool showHelp) {
		if (flags['n'])
			Plugin.toast.ShowNormal(argline, flags['f'] || flags['s'] ? new ToastOptions() { Speed = flags['f'] ? ToastSpeed.Fast : ToastSpeed.Slow } : null!);

		if (flags['e'])
			Plugin.toast.ShowError(argline);

		if (flags['q']) {
			if (flags['i'] && argline.Contains(' ') && uint.TryParse(argline.Split(" ", 2)[0], out uint icon)) { // undocumented cause it's iffy at best
				Plugin.toast.ShowQuest(argline.Split(" ", 2)[1], new QuestToastOptions() { IconId = icon, PlaySound = flags['p'] });
			}
			else {
				Plugin.toast.ShowQuest(argline, new QuestToastOptions() { DisplayCheckmark = flags['c'], PlaySound = flags['p'] });
			}
		}

		if (!flags['n'] && !flags['e'] && !flags['q'])
			showHelp = true;
	}
}
