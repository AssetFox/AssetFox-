// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Style", "IDE0046:Convert to conditional expression", Justification = "The 'simplified' version is harder to read than the 'unsimplified' version", Scope = "module")]
[assembly: SuppressMessage("Style", "IDE0022:Use expression body for methods", Justification = "This should not be expected", Scope = "module")]
