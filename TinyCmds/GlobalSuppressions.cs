// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Usage", "CA2201:Do not raise reserved exception types", Justification = "It's a stupid fucking rule. If you have null, throw a damn NullReferenceException.")]
