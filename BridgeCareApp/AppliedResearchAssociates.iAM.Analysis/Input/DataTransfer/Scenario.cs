using System;
using System.Collections.Generic;
using System.Linq;

namespace AppliedResearchAssociates.iAM.Analysis.Input.DataTransfer
{
    public sealed class Scenario
    {
        public AnalysisMethod AnalysisMethod { get; set; }

        public List<CommittedProject> CommittedProjects { get; set; }

        public InvestmentPlan InvestmentPlan { get; set; }

        public Network Network { get; set; }

        public int NumberOfYearsOfTreatmentOutlook { get; set; }

        public string PassiveTreatmentID { get; set; }

        public List<PerformanceCurve> PerformanceCurves { get; set; }

        public List<SelectableTreatment> SelectableTreatments { get; set; }

        public bool ShouldPreapplyPassiveTreatment { get; set; }

        public Simulation Convert() => throw new NotImplementedException();

        public static Scenario Convert(Simulation source) => new()
        {
            AnalysisMethod = Convert(source.AnalysisMethod),
            CommittedProjects = source.CommittedProjects.Select(Convert).ToList(),
            InvestmentPlan = Convert(source.InvestmentPlan),
            Network = Convert(source.Network),
            NumberOfYearsOfTreatmentOutlook = source.NumberOfYearsOfTreatmentOutlook,
            PassiveTreatmentID = source.DesignatedPassiveTreatment.Name,
            PerformanceCurves = source.PerformanceCurves.Select(Convert).ToList(),
            SelectableTreatments = source.Treatments.Select(Convert).ToList(),
            ShouldPreapplyPassiveTreatment = source.ShouldPreapplyPassiveTreatment,
        };

        #region Conversion-from-this

        // TODO

        #endregion

        #region Conversion-to-this

        private static AnalysisMethod Convert(Analysis.AnalysisMethod source) => new()
        {
            AllowFundingFromMultipleBudgets = source.AllowFundingFromMultipleBudgets,
            BenefitAttributeID = source.Benefit.Attribute.Name,
            BenefitLimit = source.Benefit.Limit,
            BenefitWeightAttributeID = source.Weighting.Name,
            BudgetPriorities = source.BudgetPriorities.Select(Convert).ToList(),
            DeficientConditionGoals = source.DeficientConditionGoals.Select(Convert).ToList(),
            FilterExpression = source.Filter.Expression,
            OptimizationStrategy = source.OptimizationStrategy,
            RemainingLifeLimits = source.RemainingLifeLimits.Select(Convert).ToList(),
            ShouldApplyMultipleFeasibleCosts = source.ShouldApplyMultipleFeasibleCosts,
            ShouldDeteriorateDuringCashFlow = source.ShouldDeteriorateDuringCashFlow,
            ShouldRestrictCashFlowToFirstYearBudgets = source.ShouldRestrictCashFlowToFirstYearBudgets,
            SpendingStrategy = source.SpendingStrategy,
            TargetConditionGoals = source.TargetConditionGoals.Select(Convert).ToList(),
        };

        private static Asset Convert(AnalysisMaintainableAsset source)
        {
            var numberAttributes = source.HistoricalAttributes.OfType<Analysis.NumberAttribute>();
            var textAttributes = source.HistoricalAttributes.OfType<Analysis.TextAttribute>();

            return new()
            {
                ID = source.Id.ToString(),
                NumberAttributeHistories = numberAttributes.Select(convert).OrderBy(_ => _.AttributeID).ToList(),
                SpatialWeightExpression = source.SpatialWeighting.Expression,
                TextAttributeHistories = textAttributes.Select(convert).OrderBy(_ => _.AttributeID).ToList(),
            };

            AttributeValueHistory<T> convert<T>(Analysis.Attribute<T> attribute) => new()
            {
                AttributeID = attribute.Name,
                History = source.HistoryProvider.GetHistory(attribute).Select(Convert).OrderByDescending(_ => _.Year).ToList(),
            };
        }

        private static AttributeSystem Convert(Explorer source) => new()
        {
            CalculatedFields = source.CalculatedFields.Select(Convert).ToList(),
            NumberAttributes = source.NumberAttributes.Select(Convert).ToList(),
            TextAttributes = source.TextAttributes.Select(Convert).ToList(),
        };

        private static Budget Convert(Analysis.Budget source) => new()
        {
            ID = source.Name,
            YearlyAmounts = source.YearlyAmounts.Select(amount => amount.Value).ToList(),
        };

        private static BudgetCondition Convert(Analysis.BudgetCondition source) => new()
        {
            BudgetID = source.Budget.Name,
            ConditionExpression = source.Criterion.Expression,
        };

        private static BudgetPriority Convert(Analysis.BudgetPriority source) => new()
        {
            CriterionExpression = source.Criterion.Expression,
            PriorityLevel = source.PriorityLevel,
            Year = source.Year,
        };

        private static CalculatedField Convert(Analysis.CalculatedField source) => new()
        {
            ID = source.Name,
            IsDecreasingWithDeterioration = source.IsDecreasingWithDeterioration,
            Timing = source.Timing,
            ValueDefinitions = source.ValueSources.Select(Convert).ToList(),
        };

        private static CashFlowRule Convert(Analysis.CashFlowRule source) => new()
        {
            CriterionExpression = source.Criterion.Expression,
            DistributionRules = source.DistributionRules.Select(Convert).ToList(),
            ID = source.Name,
        };

