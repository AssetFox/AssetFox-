namespace AppliedResearchAssociates.iAM.Analysis;

internal interface ITreatmentScheduling
{
    int OffsetToFutureYear { get; }

    Treatment TreatmentToSchedule { get; }
}
