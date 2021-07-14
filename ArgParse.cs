using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using Dalamud.Plugin;

namespace TinyCmds {
	public partial class TinyCmdsPlugin: IDalamudPlugin {

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
		public static (FlagMap, string[]) ParseArguments(in string argline) {
			FlagMap flags = new();
			string[] args = ShellParse(argline);
			int i;
			for (i = 0; i < args.Length; ++i) {
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
		public static (FlagMap, string) ExtractFlags(in string argline) {
			FlagMap flags = new();
			char[] chars = argline.ToCharArray();
			bool inFlag = false;
			int idx;
			for (idx = 0; idx < chars.Length; ++idx) {
				char next = chars[idx];
				if (next == '-') {
					if (inFlag) {
						++idx;
						break;
					}
					inFlag = true;
					continue;
				}
				if (char.IsWhiteSpace(next)) {
					inFlag = false;
					continue;
				}
				if (inFlag)
					flags.Set(next);
				else
					break;
			}
			string remaining;
			try {
				remaining = argline.Substring(idx).Trim();
			}
			catch (ArgumentOutOfRangeException) {
				remaining = string.Empty;
			}
			return (flags, remaining);
		}

	}
}