using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore
{
    public static class DataPersistenceConstants
    {
        public const string PennDotNetworkId = "D7B54881-DD44-4F93-8250-3D4A630A4D3B";
        public const string TestSimulationId = "DF71AC9B-B90A-425C-A519-7B2D6B531DDC";

        public const string AggregatedResultNumericDiscriminator = "NumericAggregatedResult";
        public const string AggregatedResultTextDiscriminator = "TextAggregatedResult";

        public const string SectionLocation = "SectionLocation";
        public const string LinearLocation = "LinearLocation";

        public static class EquationJoinEntities
        {
            public const string PerformanceCurve = "PerformanceCurveEntity";
            public const string TreatmentCost = "TreatmentCostEntity";
            public const string TreatmentConsequence = "ConditionalTreatmentConsequenceEntity";
        }

        public static class CriterionLibraryJoinEntities
        {
            public const string AnalysisMethod = "AnalysisMethodEntity";
            public const string Budget = "BudgetEntity";
            public const string BudgetPriority = "BudgetPriorityEntity";
            public const string DeficientConditionGoal = "DeficientConditionGoalEntity";
            public const string PerformanceCurve = "PerformanceCurveEntity";
            public const string CashFlowRule = "CashFlowRuleEntity";
            public const string RemainingLifeLimit = "RemainingLifeLimitEntity";
            public const string SelectableTreatment = "SelectableTreatmentEntity";
            public const string TargetConditionGoal = "TargetConditionGoalEntity";
            public const string ConditionalTreatmentConsequence = "ConditionalTreatmentConsequenceEntity";
            public const string TreatmentCost = "TreatmentCostEntity";
            public const string TreatmentSupersession = "TreatmentSupersessionEntity";

            public static readonly Dictionary<string, string> NameConventionPerEntityType = new Dictionary<string, string>
            {
                {AnalysisMethod, "Analysis Method"},
                {Budget, "Budget"},
                {BudgetPriority, "Budget Priority"},
                {DeficientConditionGoal, "Deficient Condition Goal"},
                {PerformanceCurve, "Performance Curve"},
                {CashFlowRule, "Cash Flow Rule"},
                {RemainingLifeLimit, "Remaining Life Limit"},
                {SelectableTreatment, "Feasibility"},
                {TargetConditionGoal, "Target Condition Goal"},
                {ConditionalTreatmentConsequence, "Treatment Consequence"},
                {TreatmentCost, "Treatment Cost"},
                {TreatmentSupersession, "Treatment Supersession"}
            };
        }

        public static class PennDOTKeyFields
        {
            public static readonly List<string> Keys = new List<string>
            {
                "BRKEY_",
                "BMSID"
            };
        }
    }
}
