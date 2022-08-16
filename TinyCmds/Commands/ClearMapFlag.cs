namespace TinyCmds.Commands;

using FFXIVClientStructs.FFXIV.Client.UI.Agent;

using TinyCmds.Attributes;
using TinyCmds.Utils;

[Command("/clearflag")]
[Summary("Remove the flag marker from your map")]
[Arguments()]
[HelpMessage(
	"This command is a conveniently macro-able way to remove your map's flag marker."
)]
[Aliases("/unflag")]
public class ClearMapFlag: PluginCommand {
	protected override unsafe void Execute(string? command, string argline, FlagMap flags, bool verbose, bool dryRun, ref bool showHelp)
		=> AgentMap.Instance()->IsFlagMarkerSet = 0;
}
