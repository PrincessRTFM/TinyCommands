namespace TinyCmds;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Dalamud.Game.Command;
using Dalamud.Logging;

using TinyCmds.Attributes;

public delegate void PluginCommandInvocationErrorHandlerDelegate(params object[] payloads);

public class PluginCommandManager: IDisposable {

	private readonly List<PluginCommand> commandList;

	internal PluginCommand[] commands => this.commandList.ToArray();

	private readonly bool disposed = false;

	public PluginCommandInvocationErrorHandlerDelegate? ErrorHandler { get; internal set; }
	public PluginCommand? HelpHandler { get; internal set; }

	public PluginCommandManager() {
		Type b = typeof(PluginCommand);
		this.commandList = AppDomain.CurrentDomain.GetAssemblies()
			.SelectMany(asm => asm.GetTypes())
			.Where(t => t.IsSubclassOf(b) && !t.IsAbstract)
			.Where(t => t.GetConstructor(Array.Empty<Type>()) is not null)
			.Select(t => Activator.CreateInstance(t))
			.Where(o => o is not null)
			.Cast<PluginCommand>()
			.ToList();
		this.HelpHandler = this.commandList.Where(c => c.GetType().GetCustomAttribute<PluginCommandHelpHandlerAttribute>() is not null).FirstOrDefault();
	}

	internal void addCommandHandlers() {
		foreach (PluginCommand cmd in this.commands) {
			PluginLog.Information($"Registering command {cmd.InternalName} as {string.Join(", ", cmd.InvocationNames)}");
			Plugin.cmdManager.AddHandler(cmd.Command, cmd.MainCommandInfo);
			CommandInfo hidden = cmd.AliasCommandInfo;
			foreach (string alt in cmd.Aliases) {
				Plugin.cmdManager.AddHandler(alt, hidden);
			}
		}
	}

	internal void removeCommandHandlers() {
		foreach (PluginCommand cmd in this.commands) {
			PluginLog.Information($"Unregistering command {string.Join(", ", cmd.InvocationNames)} for {cmd.InternalName}");
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
