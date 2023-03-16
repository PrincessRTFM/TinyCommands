namespace PrincessRTFM.TinyCmds.Commands;

using FFXIVClientStructs.FFXIV.Client.UI.Agent;

using PrincessRTFM.TinyCmds.Attributes;
using PrincessRTFM.TinyCmds.Utils;

[Command("/clearflag", "/unflag")]
[Summary("Remove the flag marker from your map")]
[Arguments()]
[HelpText(
	"This command is a conveniently macro-able way to remove your map's flag marker."
)]
public class ClearMapFlag: PluginCommand {
	protected override unsafe void Execute(string? command, string argline, FlagMap flags, bool verbose, bool dryRun, ref bool showHelp) {
		AgentMap* map = AgentMap.Instance();
		Assert(map is not null, "failed to load AgentMap");
		map->IsFlagMarkerSet = 0;
	}
}
