using System;
using System.Collections.Generic;
using System.Linq;

namespace AppliedResearchAssociates.iAM.Analysis.Input.DataTransfer;

public sealed class Scenario
{
    public AnalysisMethod AnalysisMethod { get; set; }

    public List<CommittedProject> CommittedProjects { get; set; }

    public InvestmentPlan InvestmentPlan { get; set; }

    public string Name { get; set; }

    public string NameOfPassiveTreatment { get; set; }

    public Network Network { get; set; }

    public int NumberOfYearsOfTreatmentOutlook { get; set; }

    public List<PerformanceCurve> PerformanceCurves { get; set; }

    public List<SelectableTreatment> SelectableTreatments { get; set; }

    public bool ShouldPreapplyPassiveTreatment { get; set; }

    public static Scenario ConvertIn(Simulation source) => new()
    {
        AnalysisMethod = Convert(source.AnalysisMethod),
        CommittedProjects = source.CommittedProjects.Select(Convert).ToList(),
        InvestmentPlan = Convert(source.InvestmentPlan),
        Name = source.Name,
        Network = Convert(source.Network),
        NumberOfYearsOfTreatmentOutlook = source.NumberOfYearsOfTreatmentOutlook,
        NameOfPassiveTreatment = source.DesignatedPassiveTreatment.Name,
        PerformanceCurves = source.PerformanceCurves.Select(Convert).ToList(),
        SelectableTreatments = source.Treatments.Select(Convert).ToList(),
        ShouldPreapplyPassiveTreatment = source.ShouldPreapplyPassiveTreatment,
    };

    public Simulation ConvertOut() => new OutConverter().Convert(this);

    #region Conversion-to-this

