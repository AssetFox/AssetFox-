using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.Analysis.Input.DataTransfer;

public sealed class SelectableTreatment : Treatment
{
    public List<ConditionalTreatmentConsequence> Consequences { get; set; }

    public List<CriterionEquationPair> Costs { get; set; }

    public List<string> FeasibilityCriterionExpressions { get; set; }

    public List<string> NamesOfUsableBudgets { get; set; }

    public List<PerformanceCurveAdjustmentFactor> PerformanceCurveAdjustmentFactors { get; set; }

    public List<TreatmentScheduling> Schedulings { get; set; }

    public List<TreatmentSupersession> Supersessions { get; set; }
}
