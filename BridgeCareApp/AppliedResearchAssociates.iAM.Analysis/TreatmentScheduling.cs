using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAM.Analysis;

public sealed class TreatmentScheduling : WeakEntity, IValidator, ITreatmentScheduling
{
    public int OffsetToFutureYear { get; set; }

    public string ShortDescription => nameof(TreatmentScheduling);

    public ValidatorBag Subvalidators => new();

    public SelectableTreatment TreatmentToSchedule { get; set; }

    Treatment ITreatmentScheduling.TreatmentToSchedule => TreatmentToSchedule;

    public ValidationResultBag GetDirectValidationResults()
    {
        var results = new ValidationResultBag();

        if (OffsetToFutureYear < 1)
        {
            results.Add(ValidationStatus.Error, "Offset to future year is less than one.", this, nameof(OffsetToFutureYear));
        }

        if (TreatmentToSchedule == null)
        {
            results.Add(ValidationStatus.Error, "Treatment is unset.", this, nameof(TreatmentToSchedule));
        }

        return results;
    }
}
