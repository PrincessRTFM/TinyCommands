namespace PrincessRTFM.TinyCmds.Commands;

using Dalamud.Interface.Windowing;

using PrincessRTFM.TinyCmds.Attributes;
using PrincessRTFM.TinyCmds.Utils;


[Command("/tinycmds", "/tcmds")]
[Arguments()]
[Summary("List all plugin commands")]
[HelpText(
	"This command displays a list of all of this plugin's commands.",
	"",
	"You can also pass the \"-o\" flag to close all other help windows."
)]
public class ListPluginCommands: PluginCommand {
	protected override void Execute(string? command, string rawArguments, FlagMap flags, bool verbose, bool dryRun, ref bool showHelp) {
		Assert(this.Plugin is not null, "plugin is null, everything is broken");
		Assert(this.Plugin!.helpWindows["<LIST>"] is not null, "command list window doesn't exist");
		if (flags['o']) {
			foreach (Window wnd in this.Plugin!.helpWindows.Values)
				wnd.IsOpen = false;
		}
		this.Plugin!.helpWindows["<LIST>"].IsOpen = true;
	}
}
