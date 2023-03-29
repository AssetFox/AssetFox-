# AppliedResearchAssociates.iAM.Analysis.Input

This namespace sub-tree is WIP, but is intended (a) to provide a
serialization-friendly representation of the data required for analysis input
(`DataTransfer` sub-namespace; complete) and (b) to provide an immutable and
optimized version of the current analysis input data structures (another
sub-namespace; not yet started).

The goals are (a) to facilitate automated characterization testing independent
of any other modules in the iAM software (controllers, databases, etc) and (b)
to eliminate a vast amount of bug-susceptible code (unnecessarily supporting
mutability features which are never used) in the current analysis input data
structures.

## Notes

Currently, the `DataTransfer` sub-namespace is geared only toward persistence of
data explicitly used within the analysis engine. There are some properties on
the current analysis input data structures that are not used within the analysis
engine but are instead used elsewhere, e.g. by the reporting module. Such
properties are currently not included on the types defined under `DataTransfer`.

**The `DataTransfer` types are not intended to provide persistence capability
for anything other than the iAM analysis.** That being said, the `DataTransfer`
types are probably the easiest for external code to work with when it comes to
mapping into and out of the analysis.
