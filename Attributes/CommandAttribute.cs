namespace TinyCmds.Attributes;

using System;


[AttributeUsage(AttributeTargets.Method)]
internal class CommandAttribute: Attribute {
	public string Command { get; }

	public CommandAttribute(string command) {
		this.Command = command;
	}
}