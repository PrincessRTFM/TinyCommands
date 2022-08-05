namespace TinyCmds.Commands;

using System;
using System.Diagnostics;

using Dalamud.Memory;

using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.FFXIV.Client.System.Framework;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;

using TinyCmds.Attributes;
using TinyCmds.Chat;
using TinyCmds.Internal;
using TinyCmds.Utils;

[Command("/execute")]
[Summary("Evaluate placeholders in the given command, then run that new command")]
[Aliases("/exec", "/eval", "/evaluate")]
[HelpMessage(
	"When provided with an input line, certain placeholders are evaluated, then the new input line is executed.",
	"If you use one of :@#$%^&*+ as a flag, only placeholders that start with that character will be replaced, as in <:target>.",
	"This is to allow passing actual placeholders in the event that a command has only partial support.",
	"All placeholders that identify players or NPCs are supported, as are <class>/<job>. No others are evaluated, due to a lack of consistent/sensible formatting.",
	"There are a handful of non-vanilla placeholders that are also executed, which can only be used with this command.",
	"They are: year, month, month0, day, day0,weekday, yearday, yearday0, hour/hour24, hour12, min/minute, sec/second, ms/milli/millisec/millisecond/milliseconds, am/pm.",
	"Month, day of month, and day of year can take the suffix \"0\" to be zero-based.",
	"These placeholders can be suffixed with \".utc\" to use UTC/GMT instead of local time. Due to internal restrictions, \"<am>\" is lowercase, while \"<pm>\" is uppercase.",
	"As with all commands that send input to the server, -? will display the command before it's run, and -! will display the command without running it."
)]
public class EvaluatePlaceholders: PluginCommand {
	private readonly string[] leaders = new string[] {
		":",
		"@",
		"#",
		"$",
		"%",
		"^",
		"&",
		"*",
		"+",
	};

	private static unsafe string readObjName(GameObject* obj)
		=> obj is null ? "" : MemoryHelper.ReadStringNullTerminated(new IntPtr(obj->Name));

	private static unsafe string replace(string p)
		=> p.Trim().ToLower() switch {
			// Custom placeholders - time - local
			"year" => DateTime.Now.Year.ToString(),
			"month" => (DateTime.Now.Month + 1).ToString("D2"),
			"month0" => DateTime.Now.Month.ToString("D2"),
			"day" => (DateTime.Now.Day + 1).ToString("D2"),
			"day0" => DateTime.Now.Day.ToString("D2"),
			"weekday" => DateTime.Now.DayOfWeek.ToString(),
			"yearday" => (DateTime.Now.DayOfYear + 1).ToString("D3"),
			"yearday0" => DateTime.Now.DayOfYear.ToString("D3"),
			"hour" or "hour24" => DateTime.Now.Hour.ToString("D2"),
			"hour12" => ((DateTime.Now.Hour % 12) + 1).ToString("D2"),
			"min" or "minute" => DateTime.Now.Minute.ToString("D2"),
			"sec" or "second" => DateTime.Now.Second.ToString("D2"),
			"ms" or "milli" or "millisec" or "millisecond" or "milliseconds" => DateTime.Now.Millisecond.ToString("D3"),
			"am" => DateTime.Now.Hour < 12 ? "am" : "pm",
			"pm" => DateTime.Now.Hour < 12 ? "AM" : "PM",
			// Custom placeholders - time - UTC
			"year.utc" => DateTime.UtcNow.Year.ToString(),
			"month.utc" => (DateTime.UtcNow.Month + 1).ToString("D2"),
			"month0.utc" => DateTime.UtcNow.Month.ToString("D2"),
			"day.utc" => (DateTime.UtcNow.Day + 1).ToString("D2"),
			"day0.utc" => DateTime.UtcNow.Day.ToString("D2"),
			"weekday.utc" => DateTime.UtcNow.DayOfWeek.ToString(),
			"yearday.utc" => (DateTime.UtcNow.DayOfYear + 1).ToString("D3"),
			"yearday0.utc" => DateTime.UtcNow.DayOfYear.ToString("D3"),
			"hour.utc" or "hour24.utc" => DateTime.UtcNow.Hour.ToString("D2"),
			"hour12.utc" => ((DateTime.UtcNow.Hour % 12) + 1).ToString("D2"),
			"min.utc" or "minute.utc" => DateTime.UtcNow.Minute.ToString("D2"),
			"sec.utc" or "second.utc" => DateTime.UtcNow.Second.ToString("D2"),
			"ms.utc" or "milli.utc" or "millisec.utc" or "millisecond.utc" or "milliseconds.utc" => DateTime.UtcNow.Millisecond.ToString("D3"),
			"am.utc" => DateTime.UtcNow.Hour < 12 ? "am" : "pm",
			"pm.utc" => DateTime.UtcNow.Hour < 12 ? "AM" : "PM",
			// Vanilla placeholders - targets
			"t" or "target"
			or "tt" or "t2t"
			or "me" or "0"
			or "r" or "reply" // TODO: confirm if this one works or not
			or "1" or "2" or "3" or "4" or "5" or "6" or "7" or "8"
			or "f" or "focus"
			or "lt" or "lasttarget"
			or "le" or "lastenemy"
			or "la" or "lastattacker"
			or "c" or "comp" or "b" or "buddy"
			or "pet"
			or "attack1" or "attack2" or "attack3" or "attack4" or "attack5"
			or "bind1" or "bind2" or "bind3"
			or "ignore1" or "ignore2"
			or "square" or "circle" or "cross" or "triangle"
			or "mo" or "mouse"
			or "e1" or "e2" or "e3" or "e4" or "e5" => readObjName(Framework.Instance()->UIModule->GetPronounModule()->ResolvePlaceholder($"<{p}>", 1, 0)),
			// Vanilla placeholders - other
			"class" or "job" => Plugin.client.LocalPlayer is null ? "" : Plugin.client.LocalPlayer.ClassJob.GameData!.Name.ToString() + "(" + Plugin.client.LocalPlayer.Level + ")",
			// Not a placeholder
			_ => p,
		};

	protected override unsafe void Execute(string? command, string rawArguments, FlagMap flags, bool verbose, bool dryRun, ref bool showHelp) {
		string lead = "";
		foreach (string leader in this.leaders) {
			if (flags[leader]) {
				lead = leader;
				break;
			}
		}

		Stopwatch timer = new();
		PronounModule* pronouns = Framework.Instance()->UIModule->GetPronounModule();
		string input = rawArguments;
		int pos1 = input.IndexOf('<');
		int pos2 = input.IndexOf('>');

		timer.Start();
		while (pos1 >= 0 && pos2 > pos1) {
			if (timer.ElapsedMilliseconds >= 1000) // If you manage to hardlock this loop somehow, at least it won't last forever. If a legit string exceeds this, something's wrong anyway.
				break;

			string pt = input[(pos1 + 1)..pos2];
			if (string.IsNullOrEmpty(lead) || pt.StartsWith(lead)) {
				string p = pt[lead.Length..];
				string r = replace(p);

				if (r != p)
					input = input[0..pos1] + r + input[(pos2 + 1)..];

				pos2 = pos1 + r.Length;
				Logger.info($"\"{p}\" -> \"{r}\": ({pos1}, {pos2}) {input}");
			}

			if (pos2 + 3 > input.Length)
				break;

			pos1 = input.IndexOf('<', pos2);
			pos2 = input.IndexOf('>', pos1 + 1);
			Logger.info($"Next scan resolved ({pos1}, {pos2})");
		}

		ChatUtil.SendChatlineToServer(input, verbose || dryRun, dryRun);
	}
}
