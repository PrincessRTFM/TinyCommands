#if DEBUG
using Dalamud.Plugin;

using TinyCmds.Attributes;

namespace TinyCmds {
	public partial class TinyCmdsPlugin: IDalamudPlugin {
		[Command("/tinydebug")]
		[HelpMessage("Specifically for dev use")]
		[DoNotShowInHelp]
		[HideInCommandListing]
		public void PluginDebugCommand(string command, string[] args, FlagMap flags) {
			string jobName = this.Interface.ClientState.LocalPlayer.ClassJob.GameData.Abbreviation.ToString().ToUpper();
			uint jobId = this.Interface.ClientState.LocalPlayer.ClassJob.Id;
			this.SendPrefixedChat(
				ChatColour.GREY,
				"Received ",
				ChatColour.BROWN,
				flags.Count,
				ChatColour.GREY,
				" flags",
				ChatColour.NONE
			);
			this.SendDirectChat(
				ChatColour.INDIGO,
				string.Join(" ", flags.Keys),
				ChatColour.NONE
			);
			this.SendPrefixedChat(
				ChatColour.GREY,
				"Received ",
				ChatColour.BROWN,
				args.Length,
				ChatColour.GREY,
				" arguments",
				ChatColour.NONE
			);
			foreach (string arg in args) {
				this.SendDirectChat(ChatColour.INDIGO, arg, ChatColour.NONE);
			}
			this.SendPrefixedChat($"Current job is {jobName} ({jobId})");
		}
	}
}
#endif