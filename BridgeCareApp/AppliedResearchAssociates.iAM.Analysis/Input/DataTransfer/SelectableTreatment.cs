using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.Analysis.Input.DataTransfer;

public sealed class SelectableTreatment : Treatment
{
    public List<ConditionalTreatmentConsequence> Consequences { get; init; } = new();

    public List<CriterionEquationPair> Costs { get; init; } = new();

    public List<string> FeasibilityCriterionExpressions { get; init; } = new();

    public bool ForCommittedProjectsOnly { get; set; }

    public List<string> NamesOfUsableBudgets { get; init; } = new();

    public List<PerformanceCurveAdjustmentFactor> PerformanceCurveAdjustmentFactors { get; init; } = new();

    public List<TreatmentScheduling> Schedulings { get; init; } = new();

    public int ShadowForAnyTreatment { get; set; }

    public int ShadowForSameTreatment { get; set; }

    public List<TreatmentSupersedeRule> SupersedeRules { get; init; } = new();
}
