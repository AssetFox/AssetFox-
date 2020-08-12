namespace AppliedResearchAssociates.iAM.Analysis
{
    public interface ITreatmentStatistics
    {
        double Benefit { get; }

        double CostPerUnitArea { get; }

        double? RemainingLife { get; }
    }
}
