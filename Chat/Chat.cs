using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Plugin;

using TinyCmds.Chat;

namespace TinyCmds {
	public partial class TinyCmds: IDalamudPlugin {

		internal List<Payload> GenerateMessagePayloads(params object[] payloads) {
			return new List<Payload>(payloads
				.Where(e => e != null)
				.SelectMany(e => {
					return e switch {
						Payload pl => new Payload[] { pl },
						string st => new Payload[] { new TextPayload(st) },
						ChatColour ck => new Payload[] { new UIForegroundPayload(this.Interface.Data, (ushort)ck) },
						ChatGlow ck => new Payload[] { new UIGlowPayload(this.Interface.Data, (ushort)ck) },
						object o => new Payload[] { new TextPayload(o.ToString()) },
						_ => new Payload[] {
							new UIForegroundPayload(this.Interface.Data, (ushort)ChatColour.ERROR),
							new TextPayload($"[internal error in {this.Name}]"),
							new UIForegroundPayload(this.Interface.Data, (ushort)ChatColour.RESET),
						},
					};
				})
			);
		}

		#region Chatlog functions
		public void ShowChatMessage(params object[] payloads) {
			if (payloads.Length < 1 || !payloads.Where(e => e != null).Any())
				return;
			this.Interface.Framework.Gui.Chat.Print(new SeString(this.GenerateMessagePayloads(payloads)));
		}
		public void ShowPrefixedChatMessage(params object[] payloads) {
			List<object> plList = new() {
				ChatColour.PREFIX,
				$"[{this.Prefix}] ",
				ChatColour.RESET,
			};
			plList.AddRange(payloads);
			this.ShowChatMessage(plList.ToArray());
		}
		public void ShowChatError(params object[] payloads) {
			this.Interface.Framework.Gui.Chat.PrintError(new SeString(this.GenerateMessagePayloads(payloads)));
		}
		public void ShowPrefixedChatError(params object[] payloads) {
			List<object> plList = new() {
				ChatColour.PREFIX,
				$"[{this.Prefix}] ",
				ChatColour.RESET,
			};
			plList.AddRange(payloads);
			this.ShowChatError(plList.ToArray());
		}

		[Conditional("DEBUG")]
		public void Debug(string message, params object[] args) {
			this.ShowPrefixedChatMessage(
				ChatColour.QUIET,
				string.Format(message, args),
				ChatColour.RESET
			);
		}
		#endregion

		public void SendServerChat(string line, bool displayInChatlog = false, bool dryRun = false) {
			// TODO add plugin logging
			if (displayInChatlog || dryRun)
				this.ShowPrefixedChatMessage(ChatColour.OUTGOING_TEXT, line, ChatColour.RESET);
			if (!dryRun)
				this.Common.Functions.Chat.SendMessage(line);
		}
	}
}