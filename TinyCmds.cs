using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

using Dalamud.Plugin;

using TinyCmds.Attributes;
using TinyCmds.Internal;

using XivCommon;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// I cannot fucking WAIT for dalamud net5 so I can use the nullable analysis annotations.
namespace TinyCmds {
	[SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Plugin command methods are delegates")]
	public partial class TinyCmds: IDalamudPlugin {

		public string Name => "TinyCommands";
		public readonly string Prefix = "TinyCmds";
		public string PluginHelpCommand { get; private set; }

		private bool disposed = false;

		internal XivCommonBase Common { get; private set; }

		internal DalamudPluginInterface Interface { get; private set; }
		internal Configuration Config { get; private set; }
		internal PluginCommandManager CommandManager { get; private set; }

		internal PlaySound SoundEffect { get; private set; }

		public void Initialize(DalamudPluginInterface pluginInterface) {
			this.PluginHelpCommand ??= this.GetType().GetMethod(nameof(DisplayPluginCommandHelp)).GetCustomAttribute<CommandAttribute>().Command;
			this.Interface = pluginInterface;
			this.Config = pluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
			this.Config.Initialize(pluginInterface);
			this.CommandManager = new(this);
			this.Common = new(this.Interface, Hooks.None); // just need the chat feature to send commands
			this.SoundEffect = new(pluginInterface.TargetModuleScanner);
		}

		#region IDisposable Support
		protected virtual void Dispose(bool disposing) {
			if (this.disposed) {
				return;
			}
			if (disposing) {
				this.Interface.SavePluginConfig(this.Config);
				this.CommandManager.Dispose();
				this.Interface.Dispose();
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
