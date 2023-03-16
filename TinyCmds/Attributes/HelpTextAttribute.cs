namespace PrincessRTFM.TinyCmds.Attributes;

using System;


[AttributeUsage(AttributeTargets.Class)]
internal class HelpTextAttribute: Attribute {
	public string HelpMessage { get; }

	public HelpTextAttribute(params string[] helpMessage) {
		this.HelpMessage = string.Join("\n", helpMessage);
	}
}
