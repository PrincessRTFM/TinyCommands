using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Dalamud.Game.Command;
using Dalamud.Plugin;

using TinyCmds.Attributes;

namespace TinyCmds {
	public class PluginCommandManager: IDisposable {

		private readonly DalamudPluginInterface pluginInterface;
		private readonly List<PluginCommand> commands;

		internal PluginCommand[] Commands => this.commands.ToArray();

		private readonly bool disposed = false;

		public PluginCommandManager(TinyCmds host) {
			this.pluginInterface = host.Interface;
			this.commands = host.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance)
				.Where(method => method.GetCustomAttribute<CommandAttribute>() is not null)
				.Select(m => new PluginCommand(host, m, host.DisplayPluginCommandHelp, host.ShowPrefixedChatError))
				.ToList();
			this.AddCommandHandlers();
		}

		private void AddCommandHandlers() {
			foreach (PluginCommand cmd in this.Commands) {
				this.pluginInterface.CommandManager.AddHandler(cmd.Command, cmd.MainCommandInfo);
				CommandInfo hidden = cmd.AliasCommandInfo;
				foreach (string alt in cmd.Aliases) {
					this.pluginInterface.CommandManager.AddHandler(alt, hidden);
				}
			}
		}

		private void RemoveCommandHandlers() {
			foreach (PluginCommand cmd in this.Commands) {
				this.pluginInterface.CommandManager.RemoveHandler(cmd.Command);
				foreach (string alt in cmd.Aliases) {
					this.pluginInterface.CommandManager.RemoveHandler(alt);
				}
			}
		}

		#region IDisposable Support
		protected virtual void Dispose(bool disposing) {
			if (this.disposed)
				return;
			this.RemoveCommandHandlers();
			this.commands.Clear();
		}
		public void Dispose() {
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}
		~PluginCommandManager() {
			this.Dispose(false);
		}
		#endregion
	}
}
