using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Dalamud.Game.Command;

using TinyCmds.Attributes;
using TinyCmds.Chat;

namespace TinyCmds {
	public class PluginCommandManager: IDisposable {

		private readonly List<PluginCommand> commandList;

		internal PluginCommand[] commands => this.commandList.ToArray();

		private readonly bool disposed = false;

		public PluginCommandManager() {
			this.commandList = typeof(TinyCmds).GetMethods(BindingFlags.Public | BindingFlags.Instance)
			this.commandList = typeof(PluginCommands).GetMethods(BindingFlags.Public | BindingFlags.Static)
				.Where(method => method.GetCustomAttribute<CommandAttribute>() is not null)
				.Select(m => new PluginCommand(m, TinyCmds.pluginHelpCommand, ChatUtil.ShowPrefixedError))
				.ToList();
			this.addCommandHandlers();
		}

		private void addCommandHandlers() {
			foreach (PluginCommand cmd in this.commands) {
				TinyCmds.cmdManager.AddHandler(cmd.Command, cmd.MainCommandInfo);
				CommandInfo hidden = cmd.AliasCommandInfo;
				foreach (string alt in cmd.Aliases) {
					TinyCmds.cmdManager.AddHandler(alt, hidden);
				}
			}
		}

		private void removeCommandHandlers() {
			foreach (PluginCommand cmd in this.commands) {
				TinyCmds.cmdManager.RemoveHandler(cmd.Command);
				foreach (string alt in cmd.Aliases) {
					TinyCmds.cmdManager.RemoveHandler(alt);
				}
			}
		}

		#region IDisposable Support
		protected virtual void Dispose(bool disposing) {
			if (this.disposed)
				return;
			this.removeCommandHandlers();
			this.commandList.Clear();
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
