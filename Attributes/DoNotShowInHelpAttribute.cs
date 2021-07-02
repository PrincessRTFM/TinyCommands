using System;

namespace TinyCmds {
	[AttributeUsage(AttributeTargets.Method)]
	public class DoNotShowInHelpAttribute: Attribute {
	}
}