        private static CashFlowDistributionRule Convert(Analysis.CashFlowDistributionRule source) => new()
        {
            CostCeiling = source.CostCeiling,
            YearlyPercentages = source.YearlyPercentages.ToList(),
        };

        private static CommittedProject Convert(Analysis.CommittedProject source) => new()
        {
            AssetID = source.Asset.Id.ToString(),
            Consequences = source.Consequences.Select(Convert).ToList(),
            Cost = source.Cost,
            ID = source.Name,
            ShadowForAnyTreatment = source.ShadowForAnyTreatment,
            ShadowForSameTreatment = source.ShadowForSameTreatment,
            UsableBudgetID = source.Budget.Name,
            Year = source.Year,
        };

        private static ConditionalTreatmentConsequence Convert(Analysis.ConditionalTreatmentConsequence source) => new()
        {
            AttributeID = source.Attribute.Name,
            ChangeExpression = source.Change.Expression,
            CriterionExpression = source.Criterion.Expression,
            EquationExpression = source.Equation.Expression,
        };

        private static CriterionEquationPair Convert(CalculatedFieldValueSource source) => new()
        {
            CriterionExpression = source.Criterion.Expression,
            EquationExpression = source.Equation.Expression,
        };

        private static CriterionEquationPair Convert(TreatmentCost source) => new()
        {
            CriterionExpression = source.Criterion.Expression,
            EquationExpression = source.Criterion.Expression,
        };

        private static DeficientConditionGoal Convert(Analysis.DeficientConditionGoal source) => new()
        {
            AllowedDeficientPercentage = source.AllowedDeficientPercentage,
            AttributeID = source.Attribute.Name,
            CriterionExpression = source.Criterion.Expression,
            DeficientLimit = source.DeficientLimit,
            ID = source.Name,
        };

        private static HistoricalValue<T> Convert<T>(KeyValuePair<int, T> source) => new()
        {
            Value = source.Value,
            Year = source.Key,
        };

        private static InvestmentPlan Convert(Analysis.InvestmentPlan source) => new()
        {
            Budgets = source.Budgets.Select(Convert).ToList(),
            BudgetConditions = source.BudgetConditions.Select(Convert).ToList(),
            CashFlowRules = source.CashFlowRules.Select(Convert).ToList(),
            FirstYearOfAnalysisPeriod = source.FirstYearOfAnalysisPeriod,
            InflationRatePercentage = source.InflationRatePercentage,
            MinimumProjectCostLimit = source.MinimumProjectCostLimit,
            NumberOfYearsInAnalysisPeriod = source.NumberOfYearsInAnalysisPeriod,
            ShouldAccumulateUnusedBudgetAmounts = source.ShouldAccumulateUnusedBudgetAmounts,
        };

        private static Network Convert(Analysis.Network source) => new()
        {
            Assets = source.Assets.Select(Convert).ToList(),
            AttributeSystem = Convert(source.Explorer),
        };

        private static NumberAttribute Convert(Analysis.NumberAttribute source) => new()
        {
            DefaultValue = source.DefaultValue,
            ID = source.Name,
            IsDecreasingWithDeterioration = source.IsDecreasingWithDeterioration,
            MaximumValue = source.Maximum,
            MinimumValue = source.Minimum,
        };

        private static PerformanceCurve Convert(Analysis.PerformanceCurve source) => new()
        {
            AttributeID = source.Attribute.Name,
            CriterionExpression = source.Criterion.Expression,
            EquationExpression = source.Equation.Expression,
            Shift = source.Shift,
        };

        private static RemainingLifeLimit Convert(Analysis.RemainingLifeLimit source) => new()
        {
            AttributeID = source.Attribute.Name,
            CriterionExpression = source.Criterion.Expression,
            Value = source.Value,
        };

        private static SelectableTreatment Convert(Analysis.SelectableTreatment source) => new()
        {
            Consequences = source.Consequences.Select(Convert).ToList(),
            Costs = source.Costs.Select(Convert).ToList(),
            FeasibilityCriterionExpressions = source.FeasibilityCriteria.Select(criterion => criterion.Expression).ToList(),
            ID = source.Name,
            Schedulings = source.Schedulings.Select(Convert).ToList(),
            ShadowForAnyTreatment = source.ShadowForAnyTreatment,
            ShadowForSameTreatment = source.ShadowForSameTreatment,
            Supersessions = source.Supersessions.Select(Convert).ToList(),
            UsableBudgetIDs = source.Budgets.Select(budget => budget.Name).ToList(),
        };

        private static TargetConditionGoal Convert(Analysis.TargetConditionGoal source) => new()
        {
            AttributeID = source.Attribute.Name,
            CriterionExpression = source.Criterion.Expression,
            ID = source.Name,
            Target = source.Target,
            Year = source.Year,
        };

        private static TextAttribute Convert(Analysis.TextAttribute source) => new()
        {
            DefaultValue = source.DefaultValue,
            ID = source.Name,
        };

        private static TreatmentConsequence Convert(Analysis.TreatmentConsequence source) => new()
        {
            AttributeID = source.Attribute.Name,
            ChangeExpression = source.Change.Expression,
        };

        private static TreatmentScheduling Convert(Analysis.TreatmentScheduling source) => new()
        {
            OffsetToFutureYear = source.OffsetToFutureYear,
            TreatmentID = source.Treatment.Name,
        };

        private static TreatmentSupersession Convert(Analysis.TreatmentSupersession source) => new()
        {
            CriterionExpression = source.Criterion.Expression,
            TreatmentID = source.Treatment.Name,
        };

        #endregion Conversion helpers
    }
}
