using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Plugin;

namespace TinyCmds {
	public partial class TinyCmdsPlugin: IDalamudPlugin {

		#region UI*Payload functions
		public UIForegroundPayload Foreground(ChatColour color) {
			return new UIForegroundPayload(this.Interface.Data, (ushort)color);
		}
		public UIGlowPayload Glow(ChatColour color) {
			return new UIGlowPayload(this.Interface.Data, (ushort)color);
		}
		#endregion

		#region Chat functions
		public void SendDirectChat(params object[] payloads) {
			if (payloads.Length < 1 || !payloads.Where(e => e != null).Any())
				return;
			this.Interface.Framework.Gui.Chat.PrintChat(new XivChatEntry() {
				MessageBytes = new SeString(new List<Payload>(payloads
					.Where(e => e != null)
					.SelectMany(e => {
						return e switch {
							Payload pl => new Payload[] { pl },
							string st => new Payload[] { new TextPayload(st) },
							ChatColour ck => new Payload[] { this.Foreground(ck) },
							object o => new Payload[] { new TextPayload(o.ToString()) },
							_ => new Payload[] {
								this.Foreground(ChatColour.ERROR),
								new TextPayload($"[internal error in {this.Name}]"),
								this.Foreground(ChatColour.RESET),
							},
						};
					})
				)).Encode()
			});
		}
		public void SendPrefixedChat(params object[] payloads) {
			List<object> plList = new() {
				ChatColour.PREFIX,
				$"[{this.Prefix}] ",
				ChatColour.RESET,
			};
			plList.AddRange(payloads);
			this.SendDirectChat(plList.ToArray());
		}
		public void SendChatError(string error) {
			this.SendPrefixedChat(ChatColour.ERROR, error, ChatColour.RESET);
		}

		public void SendServerChat(string line, bool displayInChatlog = false, bool dryRun = false) {
			// TODO add plugin logging
			if (displayInChatlog)
				this.SendPrefixedChat(ChatColour.OUTGOING_TEXT, line, ChatColour.RESET);
			if (!dryRun)
				this.common.Functions.Chat.SendMessage(line);
		}

		[Conditional("DEBUG")]
		public void Debug(string message, params object[] args) {
			this.SendPrefixedChat(
				ChatColour.QUIET,
				string.Format(message, args),
				ChatColour.RESET
			);
		}
		#endregion
	}
}