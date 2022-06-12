namespace TinyCmds;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Logging;

using Lumina.Excel;
using Lumina.Excel.GeneratedSheets;

using TinyCmds.Attributes;
using TinyCmds.Chat;
using TinyCmds.Utils;

using CSGO = FFXIVClientStructs.FFXIV.Client.Game.Object.GameObject;

public static partial class PluginCommands {
	[Command("/whereis")]
	[Arguments("partial name...")]
	[HelpMessage(
		"This command locates a game object (player, npc, etc) by name in your current zone.",
		"It uses case-insensitive partial matching, so that something like 'alph' will match Alphinaud and also Alpha.",
		"All found objects are printed to chat with their full name and a location flag for where they were found.",
		"By default, they are sorted alphabetically, but you can use the -d flag to instead sort by distance from you.",
		"By default, sorts are ascending order (a-z or closest first). To reverse the sort order, pass the -i flag.",
		"If you pass the -f flag, the first result IN THE SORT ORDER USED will be flagged on your map automatically."
	)]
	[Aliases("/locate")]
	public static unsafe void LocateGameObjectCommand(string? command, string argline, FlagMap flags, ref bool showHelp) {
		if (string.IsNullOrWhiteSpace(argline)) {
			ChatUtil.ShowPrefixedError("You need to provide a (partial) name filter.");
			showHelp = true;
			return;
		}

		string needle = argline.Trim().ToLower();
		Vector3 here = Plugin.client.LocalPlayer!.Position;
		IEnumerable<(string name, Vector3 position, float distance)> found = Plugin.objects
			.Where(
				o => o.IsValid()
				//&& o.ObjectId != GameObject.InvalidGameObjectId
				&& o.ObjectKind is ObjectKind.BattleNpc or ObjectKind.Player or ObjectKind.EventNpc or ObjectKind.EventObj or ObjectKind.Companion
				&& ((CSGO*)o.Address)->GetIsTargetable()
				&& o.Name.TextValue.Trim().ToLower().Contains(needle)
			)
			.Select(o => (o.Name.TextValue.Trim(), o.Position, (o.Position - here).Length()));

		if (!found.Any()) {
			ChatUtil.ShowPrefixedError(ChatColour.HIGHLIGHT_FAILED, "No results found for ", ChatColour.RESET, ChatColour.CONDITION_FAILED, argline.Trim(), ChatColour.RESET);
			return;
		}

		if (flags['d']) {
			found = found.OrderBy(o => o.distance);
		}
		else {
			found = found.OrderBy(o => o.name.ToLower());
		}
		if (flags['i'])
			found = found.Reverse();

		uint zone = Plugin.client.TerritoryType;
		Map map = Plugin.data.GetExcelSheet<TerritoryType>()?.GetRow(zone)?.Map?.Value ?? throw new NullReferenceException("Cannot find map ID");
		ExcelSheet<TerritoryTypeTransient>? transientSheet = Plugin.data.Excel.GetSheet<TerritoryTypeTransient>();
		short zOffset = transientSheet?.GetRow(map.TerritoryType.Row)?.OffsetZ ?? 0;
		uint mapId = map.RowId;
		float scale = (float)(map.SizeFactor / 100.0f);
		int count = found.Count();
		if (zOffset == -10000)
			zOffset = 0;

		SeStringBuilder msg = new SeStringBuilder()
			.AddUiForeground((ushort)ChatColour.PREFIX)
			.AddText($"[{Plugin.Prefix}] ")
			.AddUiForegroundOff()
			.AddUiForeground((ushort)ChatColour.CONDITION_PASSED)
			.AddText("Found ")
			.AddUiForegroundOff()
			.AddUiForeground((ushort)ChatColour.HIGHLIGHT_PASSED)
			.AddText(count.ToString())
			.AddUiForegroundOff()
			.AddUiForeground((ushort)ChatColour.CONDITION_PASSED)
			.AddText($" entit{(count == 1 ? "y" : "ies")} matching ")
			.AddUiForegroundOff()
			.AddUiForeground((ushort)ChatColour.HIGHLIGHT_PASSED)
			.AddText(argline.Trim())
			.AddUiForegroundOff();

		foreach ((string name, Vector3 position, float distance) in found) {
			Vector2 mapped = Plugin.worldToMap(position, map.SizeFactor, map.OffsetX, map.OffsetY);
			msg = msg
				.AddText("\n")
				.AddUiForeground((ushort)ChatColour.HIGHLIGHT_PASSED)
				.AddText(name)
				.AddUiForegroundOff()
				.AddUiForeground((ushort)ChatColour.QUIET)
				.AddText(" (")
				.AddUiForegroundOff()
				.AddUiForeground((ushort)ChatColour.HIGHLIGHT)
				.AddText(distance.ToString("0.0"))
				.AddUiForegroundOff()
				.AddUiForeground((ushort)ChatColour.QUIET)
				.AddText("y): ")
				.AddUiForegroundOff()
				// Taken from https://github.com/Ottermandias/GatherBuddy/blob/39a7947d47bd4b2f266715b6bbe3cf4b3b3af298/GatherBuddy/Plugin/Communicator.cs#L34
				.AddUiForeground(0x0225)
				.AddUiGlow(0x0226)
				.Add(new MapLinkPayload(zone, mapId, mapped.X, mapped.Y))
				.AddUiForeground(500)
				.AddUiGlow(501)
				.AddText($"{(char)SeIconChar.LinkMarker}")
				.AddUiGlowOff()
				.AddUiForegroundOff()
				.AddText($"({mapped.X:0.0}, {mapped.Y:0.0})")
				.Add(RawPayload.LinkTerminator)
				.AddUiGlowOff()
				.AddUiForegroundOff();
		}

		SeString built = msg.AddUiForegroundOff().BuiltString;
#if DEBUG
		PluginLog.Information($"{built.Encode().LongLength} bytes:");
		PluginLog.Information(built.TextValue);
#endif
		Plugin.chat.Print(built);

		if (flags['f']) {
			Vector2 mapped = Plugin.worldToMap(found.First().position, map.SizeFactor, map.OffsetX, map.OffsetY);
			MapLinkPayload pl = new(zone, mapId, mapped.X, mapped.Y);
			Plugin.gui.OpenMapWithMapLink(pl);
		}
	}
}
