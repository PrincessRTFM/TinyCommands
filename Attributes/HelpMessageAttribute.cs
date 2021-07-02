using System;

namespace TinyCmds.Attributes {
	[AttributeUsage(AttributeTargets.Method)]
	public class HelpMessageAttribute: Attribute {
		public string HelpMessage { get; }

		public HelpMessageAttribute(params string[] helpMessage) {
			this.HelpMessage = string.Join("\n", helpMessage);
		}
	}
}