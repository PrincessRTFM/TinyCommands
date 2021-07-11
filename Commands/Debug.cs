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
			this.Util.SendPrefixedChat(PluginUtil.Colour.GREY, "Received ", PluginUtil.Colour.BROWN, flags.Count, PluginUtil.Colour.GREY, " flags", PluginUtil.Colour.NONE);
			this.Util.SendDirectChat(PluginUtil.Colour.INDIGO, string.Join(" ", flags.Keys), PluginUtil.Colour.NONE);
			this.Util.SendPrefixedChat(PluginUtil.Colour.GREY, "Received ", PluginUtil.Colour.BROWN, args.Length, PluginUtil.Colour.GREY, " arguments", PluginUtil.Colour.NONE);
			foreach (string arg in args) {
				this.Util.SendDirectChat(PluginUtil.Colour.INDIGO, arg, PluginUtil.Colour.NONE);
			}
			this.Util.SendPrefixedChat($"Current job is {jobName} ({jobId})");
		}
	}
}
#endif