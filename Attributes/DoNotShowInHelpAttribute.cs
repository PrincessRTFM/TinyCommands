namespace TinyCmds;

using System;


[AttributeUsage(AttributeTargets.Method)]
internal class DoNotShowInHelpAttribute: Attribute {
}