using System;
using System.Linq;
using System.Reflection;

using Dalamud.Game.ClientState;
using Dalamud.Game.ClientState.Actors;
using Dalamud.Plugin;

using TinyCmds.Attributes;

namespace TinyCmds {
	public class TinyCmdsPlugin: IDalamudPlugin {

		public string Name => "TinyCommands";
		public readonly string Prefix = "TinyCmds";
		public string PluginHelpCommand { get; private set; }

		internal DalamudPluginInterface Interface { get; private set; }
		internal Configuration Config { get; private set; }
		internal TinyCmdPluginCommandManager CommandManager { get; private set; }
		internal PluginUtil Util { get; private set; }

		public void Initialize(DalamudPluginInterface pluginInterface) {
			this.PluginHelpCommand ??= this.GetType().GetMethod(nameof(DisplayPluginCommandHelp)).GetCustomAttribute<CommandAttribute>().Command;
			this.Interface = pluginInterface;
			this.Config = pluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
			this.Config.Initialize(pluginInterface);
			this.Util = new PluginUtil(this);
			this.CommandManager = new TinyCmdPluginCommandManager(this);
		}

		#region Plugin commands
#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable IDE0060 // Remove unused parameter
		[Command("/tinycmds")]
		[Arguments()]
		[Summary("List all plugin commands, along with their help messages")]
		[Aliases("/ptinycmds", "/tcmds", "/ptcmds")]
		[HelpMessage(
			"Lists all plugin commands.",
			"Use \"-a\" to include command aliases, \"-v\" to include help messages, or both (\"-av\" or \"-va\" or separately) for both."
		)]
		public void ListPluginCommands(string command, string[] args, FlagMap flags) {
			foreach (TinyCmdPluginCommandManager.PluginCommand cmd in this.CommandManager.Commands) {
				this.Util.SendPrefixedChat(
					PluginUtil.Colour.LIGHTBLUE,
					cmd.Usage,
					PluginUtil.Colour.NONE
				);
				if (flags["a"] && cmd.Aliases.Length > 0) {
					this.Util.SendPrefixedChat(
						PluginUtil.Colour.GREY,
						string.Join(", ", cmd.Aliases),
						PluginUtil.Colour.NONE
					);
				}
				if (flags["v"]) {
					foreach (string line in cmd.HelpLines) {
						this.Util.SendPrefixedChat(
							PluginUtil.Colour.PALEGREEN,
							line,
							PluginUtil.Colour.NONE
						);
					}
				}
			}
		}

		[Command("/tinyhelp")]
		[Arguments("command")]
		[Summary("Displays usage/help for the plugin's commands")]
		[Aliases("/ptinyhelp", "/thelp", "/pthelp", "/tinycmd", "/ptinycmd", "/tcmd", "/ptcmd")]
		public void DisplayPluginCommandHelp(string command, string[] args, FlagMap flags) {
			if (args.Length < 1) {
				this.Util.SendPrefixedChat($"{this.Name} uses a custom command parser that accepts single-character boolean flags starting with a hyphen.");
				this.Util.SendPrefixedChat(
					"These flags can be bundled into one argument, such that ",
					PluginUtil.Colour.YELLOW,
					"-va",
					PluginUtil.Colour.NONE,
					" will set both the ",
					PluginUtil.Colour.BROWN,
					"v",
					PluginUtil.Colour.NONE,
					" and ",
					PluginUtil.Colour.BROWN,
					"a",
					PluginUtil.Colour.NONE,
					" flags."
				);
				this.Util.SendPrefixedChat(
					"All plugin commands accept ",
					PluginUtil.Colour.YELLOW,
					"-h",
					PluginUtil.Colour.NONE,
					" to display their built-in help message."
				);
				this.Util.SendPrefixedChat(
					"To list all commands, use ",
					PluginUtil.Colour.TEAL,
					"/tinycmds",
					PluginUtil.Colour.NONE,
					", optionally with ",
					PluginUtil.Colour.YELLOW,
					"-a",
					PluginUtil.Colour.NONE,
					" to show their aliases and/or ",
					PluginUtil.Colour.YELLOW,
					"-v",
					PluginUtil.Colour.NONE,
					" to show their help messages."
				);
				return;
			}
			foreach (string listing in args) {
				string wanted = listing.TrimStart('/').ToLower();
				foreach (TinyCmdPluginCommandManager.PluginCommand cmd in this.CommandManager.Commands) {
					if (cmd.CommandComparable.Equals(wanted) || cmd.AliasesComparable.Contains(wanted)) {
						this.Util.SendPrefixedChat(PluginUtil.Colour.LIGHTBLUE, cmd.Usage, PluginUtil.Colour.NONE);
						if (flags["a"] && cmd.Aliases.Length > 0) {
							this.Util.SendPrefixedChat(
								PluginUtil.Colour.GREY,
								string.Join(", ", cmd.Aliases),
								PluginUtil.Colour.NONE
							);
						}
						foreach (string line in cmd.HelpLines) {
							this.Util.SendPrefixedChat(
								PluginUtil.Colour.PALEGREEN,
								line,
								PluginUtil.Colour.NONE
							);
						}
						return;
					}
				}
				this.Util.SendChatError($"Couldn't find plugin command '/{wanted}'");
			}
		}

