namespace TinyCmds.Attributes;

using System;


[AttributeUsage(AttributeTargets.Method)]
internal class HelpMessageAttribute: Attribute {
	public string HelpMessage { get; }

	public HelpMessageAttribute(params string[] helpMessage) {
		this.HelpMessage = string.Join("\n", helpMessage);
	}
}