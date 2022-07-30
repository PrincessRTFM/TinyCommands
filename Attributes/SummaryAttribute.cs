namespace TinyCmds.Attributes;

using System;


[AttributeUsage(AttributeTargets.Class)]
internal class SummaryAttribute: Attribute {
	public string Summary { get; }

	public SummaryAttribute(string helpMessage) {
		this.Summary = helpMessage;
	}
}
