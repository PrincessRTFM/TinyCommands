using System;

namespace PrincessRTFM.TinyCmds.Attributes;

[AttributeUsage(AttributeTargets.Class)]
internal class HelpTextAttribute(params string[] helpMessage): Attribute {
	public string HelpMessage { get; } = string.Join("\n", helpMessage);
}
