using System;

namespace TinyCmds.Attributes {
	[AttributeUsage(AttributeTargets.Method)]
	internal class AliasesAttribute: Attribute {
		public string[] Aliases { get; }

		public AliasesAttribute(params string[] aliases) {
			this.Aliases = aliases;
		}
	}
}