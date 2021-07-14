#if DEBUG
using Dalamud.Plugin;

using TinyCmds.Attributes;

namespace TinyCmds {
	public partial class TinyCmdsPlugin: IDalamudPlugin {
		[Command("/tinydebug")]
		[HelpMessage("Specifically for dev use")]
		[DoNotShowInHelp]
		[HideInCommandListing]
		public void PluginDebugCommand(string command, string argline, FlagMap flags, ref bool showHelp) {
			string[] args = ShellParse(argline);
			string jobName = this.Interface.ClientState.LocalPlayer.ClassJob.GameData.Abbreviation.ToString().ToUpper();
			uint jobId = this.Interface.ClientState.LocalPlayer.ClassJob.Id;
			this.SendPrefixedChat(
				ChatColour.QUIET,
				"Received ",
				ChatColour.DEBUG,
				flags.Count,
				ChatColour.QUIET,
				" flags",
				ChatColour.RESET
			);
			this.SendDirectChat(
				ChatColour.DEBUG,
				string.Join(" ", flags.Keys),
				ChatColour.RESET
			);
			this.SendPrefixedChat(
				ChatColour.QUIET,
				"Received ",
				ChatColour.DEBUG,
				args.Length,
				ChatColour.QUIET,
				" arguments",
				ChatColour.RESET
			);
			foreach (string arg in args) {
				this.SendDirectChat(ChatColour.DEBUG, arg, ChatColour.RESET);
			}
			this.SendPrefixedChat(
				"Current job is ",
				ChatColour.JOB,
				jobName,
				ChatColour.RESET,
				" (",
				ChatColour.DEBUG,
				jobId,
				ChatColour.RESET,
				")"
			);
		}
	}
}
#endif