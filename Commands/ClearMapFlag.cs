namespace TinyCmds;

using FFXIVClientStructs.FFXIV.Client.UI.Agent;

using TinyCmds.Attributes;
using TinyCmds.Utils;

public static partial class PluginCommands {
	[Command("/clearflag")]
	[Summary("Remove the flag marker from your map")]
	[Arguments()]
	[HelpMessage(
		"This command is a conveniently macro-able way to remove your map's flag marker."
	)]
	[Aliases("/unflag")]
	public static unsafe void ClearMapFlag(string? command, string argline, FlagMap flags, ref bool showHelp)
		=> AgentMap.Instance()->IsFlagMarkerSet = 0;
}
