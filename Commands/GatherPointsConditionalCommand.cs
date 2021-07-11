using System.Linq;

using Dalamud.Game.ClientState.Actors.Types;
using Dalamud.Plugin;

using TinyCmds.Attributes;

namespace TinyCmds {
	public partial class TinyCmdsPlugin: IDalamudPlugin {
		[Command("/ifgp")]
		[Summary("Run a chat command (or directly send a message) only if GP meets condition")]
		[HelpMessage(
			"Similar to /ifcmd, but specifically checks numeric inequality conditions against your GP to allow running commands based on how much you have.",
			"There are three possible tests: at least (-g), less than (-l), and a simple at capacity (-c).",
			"If using -g or -l, the first argument should be a number to compare against. If using -c, ALL arguments are the command to run when your GP passes the check."
		)]
		[RawArgs]
		public void RunChatIfPlayerGp(string command, string[] args, FlagMap flags) {
			string arg = args[0] ?? string.Empty;
			PlayerCharacter player = this.Interface.ClientState.LocalPlayer;
			int gp = player.CurrentGp;
			if (player.MaxGp < 1) {
				// presumably not a gathering job, or maybe they have none unlocked - is MaxGp >0 when DoL is unlocked but current job is different?
				this.Util.Debug("You have no GP");
			}
			else if (flags["c"]) {
				if (player.CurrentGp >= player.MaxGp) {
					if (arg.Length > 0) {
						this.Util.SendServerChat(arg);
					}
					else {
						this.Util.SendPrefixedChat(PluginUtil.Colour.GREEN, $"GP is at capacity ({gp})", PluginUtil.Colour.NONE);
					}
				}
				else if (arg.Length < 1) {
					this.Util.SendPrefixedChat(PluginUtil.Colour.ORANGE, $"GP is below capacity ({gp})", PluginUtil.Colour.NONE);
				}
			}
			else if (flags["g"] || flags["l"]) {
				if (arg.Length < 1) {
					this.Util.SendChatError("-g and -l both require a number to compare your current GP against");
				}
				else {
					string num = arg.Split()[0];
					string cmd = arg.Substring(num.Length).Trim();
					if (int.TryParse(num, out int compareTo)) {
						if (flags["g"]) {
							if (gp >= compareTo) {
								if (cmd.Length > 0) {
									this.Util.SendServerChat(cmd);
								}
								else {
									this.Util.SendPrefixedChat(PluginUtil.Colour.GREEN, $"GP is at least {compareTo} ({gp})", PluginUtil.Colour.NONE);
								}
							}
							else if (cmd.Length < 1) {
								this.Util.SendPrefixedChat(PluginUtil.Colour.ORANGE, $"GP is too low ({gp})", PluginUtil.Colour.NONE);
							}
						}
						else if (flags["l"]) {
							if (gp < compareTo) {
								if (cmd.Length > 0) {
									this.Util.SendServerChat(cmd);
								}
								else {
									this.Util.SendPrefixedChat(PluginUtil.Colour.GREEN, $"GP is below {compareTo} ({gp})", PluginUtil.Colour.NONE);
								}
							}
							else if (cmd.Length < 1) {
								this.Util.SendPrefixedChat(PluginUtil.Colour.ORANGE, $"GP is too high ({gp})", PluginUtil.Colour.NONE);
							}
						}
					}
					else {
						this.Util.SendChatError($"Couldn't parse \"{num}\" as an integer");
					}
				}
			}
			else {
				this.Util.SendChatError("Expected one of -c, -g, or -l, but found none");
				this.Util.SendPrefixedChat(
					"(Try ",
					PluginUtil.Colour.TEAL,
					$"/{command.TrimStart('/')} -h",
					PluginUtil.Colour.NONE,
					" for help)"
				);
			}
		}
	}
}