		[Command("/timer")]
		[Arguments("delay", "name")]
		[Summary("Set an in-game alarm to go off AFTER a certain amount of time, instead of AT a given time")]
		[Aliases("/ptimer", "/delay", "/pdelay")]
		[HelpMessage(
			"The delay must be specified as \"??h??m\", where \"??\" is the number of hours/minutes to wait.",
			"If either number is zero, it and the following letter can be left off entirely.",
			"As an additional special case, if you are only setting a minute-level delay, you can leave out the \"m\" as well, as in \"10\" for ten minutes.",
			"This command sets a VANILLA in-game timer for the time calculated. If you use -v, the /alarm command will be printed. If you use -d, it will be printed and NOT run.",
			"If the timer name contains spaces, it MUST be enclosed in double quotes."
		)]
		public void GenerateTimedAlarm(string command, string[] args, FlagMap flags) {
			DateTime now = DateTime.Now;
			string timespec = args[0];
			string name = args[1];
			(int hours, int minutes) = PluginUtil.ParseTimespecString(timespec);
			if (hours == 0 && minutes == 0) {
				this.Util.SendChatError("A timer must delay for at least one minute");
				return;
			}
			int futureMinutes = now.Minute + minutes;
			if (futureMinutes >= 60) {
				hours += (int)Math.Floor((double)futureMinutes / 60);
				futureMinutes %= 60;
			}
			if (hours > 23) {
				this.Util.SendChatError("A timer's total delay cannot be more than 23:59");
				return;
			}
			int futureHours = now.Hour + hours;
			string cmd = $"/alarm \"{name}\" lt {futureHours}{futureMinutes}";
			if (flags["d"] || flags["v"]) {
				this.Util.SendPrefixedChat(PluginUtil.Colour.TEAL, cmd, PluginUtil.Colour.NONE);
				if (flags["d"])
					return;
			}
			this.Util.SendServerChat(cmd);
		}

