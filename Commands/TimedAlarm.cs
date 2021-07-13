using System;

using Dalamud.Plugin;

using TinyCmds.Attributes;

namespace TinyCmds {
	public partial class TinyCmdsPlugin: IDalamudPlugin {
		[Command("/timer")]
		[Arguments("delay", "name")]
		[Summary("Set an in-game alarm to go off AFTER a certain amount of time, instead of AT a given time")]
		[Aliases("/ptimer", "/delay", "/pdelay")]
		[HelpMessage(
			"The delay must be specified as \"??h??m\", where \"??\" is the number of hours/minutes to wait.",
			"If either number is zero, it and the following letter can be left off entirely.",
			"As an additional special case, if you are only setting a minute-level delay, you can leave out the \"m\" as well, as in \"10\" for ten minutes.",
			"This command sets a VANILLA in-game timer for the time calculated. If you use -v, the /alarm command will be printed. If you use -d, it will be printed and NOT run.",
			"If the timer name contains spaces, it MUST be enclosed in double quotes."
		)]
		public void GenerateTimedAlarm(string command, string[] args, FlagMap flags) {
			DateTime now = DateTime.Now;
			string timespec = args[0];
			string name = args[1];
			(int hours, int minutes) = PluginUtil.ParseTimespecString(timespec);
			if (hours < 1 && minutes < 1) {
				this.Util.SendChatError("A timer must delay for at least one minute");
				return;
			}
			int futureMinutes = now.Minute + minutes;
			if (futureMinutes >= 60) {
				hours += (int)Math.Floor((double)futureMinutes / 60);
				futureMinutes %= 60;
			}
			if (hours > 23) {
				this.Util.SendChatError("A timer's total delay cannot be more than 23:59");
				return;
			}
			int futureHours = (now.Hour + hours) % 24;
			string cmd = $"/alarm \"{name}\" lt {futureHours:D2}{futureMinutes:D2}";
			this.Util.SendServerChat(cmd, flags["d"] || flags["v"], flags["d"]);
		}
	}
}
