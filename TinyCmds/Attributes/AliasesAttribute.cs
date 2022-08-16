namespace TinyCmds.Attributes;

using System;


[AttributeUsage(AttributeTargets.Class)]
internal class AliasesAttribute: Attribute {
	public string[] Aliases { get; }

	public AliasesAttribute(params string[] aliases) {
		this.Aliases = aliases;
	}
}
