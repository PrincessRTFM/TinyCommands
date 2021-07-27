using Dalamud.Configuration;
using Dalamud.Plugin;

using Newtonsoft.Json;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// Why do I even HAVE this, TinyCommands doesn't have any settings!
namespace TinyCmds {
	public class Configuration: IPluginConfiguration {
		public int Version { get; set; } = 0;

		// Add any other properties or methods here.
		[JsonIgnore] private DalamudPluginInterface pluginInterface;

		public void Initialize(DalamudPluginInterface pluginInterface) {
			this.pluginInterface = pluginInterface;
		}

		public void Save() {
			this.pluginInterface.SavePluginConfig(this);
		}
	}
}
