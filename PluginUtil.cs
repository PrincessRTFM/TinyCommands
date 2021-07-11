using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

using Dalamud.Data;
using Dalamud.Game.Internal.Gui;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Plugin;

using XivCommon;

namespace TinyCmds {
	internal class PluginUtil {
		private readonly TinyCmdsPlugin plugin;
		private readonly XivCommonBase common;

		private static readonly Regex timespecMatcher = new(@"^\s*((?:\d+h)?)(\d+)([hm]?)\s*$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
		// From https://metacpan.org/dist/Text-ParseWords/source/lib/Text/ParseWords.pm - I really hope this works :/
		private static readonly Regex wordparser = new(@"^
			(?:
				# double quoted string
				""                                 # $quote
				((?>[^\\""]*(?:\\.[^\\""] *) *))""   # $quoted
			|   # --OR--
				# unquoted string
				(                                    # $unquoted
					(?:\\.|[^\\""])*?
				)
				# followed by
				(
					$(?!\n)                          # EOL
				|   # --OR--
					\s+                              # delimiter
				|   # --OR--
					(?!^)(?="")                      # a quote
				)
			)
		", RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline);

		#region Passthrough plugin properties
		private DalamudPluginInterface Interface => this.plugin.Interface;
		private DataManager Data => this.Interface.Data;
		private ChatGui Chat => this.Interface.Framework.Gui.Chat;
		#endregion

		public enum Colour: ushort {
			NONE = 0,
			WHITE = 1,
			GREY = 4,
			LAVENDER = 9,
			RED = 17,
			YELLOW = 25,
			LIGHTBLUE = 34,
			BLUE = 37,
			LIGHTGREEN = 43,
			GREEN = 45,
			PURPLE = 48,
			INDIGO = 49,
			TEAL = 52,
			BROWN = 54,
			CYAN = 57,
			BRIGHTGREEN = 60,
			ORANGE = 65,
			PALEGREEN = 67,
		}

		internal PluginUtil(TinyCmdsPlugin host) {
			this.plugin = host;
			this.common = new XivCommonBase(host.Interface, Hooks.None); // just need the chat feature to send commands
		}

		#region UI*Payload functions
		public UIForegroundPayload Foreground(ushort colorKey) {
			return new UIForegroundPayload(this.Data, colorKey);
		}
		public UIForegroundPayload Foreground(Colour color) {
			return new UIForegroundPayload(this.Data, (ushort)color);
		}
		public UIGlowPayload Glow(ushort colorKey) {
			return new UIGlowPayload(this.Data, colorKey);
		}
		public UIGlowPayload Glow(Colour color) {
			return new UIGlowPayload(this.Data, (ushort)color);
		}
		#endregion

		#region Chatlog functions
		public void SendDirectChat(params object[] payloads) {
			if (payloads.Length < 1 || !payloads.Where(e => e != null).Any())
				return;
			this.Chat.PrintChat(new XivChatEntry() {
				MessageBytes = new SeString(new List<Payload>(payloads
					.Where(e => e != null)
					.SelectMany(e => {
						return e switch {
							Payload pl => new Payload[] { pl },
							string st => new Payload[] { new TextPayload(st) },
							Colour ck => new Payload[] { this.Foreground(ck) },
							object o => new Payload[] { new TextPayload(o.ToString()) },
							_ => new Payload[] {
								this.Foreground(Colour.RED),
								new TextPayload($"[internal error in {this.plugin.Name}]"),
								this.Foreground(Colour.NONE),
							},
						};
					})
				)).Encode()
			});
		}
		public void SendPrefixedChat(params object[] payloads) {
			List<object> plList = new() {
				Colour.LAVENDER,
				$"[{this.plugin.Prefix}] ",
				Colour.NONE,
			};
			plList.AddRange(payloads);
			this.SendDirectChat(plList.ToArray());
		}
		public void SendChatError(string error) {
			this.SendPrefixedChat(Colour.RED, error, Colour.NONE);
		}
		[Conditional("DEBUG")]
		public void Debug(string message, params object[] args) {
			this.SendPrefixedChat(Colour.GREY, string.Format(message, args), Colour.NONE);
		}
		#endregion

		public void SendServerChat(string line) {
			// TODO add plugin logging
			this.common.Functions.Chat.SendMessage(line);
		}

		#region Argument parsing
		public static string[] ShellParse(string input) {
			if (input.Contains('"')) {
				// gotta do it the hard way :/
				List<string> words = new();
				Match match = wordparser.Match(input);
				while (match.Success && match.Value.Length > 0) {
					string word = match.Groups[2].Length > 0 ? match.Groups[2].Value.Replace("\\\"", "\"").Replace("\\\\", "\\") : match.Groups[1].Value;
					if (word.Length > 0)
						words.Add(word);
					input = input.Substring(match.Value.Length);
					match = wordparser.Match(input);
				}
				return words.ToArray();
			}
			// No quotes, easy out
			return string.IsNullOrWhiteSpace(input) ? (new string[0]) : input.Split();
		}
		public static (FlagMap, string[]) ParseArguments(string argline) {
			FlagMap flags = new();
			string[] args = ShellParse(argline);
			int i;
			for (i = 0; i < args.Length; i++) {
				string next = args[i];
				if (!next.StartsWith("-")) {
					// not a flag argument
					break;
				}
				if (next.Equals("--")) {
					// end-of-flags argument
					i++;
					break;
				}
				flags.SetAll(next.TrimStart('-').ToCharArray());
			}
			string[] remaining;
			if (i < args.Length) {
				remaining = new string[args.Length - i];
				Array.Copy(args, i, remaining, 0, remaining.Length);
			}
			else {
				remaining = new string[0];
			}
			return (flags, remaining);
		}
		#endregion

		public static (int, int) ParseTimespecString(string spec) {
			int hours = 0;
			int minutes = 0;
			if (string.IsNullOrEmpty(spec)) {
				return (hours, minutes);
			}
			Match match = timespecMatcher.Match(spec);
			if (match.Success) {
				string
					a = match.Groups[1]?.Captures[0]?.Value?.ToLower() ?? "",
					b = match.Groups[2]?.Captures[0]?.Value?.ToLower() ?? "",
					c = match.Groups[3]?.Captures[0]?.Value?.ToLower() ?? "";
				if (a.Length > 0) {
					// Hours and minutes BOTH
					hours = int.Parse(a.TrimEnd('h'));
					minutes = int.Parse(b);
				}
				else if (c.Equals("h")) {
					// Hours ONLY
					hours = int.Parse(b);
				}
				else {
					// Minutes ONLY
					minutes = int.Parse(b);
				}
			}
			return (hours, minutes);
		}

		#region IDisposable Support
		protected virtual void Dispose(bool disposing) {
			if (!disposing) {
				return;
			}
			this.common.Dispose();
		}

		public void Dispose() {
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}
		#endregion
	}
}
