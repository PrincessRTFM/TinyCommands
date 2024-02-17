using System;

using Dalamud.Interface.Windowing;

using ImGuiNET;

namespace PrincessRTFM.TinyCmds.Ui;

internal class HelpWindow: Window, IDisposable {
	private const ImGuiWindowFlags WindowFlags = ImGuiWindowFlags.AlwaysAutoResize;

	private bool disposed;

	public readonly string
		Header,
		Subheader,
		Summary,
		Text;

	public HelpWindow(string title, string header, string subheader, string summary, string text) : base($"{Plugin.PluginName} Help: {title}", WindowFlags) {
		this.RespectCloseHotkey = true;
		this.IsOpen = false;
		this.SizeConstraints = new() {
			MinimumSize = new(250, 100),
			MaximumSize = new(400, 500),
		};

		this.Header = header;
		this.Subheader = subheader;
		this.Summary = summary;
		this.Text = text;
	}
	public HelpWindow(PluginCommand cmd) : this(cmd.Command, cmd.Usage, string.Join(", ", cmd.Aliases), cmd.Summary, cmd.Help) { }

	protected void DrawHeader() {

		ImGui.TextUnformatted(this.Header);

		ImGui.PushTextWrapPos(this.SizeConstraints!.Value.MaximumSize.X - ImGui.GetStyle().WindowPadding.X);

		if (!string.IsNullOrWhiteSpace(this.Subheader)) {
			ImGui.Indent();
			ImGui.BeginDisabled();
			ImGui.TextUnformatted(this.Subheader);
			ImGui.EndDisabled();
			ImGui.Unindent();
		}

		ImGui.Spacing();
		ImGui.Spacing();
		ImGui.Spacing();

		if (string.IsNullOrWhiteSpace(this.Summary)) {
			ImGui.BeginDisabled();
			ImGui.TextUnformatted("[no summary available]");
			ImGui.EndDisabled();
		}
		else {
			ImGui.TextUnformatted(this.Summary);
		}

		ImGui.PopTextWrapPos();

		ImGui.Spacing();
		ImGui.Spacing();
		ImGui.Spacing();

		ImGui.Separator();

		ImGui.Spacing();
		ImGui.Spacing();
		ImGui.Spacing();
	}
	public override void Draw() {
		this.DrawHeader();

		ImGui.PushTextWrapPos(ImGui.GetContentRegionMax().X - 1);
		ImGui.TextUnformatted(this.Text);
		ImGui.PopTextWrapPos();
	}

	#region Disposable
	protected virtual void Dispose(bool disposing) {
		if (this.disposed)
			return;
		this.disposed = true;

		if (disposing) {
			// nop
		}

	}

	~HelpWindow() {
		this.Dispose(false);
	}

	public void Dispose() {
		this.Dispose(true);
		GC.SuppressFinalize(this);
	}
	#endregion
}
