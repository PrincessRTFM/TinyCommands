namespace PrincessRTFM.TinyCmds;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

using Dalamud.Game;
using Dalamud.Game.ClientState.Objects;
using Dalamud.Game.Gui;
using Dalamud.Game.Gui.Toast;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Interface.Windowing;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using Dalamud.Utility;

using Lumina.Excel.GeneratedSheets;

using PrincessRTFM.TinyCmds.Chat;
using PrincessRTFM.TinyCmds.Internal;
using PrincessRTFM.TinyCmds.Ui;

using XivCommon;

public class Plugin: IDalamudPlugin {
	internal const uint findFateByNamePayloadId = 1;

	public const string PluginName = "TinyCommands";
	public const string Prefix = "TinyCmds";
	public string Name => PluginName;

	private bool disposed = false;

	[PluginService] internal static ChatGui chat { get; private set; } = null!;
	[PluginService] internal static IGameGui gui { get; private set; } = null!;
	[PluginService] internal static ToastGui toast { get; private set; } = null!;
	[PluginService] internal static DalamudPluginInterface pluginInterface { get; private set; } = null!;
	[PluginService] internal static ISigScanner scanner { get; private set; } = null!;
	[PluginService] internal static ICommandManager cmdManager { get; private set; } = null!;
	[PluginService] internal static IClientState client { get; private set; } = null!;
	[PluginService] internal static Dalamud.Game.ClientState.Conditions.Condition conditions { get; private set; } = null!;
	[PluginService] internal static ITargetManager targets { get; private set; } = null!;
	[PluginService] internal static IDataManager data { get; private set; } = null!;
	[PluginService] internal static IPartyList party { get; private set; } = null!;
	[PluginService] internal static IObjectTable objects { get; private set; } = null!;
	[PluginService] internal static IFateTable fates { get; private set; } = null!;
	[PluginService] internal static Framework framework { get; private set; } = null!;
	internal static XivCommonBase common { get; private set; } = null!;
	internal static PluginCommandManager commandManager { get; private set; } = null!;
	internal static PlaySound sfx { get; private set; } = null!;

	private readonly WindowSystem windowSystem;
	internal readonly Dictionary<string, Window> helpWindows;

	public Plugin() {
		common = new(); // just need the chat feature to send commands
		sfx = new();
		commandManager = new(this) {
			ErrorHandler = ChatUtil.ShowPrefixedError
		};
		commandManager.addCommandHandlers();

		this.windowSystem = new(this.GetType().Namespace!);
		this.helpWindows = commandManager.commands.ToDictionary(cmd => cmd.CommandComparable, cmd => new HelpWindow(this, cmd) as Window);
		this.helpWindows.Add("<PLUGIN>", new HelpWindow(
			this,
			"Basics",
			this.Name,
			"the plugin itself",
			"Basic information about how commands work",
			$"{this.Name} uses a custom command parser that accepts single-character boolean flags starting with a hyphen."
			+ "These flags can be bundled into one argument, such that \"-va\" will set both the \"v\" and \"a\" flags, just like \"-av\" will.\n"
			+ "\n"
			+ "All commands accept \"-h\" to display their built-in help.\n"
			+ "\n"
			+ "To list all commands, you can use \"/tinycmds\", optionally with \"-a\" to also list their aliases."
			+ " Be aware that this list may be a little long. It's also not really sorted."
		));
		this.helpWindows.Add("<LIST>", new CommandListWindow(this));

		foreach (Window wnd in this.helpWindows.Values)
			this.windowSystem.AddWindow(wnd);

		pluginInterface.UiBuilder.Draw += this.windowSystem.Draw;
	}

	internal static Vector2 worldToMap(Vector3 pos, Map zone) => worldToMap(new Vector2(pos.X, pos.Z), zone);
	internal static Vector2 worldToMap(Vector2 pos, Map zone) {
		Vector2 raw = MapUtil.WorldToMap(pos, zone);
		return new((int)MathF.Round(raw.X * 10, 1) / 10f, (int)MathF.Round(raw.Y * 10, 1) / 10f);
	}
	internal static Vector2 mapToWorld(Vector2 pos, Map zone) {
		MapLinkPayload maplink = new(zone.TerritoryType.Value!.RowId, zone.RowId, pos.X, pos.Y);
		return new(maplink.RawX / 1000f, maplink.RawY / 1000f);
	}

	#region IDisposable Support
	protected virtual void Dispose(bool disposing) {
		if (this.disposed)
			return;
		this.disposed = true;

		if (disposing) {
			common.Dispose();
			commandManager.Dispose();
			pluginInterface.UiBuilder.Draw -= this.windowSystem.Draw;
			this.windowSystem.RemoveAllWindows();
			foreach (HelpWindow wnd in this.helpWindows.Values.Cast<HelpWindow>())
				wnd.Dispose();
		}

	}
	public void Dispose() {
		this.Dispose(true);
		GC.SuppressFinalize(this);
	}
	~Plugin() {
		this.Dispose(false);
	}
	#endregion
}
