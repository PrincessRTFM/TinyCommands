namespace PrincessRTFM.TinyCmds;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

using Dalamud.Game.Command;
using Dalamud.Logging;

using PrincessRTFM.TinyCmds.Attributes;

public delegate void PluginCommandInvocationErrorHandlerDelegate(params object[] payloads);

public class PluginCommandManager: IDisposable {

	private readonly List<PluginCommand> commandList;

	internal PluginCommand[] commands => this.commandList.ToArray();

	private readonly bool disposed = false;

	public PluginCommandInvocationErrorHandlerDelegate? ErrorHandler { get; internal set; }
	public PluginCommand? HelpHandler { get; internal set; }

	public PluginCommandManager(Plugin core) {
		Type b = typeof(PluginCommand);
		Type p = core.GetType();
		this.commandList = AppDomain.CurrentDomain.GetAssemblies()
			.SelectMany(asm => asm.GetTypes())
			.Where(t => t.IsSubclassOf(b) && !t.IsAbstract && t.GetConstructor(Array.Empty<Type>()) is not null)
			.Select(t => {
				try {
					ConstructorInfo ctor = t.GetConstructor(Array.Empty<Type>())!;
					object instance = FormatterServices.GetUninitializedObject(t);
					PropertyInfo prop = b
						.GetProperties(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
						.Where(prop => prop.PropertyType == p)
						.First();
					PluginLog.Information($"Injecting {p.Name} object to {t.Name}.{prop.Name}");
					prop.SetValue(instance, core);
					PluginLog.Information($"Invoking {t.Name}.<ctor>()");
					ctor.Invoke(instance, null);
					PluginCommand? cmd = instance as PluginCommand;
					cmd?.setup();
					return cmd;
				}
				catch (Exception e) {
					PluginLog.Error(e, $"Failed to instantiate {t.Name}");
					return null;
				}
			})
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

		if (disposing) {
			this.removeCommandHandlers();

			foreach (PluginCommand cmd in this.commandList)
				cmd.Dispose();

			this.commandList.Clear();
		}
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