		[Command("/ifcmd")]
		[Summary("Run a chat command (or directly send a message) only if a condition is met")]
		[Aliases("/ifthen")]
		[HelpMessage(
			"If the condition indicated by the flags is met, then all of the arguments will be executed as if entered into the chatbox manually. If no command/message is given, the test will print the result to your chatlog.",
			"Lowercase flags require that their condition be met, uppercase flags require that their condition NOT be met. Available flags are:",
			"-t has target, -f has focus, -o has mouseover, -c in combat, -p target is player, -n target is NPC, -m target is minion",
			"Remember that the plugin parses command arguments itself - if you want to use double quotes, you'll need to backslash-escape them!"
		)]
		public void RunChatIfCond(string command, string[] args, FlagMap flags) {
			//PlayerCharacter player = this.Interface.ClientState.LocalPlayer;
			//PartyList party = this.Interface.ClientState.PartyList;
			Targets targets = this.Interface.ClientState.Targets;
			Condition cond = this.Interface.ClientState.Condition;
			PluginUtil.Colour msgCol = PluginUtil.Colour.ORANGE;
			string msg = "Test passed but no command given";
			if (flags["t"] && targets.CurrentTarget is null)
				msg = "No target";
			else if (flags["T"] && targets.CurrentTarget is not null)
				msg = "Target present";
			else if (flags["p"] && targets.CurrentTarget?.ObjectKind is not ObjectKind.Player)
				msg = "Target is not player";
			else if (flags["P"] && targets.CurrentTarget?.ObjectKind is ObjectKind.Player)
				msg = "Target is player";
			else if (flags["n"] && targets.CurrentTarget?.ObjectKind is not ObjectKind.BattleNpc or ObjectKind.EventNpc or ObjectKind.Retainer)
				msg = "Target is not NPC";
			else if (flags["N"] && targets.CurrentTarget?.ObjectKind is ObjectKind.BattleNpc or ObjectKind.EventNpc or ObjectKind.Retainer)
				msg = "Target is NPC";
			else if (flags["m"] && targets.CurrentTarget?.ObjectKind is not ObjectKind.Companion)
				msg = "Target is not minion";
			else if (flags["M"] && targets.CurrentTarget?.ObjectKind is ObjectKind.Companion)
				msg = "Target is minion";
			else if (flags["f"] && targets.FocusTarget is null)
				msg = "No focus target";
			else if (flags["F"] && targets.FocusTarget is not null)
				msg = "Focus target present";
			else if (flags["o"] && targets.MouseOverTarget is null)
				msg = "No mouseover target";
			else if (flags["O"] && targets.MouseOverTarget is not null)
				msg = "Mouseover target present";
			else if (flags["c"] && !cond[ConditionFlag.InCombat])
				msg = "Not in combat";
			else if (flags["C"] && cond[ConditionFlag.InCombat])
				msg = "In combat";
			else
				msgCol = PluginUtil.Colour.GREEN;
			if (args.Length > 0) {
				if (msgCol == PluginUtil.Colour.GREEN) {
					this.Util.SendServerChat(string.Join(" ", args));
				}
			}
			else {
				this.Util.SendPrefixedChat(msgCol, msg, PluginUtil.Colour.NONE);
			}
		}

#if DEBUG
		[Command("/tinydebug")]
		[HelpMessage("Specifically for dev use")]
		[DoNotShowInHelp]
		[HideInCommandListing]
		public void PluginDebugCommand(string command, string[] args, FlagMap flags) {
			this.Util.SendPrefixedChat(PluginUtil.Colour.GREY, "Received ", PluginUtil.Colour.BROWN, flags.Count, PluginUtil.Colour.GREY, " flags", PluginUtil.Colour.NONE);
			this.Util.SendDirectChat(PluginUtil.Colour.INDIGO, string.Join(" ", flags.Keys), PluginUtil.Colour.NONE);
			this.Util.SendPrefixedChat(PluginUtil.Colour.GREY, "Received ", PluginUtil.Colour.BROWN, args.Length, PluginUtil.Colour.GREY, " arguments", PluginUtil.Colour.NONE);
			foreach (string arg in args) {
				this.Util.SendDirectChat(PluginUtil.Colour.INDIGO, arg, PluginUtil.Colour.NONE);
			}
		}
#endif
#pragma warning restore IDE0060 // Remove unused parameter
#pragma warning restore IDE0079 // Remove unnecessary suppression
		#endregion

		#region IDisposable Support
		protected virtual void Dispose(bool disposing) {
			if (!disposing) {
				return;
			}
			this.Interface.SavePluginConfig(this.Config);
			this.CommandManager.Dispose();
			this.Util.Dispose();
			this.Interface.Dispose();
		}

		public void Dispose() {
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}
		#endregion
	}
}
