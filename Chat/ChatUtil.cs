﻿using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;

namespace TinyCmds.Chat {
	public static class ChatUtil {

		internal static List<Payload> generateMessagePayloads(params object[] payloads) {
			return new List<Payload>(payloads
				.Where(e => e is not null)
				.SelectMany(e => {
					return e switch {
						Payload pl => new Payload[] { pl },
						string st => new Payload[] { new TextPayload(st) },
						ChatColour ck => new Payload[] { new UIForegroundPayload((ushort)ck) },
						ChatGlow ck => new Payload[] { new UIGlowPayload((ushort)ck) },
						object o => new Payload[] { new TextPayload(o.ToString() ?? "") },
						_ => new Payload[] {
							new UIForegroundPayload((ushort)ChatColour.ERROR),
							new TextPayload($"[internal error in {TinyCmds.PluginName}]"),
							new UIForegroundPayload((ushort)ChatColour.RESET),
						},
					};
				})
			);
		}

		#region Chatlog functions
		public static void ShowMessage(params object[] payloads) {
			if (payloads.Length < 1 || !payloads.Where(e => e != null).Any())
				return;
			TinyCmds.chat.Print(new SeString(generateMessagePayloads(payloads)));
		}
		public static void ShowPrefixedMessage(params object[] payloads) {
			List<object> plList = new() {
				ChatColour.PREFIX,
				$"[{TinyCmds.Prefix}] ",
				ChatColour.RESET,
			};
			plList.AddRange(payloads);
			ShowMessage(plList.ToArray());
		}
		public static void ShowError(params object[] payloads) => TinyCmds.chat.PrintError(new SeString(generateMessagePayloads(payloads)));
		public static void ShowPrefixedError(params object[] payloads) {
			List<object> plList = new() {
				ChatColour.PREFIX,
				$"[{TinyCmds.Prefix}] ",
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
			// TODO add plugin logging
			if (displayInChatlog || dryRun)
				ShowPrefixedMessage(line, ChatColour.RESET);
			if (!dryRun)
				TinyCmds.common.Functions.Chat.SendMessage(line);
		}
	}
}