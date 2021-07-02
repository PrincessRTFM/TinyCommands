using System;

namespace TinyCmds.Attributes {
	[AttributeUsage(AttributeTargets.Method)]
	public class SummaryAttribute: Attribute {
		public string Summary { get; }

		public SummaryAttribute(string helpMessage) {
			this.Summary = helpMessage;
		}
	}
}