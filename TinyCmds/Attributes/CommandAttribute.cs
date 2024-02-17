using System;

namespace PrincessRTFM.TinyCmds.Attributes;

[AttributeUsage(AttributeTargets.Class)]
internal class CommandAttribute: Attribute {
	public string Command { get; }
	public string[] Aliases { get; }

	public CommandAttribute(string command, params string[] aliases) {
		this.Command = command;
		this.Aliases = aliases;
	}
}
