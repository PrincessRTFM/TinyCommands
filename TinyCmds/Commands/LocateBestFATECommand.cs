namespace PrincessRTFM.TinyCmds.Commands;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

using Dalamud.Game.ClientState.Fates;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;

using FFXIVClientStructs.FFXIV.Client.UI.Agent;

using Lumina.Excel;
using Lumina.Excel.GeneratedSheets;

using PrincessRTFM.TinyCmds.Attributes;
using PrincessRTFM.TinyCmds.Chat;
using PrincessRTFM.TinyCmds.Utils;

using Fate = Dalamud.Game.ClientState.Fates.Fate;
using Map = Lumina.Excel.GeneratedSheets.Map;

[Command("/findfate", "/fate")]
[Summary("Find nearby objects, players, and NPCs by name")]
[Arguments("-f?", "max level?", "minimum time left?", "maximum completion?")]
[HelpText(
	"This command locates all FATEs in your current zone, filtered by your criteria (if possible), and sorted by distance to you."
	+ " It's intended for use in zones where you have flight and access to the ENTIRE zone, as it calculates only raw distances."
	+ " If any FATEs found meet ALL of your criteria, then only the matching FATEs will be displayed.",
	"",
	"FATE selection is a cumulative filtering process performed as follows:",
	"- If ANY of the FATEs in your current zone are at or below the maximum desired level, only those FATEs will be checked for the duration and progress thresholds."
	+ " Otherwise, the lowest level FATEs in the current zone will be checked instead.",
	"- Of the FATEs found by filtering on level, if any match your duration and progress thresholds, they will be returned immediately."
	+ " Otherwise, the time remaining and completion thresholds will be gradually relaxed (by -30s and +5% at a time) until any matching FATEs are found, which are then returned.",
	"",
	"When the list of matching FATEs is displayed, the level of the FATEs found will be included at the top of the list."
	+ " The time remaining and completion percentage are included for each FATE."
	+ " All of them will be sorted by distance from your position to the centre of the FATE, with the nearest being listed first.",
	"",
	"If the -f flag is provided, the first FATE on the list returned will be marked on your map for ease of travelling."
)]
public class LocateBestFATECommand: PluginCommand {
	internal static DalamudLinkPayload findFateByNamePayload = null!;
	protected override void Initialise() {
		base.Initialise();
		findFateByNamePayload ??= Plugin.pluginInterface.AddChatLinkHandler(Plugin.findFateByNamePayloadId, this.findFateByName);
	}
	private unsafe void findFateByName(uint linkId, SeString linkText) {
		if (linkId is not Plugin.findFateByNamePayloadId) { // ...how?
			ChatUtil.ShowPrefixedError($"Internal error: payload ID mismatch ({linkId} != {Plugin.findFateByNamePayloadId})");
			return;
		}
		string wanted = linkText.TextValue;
		foreach (Fate? fate in Plugin.fates) {
			if (fate?.Name.TextValue == wanted) {
				uint zone = Plugin.client.TerritoryType;
				Map map = Plugin.data.GetExcelSheet<TerritoryType>()?.GetRow(zone)?.Map?.Value ?? throw new NullReferenceException("Cannot find map ID");
				Vector2 mapped = Plugin.worldToMap(fate.Position, map);
				MapLinkPayload pl = new(zone, map.RowId, mapped.X, mapped.Y);
				Plugin.gui.OpenMapWithMapLink(pl);
				return;
			}
		}
		ChatUtil.ShowPrefixedError($"Can't find any FATE named {wanted} - it may have been completed or timed out.");
	}
	protected override unsafe void Execute(string? command, string rawArguments, FlagMap flags, bool verbose, bool dryRun, ref bool showHelp) {
		if (Plugin.fates.Length == 0) {
			ChatUtil.ShowPrefixedMessage(
				ChatColour.CONDITION_FAILED,
				"There are no FATEs in your current zone.",
				ChatColour.RESET
			);
			Plugin.toast.ShowError("No FATEs are available.");
			return;
		}
		Vector3 here = Plugin.client.LocalPlayer!.Position;
		byte maxLevel = (new byte[] { Plugin.client.LocalPlayer!.Level }).Concat(Plugin.party.Select(p => GameObject.IsValid(p.GameObject) ? p.Level : byte.MaxValue)).Min();
		uint minTime = 0;
		byte maxProgress = 100;
		string[] args = rawArguments.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
		if (args.Length >= 1) {
			string level = args[0];
			try {
				byte threshold = Math.Min(byte.Parse(level), (byte)90);
				if (threshold > 0)
					maxLevel = threshold;
			}
			catch {
				ChatUtil.ShowPrefixedError(ChatColour.CONDITION_FAILED, level, ChatColour.RESET, " is not a valid level.");
			}
		}
		if (args.Length >= 2) {
			minTime = TimeSpec.RawSeconds(args[1]);
		}
		if (args.Length >= 3) {
			string progress = args[2].TrimEnd('%');
			try {
				maxProgress = Math.Min(byte.Parse(progress), (byte)100);
			}
			catch {
				ChatUtil.ShowPrefixedError(ChatColour.CONDITION_FAILED, progress, ChatColour.RESET, " is not a valid progression amount.");
			}
		}
		uint originalTime = minTime;
		uint originalProgress = maxProgress;
		List<Fate> fates = new(Plugin.fates.Length);
		foreach (Fate? fate in Plugin.fates) {
			if (fate is not null)
				fates.Add(fate);
		}
		byte fateLevel = fates.Where(f => f.Level <= maxLevel).Any()
			? fates.OrderByDescending(f => f.Level).First().Level
			: fates.OrderBy(f => f.Level).First().Level;
		Fate[] filtered = fates.Where(f => f.Level == fateLevel).ToArray();
		IEnumerable<Fate> found = filtered.Where(f => f.State is FateState.Preparation || (f.TimeRemaining >= minTime && f.Progress <= maxProgress));
		bool adjusted = false;
		while (!found.Any()) { // nothing was found that matches, so we need to gradually relax the limits until SOMETHING comes up
			adjusted = true;
			minTime = minTime >= 30 ? minTime - 30 : 0;
			maxProgress = (byte)Math.Min(maxProgress + 5, 100);
			found = filtered.Where(f => f.State is FateState.Preparation || (f.TimeRemaining >= minTime && f.Progress <= maxProgress));
			if (minTime == 0 && maxProgress == 100) // just in case, to avoid infinite thrash loops
				break;
		}
		Fate[] accepted = found.OrderBy(f => Vector3.Distance(f.Position, here)).ToArray();
		if (accepted.Length == 0)
			throw new Exception("There exist FATEs in this zone, but none could be filtered, even permissively");
		string noun = "FATE" + (accepted.Length > 1 ? "s" : "");
		string verb = accepted.Length > 1 ? "were" : "was";
		List<object> payloads = new() {
			ChatColour.HIGHLIGHT,
			$"{accepted.Length} {noun}",
			ChatColour.RESET,
			$" {verb} found at ",
			fateLevel <= maxLevel ? ChatColour.CONDITION_PASSED : ChatColour.CONDITION_FAILED,
			$"level {fateLevel}",
			ChatColour.RESET,
		};
		if (adjusted) {
			payloads.AddRange(new object[] {
				ChatColour.HIGHLIGHT_FAILED,
				" outside your criteria",
				ChatColour.RESET,
			});
		}
		foreach (Fate fate in accepted) {
			payloads.AddRange(new object[] {
					"\n- ",
					ChatColour.HIGHLIGHT,
					findFateByNamePayload,
					fate.Name,
					RawPayload.LinkTerminator,
					ChatColour.RESET,
					" (",
			});
			if (fate.State is FateState.Preparation) {
				payloads.AddRange(new object[] {
					ChatColour.CONDITION_PASSED,
					"not yet triggered",
				});
			}
			else {
				payloads.AddRange(new object[] {
					fate.TimeRemaining <= originalTime ? ChatColour.HIGHLIGHT_FAILED : ChatColour.HIGHLIGHT_PASSED,
					TimeSpec.ClockDisplay(0, 0, (uint)Math.Min(fate.TimeRemaining, uint.MaxValue)),
					ChatColour.RESET,
					", ",
					fate.Progress >= originalProgress ? ChatColour.HIGHLIGHT_FAILED : ChatColour.HIGHLIGHT_PASSED,
					$"{fate.Progress}%",
				});
			}
			payloads.Add(ChatColour.RESET);
			payloads.Add(")");
		}
		ChatUtil.ShowPrefixedMessage(payloads.ToArray());
		if (flags['f']) {
			uint zone = Plugin.client.TerritoryType;
			Map map = Plugin.data.GetExcelSheet<TerritoryType>()?.GetRow(zone)?.Map?.Value ?? throw new NullReferenceException("Cannot find map ID");
			ExcelSheet<TerritoryTypeTransient>? transientSheet = Plugin.data.Excel.GetSheet<TerritoryTypeTransient>();
			uint mapId = map.RowId;
			AgentMap* agentMap = AgentMap.Instance();
			Vector3 pos = accepted[0].Position;
			agentMap->SetFlagMapMarker(zone, mapId, pos);
		}
	}
}
