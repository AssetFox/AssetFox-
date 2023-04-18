# AppliedResearchAssociates.iAM.Analysis.ConsoleApp

This console app accepts exactly one command-line argument, which is the path to
an existing JSON file containing a complete `Scenario` object as defined by the
`AppliedResearchAssociates.iAM.Analysis.Input.DataTransfer.Scenario` type.

When this console app finishes running, it will produce an output JSON file
whose name uses the same base name as the input file but with a different
extension denoting it as output based on that input.

If you'd like to automatically generate a "real" input file from a scenario run
triggered in the iAM web UI, you can (prior to building and running the iAM
backend) temporarily uncomment the definition of the `dump_analysis_input`
symbol at the top of the `SimulationRunner` type definition file. This will
enable a block of code that converts the `Simulation` object given as input to a
complete `Scenario` object which is then dumped into a timestamped file in an
"iAM" folder created in the Desktop special folder.
