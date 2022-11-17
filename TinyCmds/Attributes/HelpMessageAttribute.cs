namespace PrincessRTFM.TinyCmds.Attributes;

using System;


[AttributeUsage(AttributeTargets.Class)]
internal class HelpMessageAttribute: Attribute {
	public string HelpMessage { get; }

	public HelpMessageAttribute(params string[] helpMessage) {
		this.HelpMessage = string.Join("\n", helpMessage);
	}
}
