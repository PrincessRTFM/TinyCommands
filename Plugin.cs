namespace TinyCmds;

using System;
using System.Numerics;

using Dalamud.Data;
using Dalamud.Game;
using Dalamud.Game.ClientState;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects;
using Dalamud.Game.ClientState.Party;
using Dalamud.Game.Command;
using Dalamud.Game.Gui;
using Dalamud.Game.Gui.Toast;
using Dalamud.IoC;
using Dalamud.Plugin;

using TinyCmds.Chat;
using TinyCmds.Internal;

using XivCommon;

public class Plugin: IDalamudPlugin {

	public const string PluginName = "TinyCommands";
	public const string Prefix = "TinyCmds";
	public string Name => PluginName;

	private bool disposed = false;

	[PluginService] internal static ChatGui chat { get; private set; } = null!;
	[PluginService] internal static GameGui gui { get; private set; } = null!;
	[PluginService] internal static ToastGui toast { get; private set; } = null!;
	[PluginService] internal static DalamudPluginInterface pluginInterface { get; private set; } = null!;
	[PluginService] internal static SigScanner scanner { get; private set; } = null!;
	[PluginService] internal static CommandManager cmdManager { get; private set; } = null!;
	[PluginService] internal static ClientState client { get; private set; } = null!;
	[PluginService] internal static Condition conditions { get; private set; } = null!;
	[PluginService] internal static TargetManager targets { get; private set; } = null!;
	[PluginService] internal static DataManager data { get; private set; } = null!;
	[PluginService] internal static PartyList party { get; private set; } = null!;
	[PluginService] internal static ObjectTable objects { get; private set; } = null!;
	internal static XivCommonBase common { get; private set; } = null!;
	internal static PluginCommandManager commandManager { get; private set; } = null!;
	internal static PlaySound sfx { get; private set; } = null!;

	public Plugin() {
		common = new(); // just need the chat feature to send commands
		sfx = new();
		commandManager = new() {
			ErrorHandler = ChatUtil.ShowPrefixedError
		};
		commandManager.addCommandHandlers();
	}

	internal static Vector2 worldToMap(Vector3 pos, ushort sizeFactor, short offsetX, short offsetY) {
		float scale = sizeFactor / 100f;
		float x = (10 - ((((pos.X + offsetX) * scale) + 1024f) * -0.2f / scale)) / 10f;
		float y = (10 - ((((pos.Z + offsetY) * scale) + 1024f) * -0.2f / scale)) / 10f;
		x = MathF.Round(x, 1, MidpointRounding.ToZero);
		y = MathF.Round(y, 1, MidpointRounding.ToZero);
		return new(x, y);
	}

	#region IDisposable Support
	protected virtual void Dispose(bool disposing) {
		if (this.disposed)
			return;
		this.disposed = true;

		if (disposing) {
			commandManager.Dispose();
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
