namespace TinyCmds.Commands;

using Dalamud.Logging;

using TinyCmds.Attributes;
using TinyCmds.Utils;


[Command("/tinycmds")]
[Arguments()]
[Summary("List all plugin commands")]
[Aliases("/tcmds")]
[HelpMessage(
	"This command will list all plugin commands in your chatlog. Use \"-a\" to include command aliases."
)]
public class ListPluginCommands: PluginCommand {
	protected override void Execute(string? command, string args, FlagMap flags, bool verbose, bool dryRun, ref bool showHelp) {
		if (this.Plugin is null)
			PluginLog.Error($"{this.InternalName}.Plugin is null");
		else if (this.Plugin.helpWindows["<LIST>"] is null)
			PluginLog.Error($"Command list window is null");
		else
			this.Plugin.helpWindows["<LIST>"].IsOpen = !flags['o'];
	}
}
