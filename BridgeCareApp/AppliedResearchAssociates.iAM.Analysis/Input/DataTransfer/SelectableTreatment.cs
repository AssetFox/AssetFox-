using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.Analysis.Input.DataTransfer;

public sealed class SelectableTreatment : Treatment
{
    public List<ConditionalTreatmentConsequence> Consequences { get; set; } = new();

    public List<CriterionEquationPair> Costs { get; set; } = new();

    public List<string> FeasibilityCriterionExpressions { get; set; } = new();

    public List<string> NamesOfUsableBudgets { get; set; } = new();

    public List<PerformanceCurveAdjustmentFactor> PerformanceCurveAdjustmentFactors { get; set; } = new();

    public List<TreatmentScheduling> Schedulings { get; set; } = new();

    public List<TreatmentSupersession> Supersessions { get; set; } = new();
}
