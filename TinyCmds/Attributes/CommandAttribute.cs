namespace PrincessRTFM.TinyCmds.Attributes;

using System;


[AttributeUsage(AttributeTargets.Class)]
internal class CommandAttribute: Attribute {
	public string Command { get; }

	public CommandAttribute(string command) {
		this.Command = command;
	}
}
