using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Dalamud.Game.Command;
using Dalamud.Plugin;

using TinyCmds.Attributes;

namespace TinyCmds {
	public partial class TinyCmdPluginCommandManager: IDisposable {

		internal delegate void PluginCommandDelegate(string command, string[] arguments, FlagMap flags);
		private readonly DalamudPluginInterface pluginInterface;
		private readonly List<PluginCommand> commands;

		internal PluginCommand[] Commands => this.commands.ToArray();

		public TinyCmdPluginCommandManager(TinyCmdsPlugin host) {
			this.pluginInterface = host.Interface;
			this.commands = host.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance)
				.Where(method => method.GetCustomAttribute<CommandAttribute>() != null)
				.Select(m => new PluginCommand(host, m, host.DisplayPluginCommandHelp, host.Util.SendChatError))
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

		public void Dispose() {
			this.RemoveCommandHandlers();
			this.commands.Clear();
		}
	}
}
