namespace AppliedResearchAssociates.iAM.Analysis.Engine
{
    public enum SimulationLogStatus
    {
        // The entries "Error", "Warning", and "Information" match
        // the entries in ValidationStatus exactly. This is currently not enforced.
        Error,
        Warning,
        Information,
        Fatal,
        Progress,
    }
}
