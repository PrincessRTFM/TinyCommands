using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace PrincessRTFM.TinyCmds.Utils;

public class ArgumentParser {

	// From https://metacpan.org/dist/Text-ParseWords/source/lib/Text/ParseWords.pm - I really hope this works :/
	[System.Diagnostics.CodeAnalysis.SuppressMessage("GeneratedRegex", "SYSLIB1045:Convert to 'GeneratedRegexAttribute'.", Justification = "Way too big and long to be one-lined for the attribute")]
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
				input = input[match.Value.Length..];
				match = wordparser.Match(input);
			}
			return words.ToArray();
		}
		// No quotes, easy out
		return string.IsNullOrWhiteSpace(input) ? Array.Empty<string>() : input.Split();
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
			remaining = argline[idx..].Trim();
		}
		catch (ArgumentOutOfRangeException) {
			remaining = string.Empty;
		}
		return (flags, remaining);
	}

}
