namespace TinyCmds;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.Text.SeStringHandling;

using Lumina.Excel.GeneratedSheets;

using TinyCmds.Attributes;
using TinyCmds.Chat;
using TinyCmds.Utils;

public static partial class PluginCommands {
	[Command("/whereis")]
	[Arguments("partial name...")]
	[HelpMessage(
		"This command locates a game object (player, npc, etc) by name in your current zone.",
		"It uses case-insensitive partial matching, so that something like 'alph' will match Alphinaud and also Alpha.",
		"All found objects are printed to chat with their full name and a location flag for where they were found.",
		"By default, they are sorted alphabetically, but you can use the -d flag to instead sort by ascending distance from you.",
		"To reverse the sort order (either one), pass the -i flag."
	)]
	[Aliases("/locate")]
	public static void LocateGameObjectCommand(string? command, string argline, FlagMap flags, ref bool showHelp) {
		if (string.IsNullOrWhiteSpace(argline)) {
			ChatUtil.ShowPrefixedError("You need to provide a (partial) name filter.");
			showHelp = true;
			return;
		}

		string needle = argline.Trim().ToLower();
		IEnumerable<(string name, Vector3 position)> found = Plugin.objects
			.Where(
				o => o.IsValid()
				&& o.ObjectId != GameObject.InvalidGameObjectId
				&& o.ObjectKind is ObjectKind.BattleNpc or ObjectKind.Player or ObjectKind.EventNpc or ObjectKind.EventObj or ObjectKind.Companion
				&& o.Name.TextValue.Trim().ToLower().Contains(needle)
			)
			.Select(o => (o.Name.TextValue.Trim(), o.Position));

		if (!found.Any()) {
			ChatUtil.ShowPrefixedError(ChatColour.HIGHLIGHT_FAILED, "No results found for ", ChatColour.CONDITION_FAILED, argline.Trim());
			return;
		}

		if (flags['d']) {
			Vector3 here = Plugin.client.LocalPlayer!.Position;
			found = found.OrderBy(o => (o.position - here).Length());
		}
		else {
			found = found.OrderBy(o => o.name.ToLower());
		}
		if (flags['i'])
			found = found.Reverse();

		uint zone = Plugin.client.TerritoryType;
		Map map = Plugin.data.GetExcelSheet<TerritoryType>()?.GetRow(zone)?.Map?.Value ?? throw new NullReferenceException("Cannot find map ID");
		uint mapId = map.RowId;
		Vector2 scale = new((float)(map.SizeFactor / 100.0));
		Vector2 offset = new(map.OffsetX, map.OffsetY);
		int count = found.Count();

		SeStringBuilder msg = new SeStringBuilder()
			.AddUiForeground((ushort)ChatColour.PREFIX)
			.AddText($"[{Plugin.Prefix}] ")
			.AddUiForeground((ushort)ChatColour.CONDITION_PASSED)
			.AddText("Found ")
			.AddUiForeground((ushort)ChatColour.HIGHLIGHT_PASSED)
			.AddText(count.ToString())
			.AddUiForeground((ushort)ChatColour.CONDITION_PASSED)
			.AddText($" entit{(count == 1 ? "y" : "ies")}:");

		foreach ((string name, Vector3 position) in found) {
			Vector2 flat = new(position.X, position.Y);
			Vector2 mapped = (new Vector2(10) - ((((flat - -offset) * scale) + new Vector2(1024f)) * -0.2f / scale)) / 10;
			msg = msg
				.AddText("\n")
				.AddUiForeground((ushort)ChatColour.HIGHLIGHT_PASSED)
				.AddText(name)
				.AddUiForeground((ushort)ChatColour.QUIET)
				.AddText(": ")
				.AddUiForeground((ushort)ChatColour.HIGHLIGHT)
				.AddText($"{mapped.X}, {mapped.Y}");
		}

		Plugin.chat.Print(msg.AddUiForegroundOff().BuiltString);
	}
}
