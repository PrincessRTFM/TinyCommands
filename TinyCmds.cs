using System;
using System.Linq;
using System.Reflection;

using Dalamud.Game;
using Dalamud.Game.ClientState;
using Dalamud.Game.Command;
using Dalamud.Game.Gui;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Game.ClientState.Conditions;

using TinyCmds.Attributes;
using TinyCmds.Internal;

using XivCommon;
using Dalamud.Game.ClientState.Objects;

namespace TinyCmds {
	public class TinyCmds: IDalamudPlugin {

		public const string PluginName = "TinyCommands";
		public const string Prefix = "TinyCmds";
		public string Name => PluginName;

		private bool disposed = false;

		[PluginService] internal static ChatGui chat { get; private set; } = null!;
		[PluginService] internal static DalamudPluginInterface pluginInterface { get; private set; } = null!;
		[PluginService] internal static SigScanner scanner { get; private set; } = null!;
		[PluginService] internal static CommandManager cmdManager { get; private set; } = null!;
		[PluginService] internal static ClientState client { get; private set; } = null!;
		[PluginService] internal static Condition conditions { get; private set; } = null!;
		[PluginService] internal static TargetManager targets { get; private set; } = null!;
		internal static XivCommonBase common { get; private set; } = null!;
		internal static PluginCommandDelegate? pluginHelpCommand { get; private set; } = null!;
		internal static PluginCommandManager commandManager { get; private set; } = null!;
		internal static PlaySound sfx { get; private set; } = null!;

		public TinyCmds() {
			pluginHelpCommand = Delegate.CreateDelegate(typeof(PluginCommandDelegate), null,
				typeof(PluginCommands)
					.GetMethods()
					.Where(m => m.GetCustomAttribute<PluginCommandHelpHandlerAttribute>() is not null)
					.First()
			) as PluginCommandDelegate;
			if (pluginHelpCommand is null)
				Logger.warning("No plugin command was flagged as the default help/usage text method");
			commandManager = new();
			common = new(); // just need the chat feature to send commands
			sfx = new();
		}

		#region IDisposable Support
		protected virtual void Dispose(bool disposing) {
			if (this.disposed) {
				return;
			}
			if (disposing) {
				commandManager.Dispose();
				pluginInterface.Dispose();
			}
			this.disposed = true;
		}
		public void Dispose() {
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}
		~TinyCmds() {
			this.Dispose(false);
		}
		#endregion
	}
}
