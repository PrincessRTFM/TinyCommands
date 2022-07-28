namespace TinyCmds;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Dalamud.Game.Command;
using Dalamud.Logging;

using TinyCmds.Attributes;
using TinyCmds.Chat;

public class PluginCommandManager: IDisposable {

	private readonly List<PluginCommand> commandList;

	internal PluginCommand[] commands => this.commandList.ToArray();

	private readonly bool disposed = false;

	public PluginCommandManager() {
		this.commandList = typeof(PluginCommands).GetMethods(BindingFlags.Public | BindingFlags.Static)
			.Where(method => method.GetCustomAttribute<CommandAttribute>() is not null)
			.Select(m => new PluginCommand(m, Plugin.pluginHelpCommand, ChatUtil.ShowPrefixedError))
			.ToList();
		this.addCommandHandlers();
	}

	private void addCommandHandlers() {
		foreach (PluginCommand cmd in this.commands) {
			PluginLog.Information($"Registering command {cmd.MainCommandInfo.Handler.Method.Name} as {string.Join(", ", cmd.InvocationNames)}");
			Plugin.cmdManager.AddHandler(cmd.Command, cmd.MainCommandInfo);
			CommandInfo hidden = cmd.AliasCommandInfo;
			foreach (string alt in cmd.Aliases) {
				Plugin.cmdManager.AddHandler(alt, hidden);
			}
		}
	}

	private void removeCommandHandlers() {
		foreach (PluginCommand cmd in this.commands) {
			PluginLog.Information($"Unregistering command {string.Join(", ", cmd.InvocationNames)} for {cmd.MainCommandInfo.Handler.Method.Name}");
			Plugin.cmdManager.RemoveHandler(cmd.Command);
			foreach (string alt in cmd.Aliases) {
				Plugin.cmdManager.RemoveHandler(alt);
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
