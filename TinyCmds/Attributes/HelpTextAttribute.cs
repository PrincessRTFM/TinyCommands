using System;

namespace PrincessRTFM.TinyCmds.Attributes;

[AttributeUsage(AttributeTargets.Class)]
internal class HelpTextAttribute: Attribute {
	public string HelpMessage { get; }

	public HelpTextAttribute(params string[] helpMessage) => this.HelpMessage = string.Join("\n", helpMessage);
}