    private static AnalysisMethod Convert(Analysis.AnalysisMethod source) => new()
    {
        AllowFundingFromMultipleBudgets = source.AllowFundingFromMultipleBudgets,
        BenefitAttributeName = source.Benefit.Attribute.Name,
        BenefitLimit = source.Benefit.Limit,
        BenefitWeightAttributeName = source.Weighting.Name,
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

    private static MaintainableAsset Convert(AnalysisMaintainableAsset source)
    {
        var numberAttributes = source.HistoricalAttributes.OfType<Analysis.NumberAttribute>();
        var textAttributes = source.HistoricalAttributes.OfType<Analysis.TextAttribute>();

        return new()
        {
            ID = source.Id,
            Name = source.AssetName,
            NumberAttributeHistories = numberAttributes.Select(convert).OrderBy(_ => _.AttributeName).ToList(),
            SpatialWeightExpression = source.SpatialWeighting.Expression,
            TextAttributeHistories = textAttributes.Select(convert).OrderBy(_ => _.AttributeName).ToList(),
        };

        AttributeValueHistory<T> convert<T>(Analysis.Attribute<T> attribute) => new()
        {
            AttributeName = attribute.Name,
            History = source.HistoryProvider.GetHistory(attribute).Select(Convert).OrderByDescending(_ => _.Year).ToList(),
        };
    }

    private static AttributeSystem Convert(Explorer source) => new()
    {
        AgeAttributeName = source.AgeAttribute.Name,
        CalculatedFields = source.CalculatedFields.Select(Convert).ToList(),
        NumberAttributes = source.NumberAttributes.Select(Convert).ToList(),
        TextAttributes = source.TextAttributes.Select(Convert).ToList(),
    };

    private static Budget Convert(Analysis.Budget source) => new()
    {
        Name = source.Name,
        YearlyAmounts = source.YearlyAmounts.Select(amount => amount.Value).ToList(),
    };

    private static BudgetCondition Convert(Analysis.BudgetCondition source) => new()
    {
        BudgetName = source.Budget.Name,
        ConditionExpression = source.Criterion.Expression,
    };

    private static BudgetPercentagePair Convert(Analysis.BudgetPercentagePair source) => new()
    {
        BudgetName = source.Budget.Name,
        Percentage = source.Percentage,
    };

    private static BudgetPriority Convert(Analysis.BudgetPriority source) => new()
    {
        BudgetPercentagePairs = source.BudgetPercentagePairs.Select(Convert).ToList(),
        CriterionExpression = source.Criterion.Expression,
        PriorityLevel = source.PriorityLevel,
        Year = source.Year,
    };

    private static CalculatedField Convert(Analysis.CalculatedField source) => new()
    {
        Name = source.Name,
        IsDecreasingWithDeterioration = source.IsDecreasingWithDeterioration,
        Timing = source.Timing,
        ValueDefinitions = source.ValueSources.Select(Convert).ToList(),
    };

    private static CashFlowRule Convert(Analysis.CashFlowRule source) => new()
    {
        CriterionExpression = source.Criterion.Expression,
        DistributionRules = source.DistributionRules.Select(Convert).ToList(),
        Name = source.Name,
    };

    private static CashFlowDistributionRule Convert(Analysis.CashFlowDistributionRule source) => new()
    {
        CostCeiling = source.CostCeiling,
        YearlyPercentages = source.YearlyPercentages.ToList(),
    };

    private static CommittedProject Convert(Analysis.CommittedProject source) => new()
    {
        AssetID = source.Asset.Id,
        Consequences = source.Consequences.Select(Convert).ToList(),
        Cost = source.Cost,
        Name = source.Name,
        ShadowForAnyTreatment = source.ShadowForAnyTreatment,
        ShadowForSameTreatment = source.ShadowForSameTreatment,
        NameOfUsableBudget = source.Budget.Name,
        Year = source.Year,
    };

    private static ConditionalTreatmentConsequence Convert(Analysis.ConditionalTreatmentConsequence source) => new()
    {
        AttributeName = source.Attribute.Name,
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
        EquationExpression = source.Equation.Expression,
    };

    private static DeficientConditionGoal Convert(Analysis.DeficientConditionGoal source) => new()
    {
        AllowedDeficientPercentage = source.AllowedDeficientPercentage,
        AttributeName = source.Attribute.Name,
        CriterionExpression = source.Criterion.Expression,
        DeficientLimit = source.DeficientLimit,
        Name = source.Name,
    };

    private static PerformanceCurveAdjustmentFactor Convert(KeyValuePair<Analysis.Attribute, double> source) => new()
    {
        AttributeName = source.Key.Name,
        Value = source.Value,
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
        AttributeSystem = Convert(source.Explorer),
        MaintainableAssets = source.Assets.Select(Convert).ToList(),
        Name = source.Name,
    };

    private static NumberAttribute Convert(Analysis.NumberAttribute source) => new()
    {
        DefaultValue = source.DefaultValue,
        Name = source.Name,
        IsDecreasingWithDeterioration = source.IsDecreasingWithDeterioration,
        MaximumValue = source.Maximum,
        MinimumValue = source.Minimum,
    };

    private static PerformanceCurve Convert(Analysis.PerformanceCurve source) => new()
    {
        AttributeName = source.Attribute.Name,
        CriterionExpression = source.Criterion.Expression,
        EquationExpression = source.Equation.Expression,
        Name = source.Name,
        Shift = source.Shift,
    };

    private static RemainingLifeLimit Convert(Analysis.RemainingLifeLimit source) => new()
    {
        AttributeName = source.Attribute.Name,
        CriterionExpression = source.Criterion.Expression,
        Value = source.Value,
    };

    private static SelectableTreatment Convert(Analysis.SelectableTreatment source) => new()
    {
        Consequences = source.Consequences.Select(Convert).ToList(),
        Costs = source.Costs.Select(Convert).ToList(),
        FeasibilityCriterionExpressions = source.FeasibilityCriteria.Select(criterion => criterion.Expression).ToList(),
        Name = source.Name,
        PerformanceCurveAdjustmentFactors = source.PerformanceCurveAdjustmentFactors.Select(Convert).ToList(),
        Schedulings = source.Schedulings.Select(Convert).ToList(),
        ShadowForAnyTreatment = source.ShadowForAnyTreatment,
        ShadowForSameTreatment = source.ShadowForSameTreatment,
        Supersessions = source.Supersessions.Select(Convert).ToList(),
        NamesOfUsableBudgets = source.Budgets.Select(budget => budget.Name).ToList(),
    };

    private static TargetConditionGoal Convert(Analysis.TargetConditionGoal source) => new()
    {
        AttributeName = source.Attribute.Name,
        CriterionExpression = source.Criterion.Expression,
        Name = source.Name,
        Target = source.Target,
        Year = source.Year,
    };

    private static TextAttribute Convert(Analysis.TextAttribute source) => new()
    {
        DefaultValue = source.DefaultValue,
        Name = source.Name,
    };

    private static TreatmentConsequence Convert(Analysis.TreatmentConsequence source) => new()
    {
        AttributeName = source.Attribute.Name,
        ChangeExpression = source.Change.Expression,
    };

    private static TreatmentScheduling Convert(Analysis.TreatmentScheduling source) => new()
    {
        OffsetToFutureYear = source.OffsetToFutureYear,
        TreatmentName = source.Treatment.Name,
    };

    private static TreatmentSupersession Convert(Analysis.TreatmentSupersession source) => new()
    {
        CriterionExpression = source.Criterion.Expression,
        TreatmentName = source.Treatment.Name,
    };

    #endregion

    #region Conversion-from-this

    private sealed class OutConverter
    {
        public Simulation Convert(Scenario source)
        {
            var result = Convert(source.Network).AddSimulation();

            result.Name = source.Name;
            result.NumberOfYearsOfTreatmentOutlook = source.NumberOfYearsOfTreatmentOutlook;
            result.ShouldPreapplyPassiveTreatment = source.ShouldPreapplyPassiveTreatment;

            Convert(source.AnalysisMethod, result.AnalysisMethod);
            Convert(source.InvestmentPlan, result.InvestmentPlan);

            foreach (var item in source.PerformanceCurves)
            {
                Convert(item, result);
            }

            foreach (var item in source.SelectableTreatments)
            {
                Convert(item, result);
            }

            TreatmentByName = result.Treatments.ToDictionary(_ => _.Name);

            foreach (var item in source.CommittedProjects)
            {
                result.CommittedProjects.Add(Convert(item));
            }

            TreatmentByName[source.NameOfPassiveTreatment].DesignateAsPassiveForSimulation();

            while (ActionsToPerformAtEndOfConversion.TryDequeue(out var action))
            {
                action();
            }

            return result;
        }

        private readonly Queue<Action> ActionsToPerformAtEndOfConversion = new();

        private Dictionary<Guid, AnalysisMaintainableAsset> AssetByID;
        private Dictionary<string, Analysis.Attribute> AttributeByName;
        private Dictionary<string, Analysis.Budget> BudgetByName;
        private Dictionary<string, Analysis.NumberAttribute> NumberAttributeByName;
        private Dictionary<string, INumericAttribute> NumericAttributeByName;
        private Dictionary<string, Analysis.TextAttribute> TextAttributeByName;
        private Dictionary<string, Analysis.SelectableTreatment> TreatmentByName;

        private void Convert(SelectableTreatment source, Simulation target)
        {
            var result = target.AddTreatment();

            result.Category = source.Category;
            result.Name = source.Name;
            result.ShadowForAnyTreatment = source.ShadowForAnyTreatment;
            result.ShadowForSameTreatment = source.ShadowForSameTreatment;

            foreach (var item in source.NamesOfUsableBudgets)
            {
                result.Budgets.Add(BudgetByName[item]);
            }

            foreach (var item in source.Consequences)
            {
                Convert(item, result);
            }

            foreach (var item in source.Costs)
            {
                Convert(item, result);
            }

            foreach (var item in source.FeasibilityCriterionExpressions)
            {
                result.AddFeasibilityCriterion().Expression = item;
            }

            foreach (var item in source.PerformanceCurveAdjustmentFactors)
            {
                var attribute = AttributeByName[item.AttributeName];
                result.PerformanceCurveAdjustmentFactors[attribute] = item.Value;
            }

            foreach (var item in source.Schedulings)
            {
                Convert(item, result);
            }

            foreach (var item in source.Supersessions)
            {
                Convert(item, result);
            }
        }

        private void Convert(TreatmentSupersession source, Analysis.SelectableTreatment target)
        {
            var result = target.AddSupersession();

            result.Criterion.Expression = source.CriterionExpression;

            ActionsToPerformAtEndOfConversion.Enqueue(
                () => result.Treatment = TreatmentByName[source.TreatmentName]);
        }

        private void Convert(TreatmentScheduling source, Analysis.SelectableTreatment target)
        {
            var result = new Analysis.TreatmentScheduling { OffsetToFutureYear = source.OffsetToFutureYear };

            target.Schedulings.Add(result);

            ActionsToPerformAtEndOfConversion.Enqueue(
                () => result.Treatment = TreatmentByName[source.TreatmentName]);
        }

        private static void Convert(CriterionEquationPair source, Analysis.SelectableTreatment target)
        {
            var result = target.AddCost();

            result.Criterion.Expression = source.CriterionExpression;
            result.Equation.Expression = source.EquationExpression;
        }

        private void Convert(ConditionalTreatmentConsequence source, Analysis.SelectableTreatment target)
        {
            var result = target.AddConsequence();

            result.Attribute = AttributeByName[source.AttributeName];
            result.Change.Expression = source.ChangeExpression;
            result.Criterion.Expression = source.CriterionExpression;
            result.Equation.Expression = source.EquationExpression;
        }

        private void Convert(PerformanceCurve source, Simulation target)
        {
            var result = target.AddPerformanceCurve();

            result.Attribute = NumberAttributeByName[source.AttributeName];
            result.Criterion.Expression = source.CriterionExpression;
            result.Equation.Expression = source.EquationExpression;
            result.Name = source.Name;
            result.Shift = source.Shift;
        }

        private void Convert(BudgetPriority source, Analysis.AnalysisMethod target)
        {
            var result = target.AddBudgetPriority();

            result.Criterion.Expression = source.CriterionExpression;
            result.PriorityLevel = source.PriorityLevel;
            result.Year = source.Year;

            var percentageByBudget = source.BudgetPercentagePairs.ToDictionary(_ => _.BudgetName, _ => _.Percentage);

            ActionsToPerformAtEndOfConversion.Enqueue(() =>
            {
                foreach (var item in result.BudgetPercentagePairs)
                {
                    item.Percentage = percentageByBudget[item.Budget.Name];
                }
            });
        }

        private static void Convert(TextAttribute source, Explorer target)
        {
            var result = target.AddTextAttribute(source.Name);

            result.DefaultValue = source.DefaultValue;
        }

        private static void Convert(NumberAttribute source, Explorer target)
        {
            var result = target.AddNumberAttribute(source.Name);

            result.DefaultValue = source.DefaultValue;
            result.IsDecreasingWithDeterioration = source.IsDecreasingWithDeterioration;
            result.Maximum = source.MaximumValue;
            result.Minimum = source.MinimumValue;
        }

        private static void Convert(CalculatedField source, Explorer target)
        {
            var result = target.AddCalculatedField(source.Name);

            result.IsDecreasingWithDeterioration = source.IsDecreasingWithDeterioration;
            result.Timing = source.Timing;

            foreach (var item in source.ValueDefinitions)
            {
                Convert(item, result);
            }
        }

        private static void Convert(CriterionEquationPair source, Analysis.CalculatedField target)
        {
            var result = target.AddValueSource();

            result.Criterion.Expression = source.CriterionExpression;
            result.Equation.Expression = source.EquationExpression;
        }

        private static void Convert<TValue, TAttribute>(AttributeValueHistory<TValue> source, AnalysisMaintainableAsset target, Dictionary<string, TAttribute> attributeByName) where TAttribute : Analysis.Attribute<TValue>
        {
            var history = target.GetHistory(attributeByName[source.AttributeName]);

            foreach (var datum in source.History)
            {
                history[datum.Year] = datum.Value;
            }
        }

        private void Convert(InvestmentPlan source, Analysis.InvestmentPlan target)
        {
            target.FirstYearOfAnalysisPeriod = source.FirstYearOfAnalysisPeriod;
            target.InflationRatePercentage = source.InflationRatePercentage;
            target.MinimumProjectCostLimit = source.MinimumProjectCostLimit;
            target.NumberOfYearsInAnalysisPeriod = source.NumberOfYearsInAnalysisPeriod;
            target.ShouldAccumulateUnusedBudgetAmounts = source.ShouldAccumulateUnusedBudgetAmounts;

            foreach (var item in source.Budgets)
            {
                Convert(item, target);
            }

            BudgetByName = target.Budgets.ToDictionary(_ => _.Name);

            foreach (var item in source.BudgetConditions)
            {
                Convert(item, target);
            }

            foreach (var item in source.CashFlowRules)
            {
                Convert(item, target);
            }
        }

        private static void Convert(CashFlowRule source, Analysis.InvestmentPlan target)
        {
            var result = target.AddCashFlowRule();

            result.Criterion.Expression = source.CriterionExpression;
            result.Name = source.Name;

            foreach (var item in source.DistributionRules)
            {
                result.DistributionRules.Add(Convert(item));
            }
        }

        private static Analysis.CashFlowDistributionRule Convert(CashFlowDistributionRule source) => new()
        {
            CostCeiling = source.CostCeiling,
            Expression = string.Join('/', source.YearlyPercentages),
        };

        private void Convert(BudgetCondition source, Analysis.InvestmentPlan target)
        {
            var result = target.AddBudgetCondition();

            result.Budget = BudgetByName[source.BudgetName];
            result.Criterion.Expression = source.ConditionExpression;
        }

        private static void Convert(Budget source, Analysis.InvestmentPlan target)
        {
            var result = target.AddBudget();

            result.Name = source.Name;

            foreach (var (resultAmount, sourceAmount) in result.YearlyAmounts.Zip(source.YearlyAmounts))
            {
                resultAmount.Value = sourceAmount;
            }
        }

        private Analysis.CommittedProject Convert(CommittedProject source)
        {
            var result = new Analysis.CommittedProject(AssetByID[source.AssetID], source.Year)
            {
                Budget = BudgetByName[source.NameOfUsableBudget],
                Cost = source.Cost,
                Name = source.Name,
                ShadowForAnyTreatment = source.ShadowForAnyTreatment,
                ShadowForSameTreatment = source.ShadowForSameTreatment,
                treatmentCategory = source.Category,
            };

            foreach (var item in source.Consequences)
            {
                result.Consequences.Add(Convert(item));
            }

            return result;
        }

        private Analysis.TreatmentConsequence Convert(TreatmentConsequence source) => new()
        {
            Attribute = AttributeByName[source.AttributeName],
            Change = { Expression = source.ChangeExpression },
        };

        private void Convert(AnalysisMethod source, Analysis.AnalysisMethod target)
        {
            target.AllowFundingFromMultipleBudgets = source.AllowFundingFromMultipleBudgets;
            target.Benefit.Attribute = NumericAttributeByName[source.BenefitAttributeName];
            target.Benefit.Limit = source.BenefitLimit;
            target.Filter.Expression = source.FilterExpression;
            target.OptimizationStrategy = source.OptimizationStrategy;
            target.ShouldApplyMultipleFeasibleCosts = source.ShouldApplyMultipleFeasibleCosts;
            target.ShouldDeteriorateDuringCashFlow = source.ShouldDeteriorateDuringCashFlow;
            target.ShouldRestrictCashFlowToFirstYearBudgets = source.ShouldRestrictCashFlowToFirstYearBudgets;
            target.SpendingStrategy = source.SpendingStrategy;
            target.Weighting = NumericAttributeByName[source.BenefitWeightAttributeName];

            foreach (var item in source.BudgetPriorities)
            {
                Convert(item, target);
            }

            foreach (var item in source.DeficientConditionGoals)
            {
                Convert(item, target);
            }

            foreach (var item in source.RemainingLifeLimits)
            {
                Convert(item, target);
            }

            foreach (var item in source.TargetConditionGoals)
            {
                Convert(item, target);
            }
        }

        private void Convert(TargetConditionGoal source, Analysis.AnalysisMethod target)
        {
            var result = target.AddTargetConditionGoal();

            result.Attribute = NumericAttributeByName[source.AttributeName];
            result.Criterion.Expression = source.CriterionExpression;
            result.Name = source.Name;
            result.Target = source.Target;
            result.Year = source.Year;
        }

        private void Convert(RemainingLifeLimit source, Analysis.AnalysisMethod target)
        {
            var result = target.AddRemainingLifeLimit();

            result.Attribute = NumericAttributeByName[source.AttributeName];
            result.Criterion.Expression = source.CriterionExpression;
            result.Value = source.Value;
        }

        private void Convert(DeficientConditionGoal source, Analysis.AnalysisMethod target)
        {
            var result = target.AddDeficientConditionGoal();

            result.AllowedDeficientPercentage = source.AllowedDeficientPercentage;
            result.Attribute = NumericAttributeByName[source.AttributeName];
            result.Criterion.Expression = source.CriterionExpression;
            result.DeficientLimit = source.DeficientLimit;
            result.Name = source.Name;
        }

        private Explorer Convert(AttributeSystem source)
        {
            var result = new Explorer(source.AgeAttributeName);

            foreach (var item in source.CalculatedFields)
            {
                Convert(item, result);
            }

            foreach (var item in source.NumberAttributes)
            {
                Convert(item, result);
            }

            foreach (var item in source.TextAttributes)
            {
                Convert(item, result);
            }

            AttributeByName = result.AllAttributes.ToDictionary(_ => _.Name);
            NumericAttributeByName = result.NumericAttributes.ToDictionary(_ => _.Name);
            NumberAttributeByName = result.NumberAttributes.ToDictionary(_ => _.Name);
            TextAttributeByName = result.TextAttributes.ToDictionary(_ => _.Name);

            return result;
        }

        private Analysis.Network Convert(Network source)
        {
            var result = Convert(source.AttributeSystem).AddNetwork();

            result.Name = source.Name;

            foreach (var item in source.MaintainableAssets)
            {
                Convert(item, result);
            }

            AssetByID = result.Assets.ToDictionary(_ => _.Id);

            return result;
        }

        private void Convert(MaintainableAsset source, Analysis.Network target)
        {
            var result = target.AddAsset();

            result.Id = source.ID;
            result.AssetName = source.Name;
            result.SpatialWeighting.Expression = source.SpatialWeightExpression;

            foreach (var item in source.NumberAttributeHistories)
            {
                Convert(item, result, NumberAttributeByName);
            }

            foreach (var item in source.TextAttributeHistories)
            {
                Convert(item, result, TextAttributeByName);
            }
        }
    }

    #endregion
}
