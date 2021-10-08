using System;

namespace TinyCmds {
	[AttributeUsage(AttributeTargets.Method)]
	internal class DoNotShowInHelpAttribute: Attribute {
	}
}