// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Style", "IDE0021:Use expression body for constructors", Justification = "As a general matter, this should not be required", Scope = "module")]
[assembly: SuppressMessage("Style", "IDE0022:Use expression body for methods", Justification = "As a general matter, this should not be required", Scope = "module")]
[assembly: SuppressMessage("Style", "IDE0046:Convert to conditional expression", Justification = "Conditional expressions are sometimes harder to read than 'if' statements", Scope = "module")]
[assembly: SuppressMessage("Performance", "CA1806:Do not ignore method results", Justification = "Sometimes a method result should be ignored", Scope = "module")]
[assembly: SuppressMessage("Style", "IDE0058:Expression value is never used", Justification = "this should not be a rule", Scope = "module")]
