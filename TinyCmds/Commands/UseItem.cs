using System.Collections.Generic;
using System.Linq;

using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Plugin.Services;
using Dalamud.Utility.Signatures;

using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;

using Lumina.Excel.Sheets;

using PrincessRTFM.TinyCmds.Attributes;
using PrincessRTFM.TinyCmds.Chat;
using PrincessRTFM.TinyCmds.Utils;

using CSFW = FFXIVClientStructs.FFXIV.Client.System.Framework.Framework;

namespace PrincessRTFM.TinyCmds.Commands;

[Command("/useitem", "/use", "/item")]
[Summary("Use an item from your inventory by numeric ID or by name (case-insensitive full match)")]
[HelpText(
	"This command attempts to use an item from your inventory, including key items. It is best used with the item ID, but can search by name as well.",
	"Name matching is case-insensitive, but NOT partial - you must pass the FULL name of the item you wish to use."
)]
public unsafe class UseItem: PluginCommand {
	private static readonly Dictionary<uint, string> usables;
	private uint retryItem = 0;

#pragma warning disable IDE0044 // Add readonly modifier
#pragma warning disable CS0649 // Member is never assigned to
	[Signature("E8 ?? ?? ?? ?? E9 ?? ?? ?? ?? 48 8D 0D ?? ?? ?? ?? E8 ?? ?? ?? ?? 4C 89 74 24 ??", Fallibility = Fallibility.Fallible)]
	private static delegate* unmanaged<nint, uint, uint, uint, short, void> useItem;

	[Signature("E8 ?? ?? ?? ?? 44 8B 4B 2C", Fallibility = Fallibility.Fallible)]
	private static delegate* unmanaged<uint, uint, uint> getActionID;
#pragma warning restore CS0649 // Member is never assigned to
#pragma warning restore IDE0044 // Add readonly modifier

	static UseItem() {
		usables = Plugin.Data.GetExcelSheet<Item>()!
			.Where(i => i.ItemAction.RowId > 0)
			.ToDictionary(i => i.RowId, i => i.Name.ToString().ToLower())
			.Concat(Plugin.Data.GetExcelSheet<EventItem>()!
				.Where(i => i.Action.RowId > 0)
				.ToDictionary(i => i.RowId, i => i.Name.ToString().ToLower())
			)
			.ToDictionary(kv => kv.Key, kv => kv.Value);
	}

	protected override void Initialise() {
		Plugin.Interop.InitializeFromAttributes(this);
		Plugin.Framework.Update += this.attemptReuse;
	}

	protected override void Execute(string? command, string rawArguments, FlagMap flags, bool verbose, bool dryRun, ref bool showHelp) {
		string target = rawArguments.ToLower().Trim();
		if (!uint.TryParse(target, out uint itemId)) {
			if (usables is null || string.IsNullOrWhiteSpace(target))
				return;

			string name = target.Replace("\uE03C", ""); // remove the HQ symbol if it gets in there somehow
			bool hq = name != target;
			try {
				itemId = usables.First(i => i.Value == name).Key + (uint)(hq ? 1_000_000 : 0);
			}
			catch {
				// no such item found
				ChatUtil.ShowPrefixedError(
					"No such item ",
					new UIForegroundPayload(14),
					rawArguments.Trim(),
					new UIForegroundPayload(0),
					" could be found."
				);
				return;
			}
		}

		this.use(itemId);
	}

	private void use(uint id) {
		if (useItem is null) {
			Plugin.Log.Error($"{this.GetType().Name}.use(uint) called without useItem delegate");
			return;
		}

		if (id == 0 || !usables.ContainsKey(id is >= 1_000_000 and < 2_000_000 ? id - 1_000_000 : id)) // yeah, HQ items are different IDs
			return;

		CSFW* framework = CSFW.Instance();
		if (framework is null)
			return;

		UIModule* uiModule = framework->GetUIModule();
		if (uiModule is null)
			return;

		AgentModule* agentModule = uiModule->GetAgentModule();
		if (agentModule is null)
			return;

		AgentInterface* itemContextMenuAgent = agentModule->GetAgentByInternalId(AgentId.InventoryContext);
		if (itemContextMenuAgent is null)
			return;


		// For some reason, items occasionally fail to actually be used, and this is how we can detect that
		// The ""fix"" is a horrible hack in which we basically just keep trying to use it every framework update :v
		if (getActionID is not null && this.retryItem == 0 && id < 2_000_000) {
			uint actionID = getActionID((uint)ActionType.Item, id);
			if (actionID == 0) {
				this.retryItem = id;
				return;
			}
		}

		useItem((nint)itemContextMenuAgent, id, 999, 0, 0);
	}
	private void attemptReuse(IFramework framework) {
		if (this.retryItem > 0) {
			this.use(this.retryItem);
			this.retryItem = 0;
		}
	}
	protected override void Dispose(bool disposing) {
		if (this.Disposed)
			return;
		this.Disposed = true;

		if (disposing)
			Plugin.Framework.Update -= this.attemptReuse;

		base.Dispose(disposing);
	}
}
