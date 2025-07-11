using System;

using VariableVixen.TinyCmds.Attributes;
using VariableVixen.TinyCmds.Chat;
using VariableVixen.TinyCmds.Utils;

namespace VariableVixen.TinyCmds.Commands;

[Command("/timer", "/delay")]
[Arguments("delay", "name")]
[Summary("Set an in-game alarm to go off AFTER a certain amount of time, instead of AT a given time")]
[HelpText(
	"This command sets an in-game alarm at a time calculated based on the given delay. Since game alarms are timed to the minute, there may be inaccuracies of up to one minute.",
	"",
	"The delay must be specified as \"??h??m\", where \"??\" is the number of hours/minutes to wait."
	+ " If either number is zero, it and the following letter can be left off entirely."
	+ " As an additional special case, if you are only setting a minute-level delay, you can leave out the \"m\" as well, as in \"10\" for ten minutes.",
	"",
	"As with all commands that send an input line to the server, the \"-?\" flag will display the generated /alarm command before it's run, and the \"-!\" flag will display the generated command without running it."
)]
public class TimedAlarm: PluginCommand {

	protected override void Execute(string? command, string rawArguments, FlagMap flags, bool verbose, bool dryRun, ref bool showHelp) {
		DateTime now = DateTime.Now;
		string timespec = rawArguments.Split()[0];
		string name = rawArguments[timespec.Length..].Trim();
		if (string.IsNullOrEmpty(timespec)) {
			showHelp = true;
			return;
		}
		if (!TimeSpec.TryParse(timespec, out uint hours, out uint minutes, out _)) {
			ChatUtil.ShowPrefixedMessage($"Invalid timespan: {timespec}");
			return;
		}
		if (hours < 1 && minutes < 1) {
			ChatUtil.ShowPrefixedError("A timer must delay for at least one minute.");
			return;
		}
		uint futureMinutes = (uint)now.Minute + minutes;
		if (futureMinutes >= 60) {
			hours += (uint)Math.Floor((double)futureMinutes / 60);
			futureMinutes %= 60;
		}
		if (hours > 23) {
			ChatUtil.ShowPrefixedError("A timer's total delay cannot be more than 23:59.");
			return;
		}
		uint futureHours = ((uint)now.Hour + hours) % 24;
		string cmd = $"/alarm \"{name}\" lt {futureHours:D2}{futureMinutes:D2}";
		ChatUtil.SendChatlineToServer(cmd, verbose, dryRun);
	}
}
