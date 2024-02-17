using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;

namespace PrincessRTFM.TinyCmds.Chat;

public static class ChatUtil {

	internal static List<Payload> GenerateMessagePayloads(params object[] payloads) {
		return new List<Payload>(payloads
			.Where(e => e is not null)
			.SelectMany(e => {
				return e switch {
					Payload pl => new Payload[] { pl },
					string st => new Payload[] { new TextPayload(st) },
					ChatColour ck => new Payload[] { new UIForegroundPayload((ushort)ck) },
					ChatGlow ck => new Payload[] { new UIGlowPayload((ushort)ck) },
					SeString se => se.Payloads.ToArray(),
					IEnumerable<Payload> en => en.ToArray(),
					object o => new Payload[] { new TextPayload(o.ToString() ?? "") },
					_ => new Payload[] {
						new UIForegroundPayload((ushort)ChatColour.ERROR),
						new TextPayload($"[internal error in {Plugin.PluginName}]"),
						new UIForegroundPayload((ushort)ChatColour.RESET),
					},
				};
			})
		);
	}

	#region Chatlog functions
	public static void ShowMessage(params object[] payloads) {
		if (payloads.Length < 1 || !payloads.Where(e => e is not null).Any())
			return;
		Plugin.Chat.Print(new SeString(GenerateMessagePayloads(payloads)));
	}
	public static void ShowPrefixedMessage(params object[] payloads) {
		if (payloads.Length < 1 || !payloads.Where(e => e is not null).Any())
			return;
		List<object> plList = new() {
			ChatColour.PREFIX,
			$"[{Plugin.Prefix}] ",
			ChatColour.RESET,
		};
		plList.AddRange(payloads);
		ShowMessage(plList.ToArray());
	}
	public static void ShowError(params object[] payloads) {
		if (payloads.Length < 1 || !payloads.Where(e => e is not null).Any())
			return;
		Plugin.Chat.PrintError(new SeString(GenerateMessagePayloads(payloads)));
	}
	public static void ShowPrefixedError(params object[] payloads) {
		if (payloads.Length < 1 || !payloads.Where(e => e is not null).Any())
			return;
		List<object> plList = new() {
			ChatColour.PREFIX,
			$"[{Plugin.Prefix}] ",
			ChatColour.RESET,
		};
		plList.AddRange(payloads);
		ShowError(plList.ToArray());
	}

	[Conditional("DEBUG")]
	public static void Debug(string message, params object[] args) {
		ShowPrefixedMessage(
			string.Format(message, args),
			ChatColour.RESET
		);
	}
	#endregion

	public static void SendChatlineToServer(string line, bool displayInChatlog = false, bool dryRun = false) {
		string content = Plugin.Common.Functions.Chat.SanitiseText(line);
		Plugin.Log.Information("{0} [{1}]",
			dryRun ? "!>" : ">>",
			content);

		if (displayInChatlog || dryRun)
			ShowPrefixedMessage(ChatColour.DEBUG, content, ChatColour.RESET);
		if (!dryRun)
			Plugin.Common.Functions.Chat.SendMessage(content);
	}
}
