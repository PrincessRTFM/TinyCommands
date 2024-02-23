using System.Diagnostics;

using Dalamud.Interface;
using Dalamud.Interface.Windowing;

using ImGuiNET;

namespace PrincessRTFM.TinyCmds.Ui;

internal abstract class BaseWindow: Window {
	private const ImGuiWindowFlags WindowFlags = ImGuiWindowFlags.AlwaysAutoResize;

	protected BaseWindow(string name, ImGuiWindowFlags flags = ImGuiWindowFlags.None) : base(name, WindowFlags | flags) {
		this.RespectCloseHotkey = true;
		this.IsOpen = false;
		TitleBarButton kofi = new() {
			Priority = int.MinValue,
			Icon = FontAwesomeIcon.Heart,
			IconOffset = new(2, 1),
			Click = _ => Process.Start(new ProcessStartInfo("https://ko-fi.com/V7V7IK9UU") { UseShellExecute = true }),
			ShowTooltip = () => {
				ImGui.BeginTooltip();
				ImGui.TextUnformatted("Support me on ko-fi");
				ImGui.EndTooltip();
			},
		};
		this.TitleBarButtons = new() {
			kofi,
		};
		this.AllowClickthrough = true;
		this.AllowPinning = true;
	}
}
