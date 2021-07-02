using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Dalamud.Game.Command;
using Dalamud.Plugin;

using TinyCmds.Attributes;

namespace TinyCmds {
	public class TinyCmdPluginCommandManager: IDisposable {
		internal class PluginCommand {
			internal delegate void PluginCommandInvocationErrorHandlerDelegate(string error);
			private readonly PluginCommandDelegate handler, helper;
			private readonly PluginCommandInvocationErrorHandlerDelegate error;
			public CommandInfo MainCommandInfo => new(this.Dispatch) {
				HelpMessage = this.Summary,
				ShowInHelp = this.ShowInDalamud,
			};
			public CommandInfo AliasCommandInfo => new(this.Dispatch) {
				HelpMessage = this.Summary,
				ShowInHelp = false,
			};
			public string CommandComparable => this.Command.TrimStart('/').ToLower();
			public string[] AliasesComparable => this.Aliases.Select(s => s.TrimStart('/').ToLower()).ToArray();
			public string[] HelpLines => this.Help.Split('\r', '\n').Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();
			public string Command { get; }
			public string Summary { get; }
			public string Help { get; }
			public string Usage { get; }
			public string[] Aliases { get; }
			public int MinArgs { get; }
			public int MaxArgs { get; }
			public bool ShowInDalamud { get; }
			public bool ShowInListing { get; }
			public PluginCommand(object instance, MethodInfo method, PluginCommandDelegate printHelp, PluginCommandInvocationErrorHandlerDelegate onError) {
				CommandAttribute attrCommand = method.GetCustomAttribute<CommandAttribute>();
				if (attrCommand is null) {
					throw new NullReferenceException("Cannot construct PluginCommand from method without CommandAttribute");
				}
				ArgumentsAttribute args = method.GetCustomAttribute<ArgumentsAttribute>();
				this.Command = $"/{attrCommand.Command.TrimStart('/')}";
				this.Summary = method.GetCustomAttribute<SummaryAttribute>()?.Summary ?? "";
				this.Help = method.GetCustomAttribute<HelpMessageAttribute>()?.HelpMessage ?? "";
				this.Usage = $"{this.Command} {args?.ArgumentDescription}".Trim();
				this.Aliases = method.GetCustomAttribute<AliasesAttribute>()?.Aliases ?? new string[0];
				this.ShowInDalamud = method.GetCustomAttribute<DoNotShowInHelpAttribute>() is null;
				this.ShowInListing = method.GetCustomAttribute<HideInCommandListingAttribute>() is null;
				this.MinArgs = args?.RequiredArguments ?? 0;
				this.MaxArgs = args?.MaxArguments ?? int.MaxValue;
				this.handler = Delegate.CreateDelegate(typeof(PluginCommandDelegate), instance, method) as PluginCommandDelegate;
				this.helper = printHelp;
				this.error = onError;
			}
			public void Dispatch(string command, string argline) {
				try {
					(FlagMap flags, string[] args) = PluginUtil.ParseArguments(argline);
					if (flags["h"]) {
						this.helper(null, new string[] { command }, flags);
						return;
					}
					if (args.Length < this.MinArgs) {
						this.error("Not enough arguments");
						this.helper(null, new string[] { command }, flags);
						return;
					}
					if (args.Length > this.MaxArgs) {
						this.error("Too many arguments");
						this.helper(null, new string[] { command }, flags);
						return;
					}
					this.handler(command, args, flags);
				}
				catch (Exception e) {
					while (e is not null) {
						this.error($"{e.GetType().Name}: {e.Message}\nat {e.TargetSite.DeclaringType.FullName} in {e.TargetSite.DeclaringType.Assembly}");
						e = e.InnerException;
					}
				}
			}
		}

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
