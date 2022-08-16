namespace TinyCmds.Commands.Conditional;

using Dalamud.Logging;

using TinyCmds.Chat;
using TinyCmds.Utils;

public abstract class BaseConditionalCommand: PluginCommand {
	protected override string ModifyHelpMessage(string original) => "All conditional commands will execute some given command when their test passes."
		+ "\nIf no command/message is given, the test will print the result to your chatlog."
		+ $"\n{original}"
		+ "\nAs with ALL conditional commands, you can pass -$ to halt the current macro if one is running.";

	protected abstract bool TryExecute(string? command, string rawArguments, FlagMap flags, bool verbose, bool dryRun, ref bool showHelp);

	protected override void Execute(string? command, string rawArguments, FlagMap flags, bool verbose, bool dryRun, ref bool showHelp) {
		if (Plugin.client.LocalPlayer is null) {
			PluginLog.Error("Can't find player object - this should be impossible unless you're not logged in.");
			return;
		}

		bool didConditionsPass = this.TryExecute(command, rawArguments, flags, verbose, dryRun, ref showHelp);

		if (didConditionsPass && !showHelp && flags['$']) {
			ChatUtil.SendChatlineToServer("/macrocancel", dryRun || verbose, dryRun);
		}
	}
}
