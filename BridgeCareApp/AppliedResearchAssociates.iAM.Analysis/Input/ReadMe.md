# AppliedResearchAssociates.iAM.Analysis.Input

This namespace sub-tree is WIP, but is intended (a) to provide a
serialization-friendly representation of the data required for analysis input
(done) and (b) to provide an immutable and optimized version of the current
analysis input data structures (not done).

The goals are (a) to facilitate automated characterization testing independent
of any other modules in the iAM software (controllers, databases, etc) and (b)
to eliminate a vast amount of bloating, bug-susceptible code in the current
analysis input data structures.
