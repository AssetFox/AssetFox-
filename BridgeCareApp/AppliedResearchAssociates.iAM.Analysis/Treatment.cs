using System.Collections.Generic;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.DTOs.Enums;
using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAM.Analysis;

public abstract class Treatment : WeakEntity, IValidator
{
    public TreatmentCategory Category { get; set; }

    public string Name { get; set; }

    public abstract IReadOnlyDictionary<NumberAttribute, double> PerformanceCurveAdjustmentFactors { get; }

    public abstract int ShadowForAnyTreatment { get; }

    public abstract int ShadowForSameTreatment { get; }

    public string ShortDescription => Name;

    public virtual ValidatorBag Subvalidators => new();

    public virtual ValidationResultBag GetDirectValidationResults()
    {
        var results = new ValidationResultBag();

        if (string.IsNullOrWhiteSpace(Name))
        {
            results.Add(ValidationStatus.Error, "Name is blank.", this, nameof(Name));
        }

        return results;
    }

    internal abstract bool CanUseBudget(Budget budget);

    internal abstract IReadOnlyCollection<ConsequenceApplicator> GetConsequenceApplicators(AssetContext scope);

    internal abstract double GetCost(AssetContext scope, bool shouldApplyMultipleFeasibleCosts);

    internal abstract IEnumerable<ITreatmentScheduling> GetSchedulings();
}
