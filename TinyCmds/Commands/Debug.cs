﻿#if DEBUG
namespace TinyCmds.Commands;

using TinyCmds.Attributes;
using TinyCmds.Utils;

[Command("/tinydebug")]
[Summary("Do not touch")]
[HelpMessage("This command is specifically for dev use. It does whatever is needed at the moment, and is neither consistent nor stable.", "", "It also doesn't exist in non-debug builds.")]
[DoNotShowInHelp]
[HideInCommandListing]
public class Debug: PluginCommand {
	protected override void Execute(string? command, string argline, FlagMap flags, bool verbose, bool dryRun, ref bool showHelp) {
		// nop until I need to debug something lol
	}
}
#endif