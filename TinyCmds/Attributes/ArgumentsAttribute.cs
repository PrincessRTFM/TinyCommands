using System;
using System.Linq;

namespace PrincessRTFM.TinyCmds.Attributes;

[AttributeUsage(AttributeTargets.Class)]
internal class ArgumentsAttribute(params string[] args): Attribute {
	public string ArgumentDescription => string.Join(" ", this.Arguments.Select(a => a.EndsWith('?') ? $"[{a.TrimEnd('?')}]" : $"<{a}>"));
	public string[] Arguments { get; } = args;
	public int RequiredArguments => this.Arguments.Count(a => !a.EndsWith('?'));
	public int MaxArguments => this.Arguments.Length;
}
