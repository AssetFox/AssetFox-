using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using AppliedResearchAssociates.iAM.Analysis;
using Humanizer;

namespace AppliedResearchAssociates.iAM.Analysis.V1DataAccess
{
    public sealed class V1DataAccessor
    {
        public V1DataAccessor(IDbConnection connection, Action<TimeSpan, string> onProgress)
        {
            Connection = connection ?? throw new ArgumentNullException(nameof(connection));
            OnProgress = onProgress ?? Inaction.Delegate;
        }

        public Dictionary<Network, int> IdPerNetwork { get; } = new Dictionary<Network, int>();

        public Dictionary<Simulation, int> IdPerSimulation { get; } = new Dictionary<Simulation, int>();

        /// <summary>
        ///     When this is null, all networks and all simulations are accessed. Otherwise, only
        ///     networks whose ID is contained will be accessed. For each key-network: if the
        ///     value-set is null, all simulations of that network are accessed; otherwise, only
        ///     simulations whose ID is contained in the value-set will be accessed.
        /// </summary>
        public SortedDictionary<int, SortedSet<int>> RequestedSimulationsPerNetwork { get; set; }

        public Explorer GetExplorer()
        {
            IdPerNetwork.Clear();
            IdPerSimulation.Clear();

            var explorer = new Explorer();
            IDictionary<int, Network> networkPerId;

            using (var command = Connection.CreateCommand())
            {
                command.CommandText = @"
select type_, attribute_, calculated, ascending, default_value, minimum_, maximum
from attributes_

select attribute_, equation, criteria
from attributes_calculated

select networkid, network_name
from networks
order by networkid
";

                using (var reader = command.ExecuteReader())
                {
                    var readerTimer = GetTimer(reader);

                    readerTimer.Time(createAttributes, nameof(createAttributes));
                    readerTimer.Time(fillCalculatedFields, nameof(fillCalculatedFields));
                    networkPerId = readerTimer.Time(createNetworks, nameof(createNetworks));

                    #region Helper functions

                    void createAttributes()
                    {
                        while (reader.Read())
                        {
                            var type = reader.GetNullableString(0);
                            var name = reader.GetNullableString(1);

                            if (name == explorer.AgeAttribute.Name)
                            {
                                if (type != NUMBER_ATTRIBUTE_TYPE_NAME)
                                {
                                    throw new InvalidOperationException("Age attribute must be numeric.");
                                }

                                continue;
                            }

                            switch (type)
                            {
                            case NUMBER_ATTRIBUTE_TYPE_NAME:
                                var isCalculated = reader.GetNullableBoolean(2) ?? false;
                                if (isCalculated)
                                {
                                    var calculatedField = explorer.AddCalculatedField(name);
                                    calculatedField.IsDecreasingWithDeterioration = reader.GetBoolean(3);
                                }
                                else
                                {
                                    var numberAttribute = explorer.AddNumberAttribute(name);
                                    numberAttribute.IsDecreasingWithDeterioration = reader.GetBoolean(3);
                                    numberAttribute.DefaultValue = double.Parse(reader.GetNullableString(4));
                                    numberAttribute.Minimum = reader.GetNullableDouble(5);
                                    numberAttribute.Maximum = reader.GetNullableDouble(6);

                                    /*if (numberAttribute.Minimum > numberAttribute.Maximum)
                                    {
                                        /*var swap = numberAttribute.Minimum;
                                        numberAttribute.Minimum = numberAttribute.Maximum;
                                        numberAttribute.Maximum = swap;#1#
                                        numberAttribute.Maximum = null;
                                    }*/
                                }
                                break;

                            case STRING_ATTRIBUTE_TYPE_NAME:
                                var textAttribute = explorer.AddTextAttribute(name);
                                textAttribute.DefaultValue = reader.GetNullableString(4);
                                break;

                            default:
                                throw new InvalidOperationException($"Invalid attribute type \"{type}\".");
                            }
                        }
                    }

                    void fillCalculatedFields()
                    {
                        var calculatedFieldPerName = explorer.CalculatedFields.ToDictionary(field => field.Name, StringComparer.OrdinalIgnoreCase);

                        while (reader.Read())
                        {
                            var name = reader.GetNullableString(0);
                            if (!calculatedFieldPerName.TryGetValue(name, out var calculatedField))
                            {
                                throw new InvalidOperationException("Unknown calculated field.");
                            }

                            var source = calculatedField.AddValueSource();
                            source.Equation.Expression = reader.GetNullableString(1);
                            source.Criterion.Expression = reader.GetNullableString(2);
                        }
                    }

                    IDictionary<int, Network> createNetworks()
                    {
                        var networks = new SortedList<int, Network>();

                        while (reader.Read())
                        {
                            var networkId = reader.GetInt32(0);
                            if (RequestedSimulationsPerNetwork?.ContainsKey(networkId) ?? true)
                            {
                                var network = explorer.AddNetwork();
                                networks.Add(networkId, network);
                                IdPerNetwork.Add(network, networkId);

                                network.Name = reader.GetNullableString(1);
                            }
                        }

                        return networks;
                    }

                    #endregion Helper functions
                }
            }

            var helper = new DataHelper
            {
                AllAttributeNames = explorer.AllAttributes.Select(attribute => attribute.Name).ToHashSet(StringComparer.OrdinalIgnoreCase),
                AttributePerName = explorer.AllAttributes.ToDictionary(attribute => attribute.Name, StringComparer.OrdinalIgnoreCase),
                NumberAttributePerName = explorer.NumberAttributes.ToDictionary(attribute => attribute.Name, StringComparer.OrdinalIgnoreCase),
                NumericAttributePerName = explorer.NumericAttributes.ToDictionary(attribute => attribute.Name, StringComparer.OrdinalIgnoreCase),
            };

            var timer = GetTimer();

            foreach (var (networkId, network) in networkPerId)
            {
                timer.Time(() => FillNetwork(network, networkId, helper), $"Fill network {networkId}");
            }

            return explorer;
        }

        private const string NUMBER_ATTRIBUTE_TYPE_NAME = "NUMBER";
        private const string STRING_ATTRIBUTE_TYPE_NAME = "STRING";

        private readonly IDbConnection Connection;
        private readonly Action<TimeSpan, string> OnProgress;

        private void FillNetwork(Network network, int networkId, DataHelper helper)
        {
            IDictionary<int, Simulation> simulationPerId;

            using (var command = Connection.CreateCommand())
            {
                command.CommandText = $@"
select facility, section, area, units, sectionid
from section_{networkId}
order by sectionid

select *
from segment_{networkId}_ns0

select simulationid, simulation, jurisdiction, analysis, budget_constraint, weighting, benefit_variable, benefit_limit, use_cumulative_cost, use_across_budget
from simulations
where networkid = {networkId}
order by simulationid
";

                using (var reader = command.ExecuteReader())
                {
                    var readerTimer = GetTimer(reader);

                    helper.AssetPerSectionId = new Dictionary<int, AnalysisMaintainableAsset>();

                    readerTimer.Time(createAssets, nameof(createAssets));
                    readerTimer.Time(fillAssetHistories, nameof(fillAssetHistories));
                    simulationPerId = readerTimer.Time(createSimulations, nameof(createSimulations));

                    #region Helper functions

                    void createAssets()
                    {
                        string areaUnit = null;

                        while (reader.Read())
                        {
                            var facilityName = reader.GetNullableString(0);
                            var sectionName = reader.GetNullableString(1);
                            var assetName = $"{facilityName}/{sectionName}";

                            if (network.Assets.All(asset => asset.AssetName != assetName))
                            {
                                var asset = network.AddAsset();
                                asset.AssetName = assetName;
                                asset.SpatialWeighting.Expression = reader.GetDouble(2).ToString();

                                var assetAreaUnit = reader.GetNullableString(3)?.Trim();
                                if (areaUnit is null)
                                {
                                    areaUnit = assetAreaUnit;
                                }
                                else if (!areaUnit.Equals(assetAreaUnit, StringComparison.OrdinalIgnoreCase))
                                {
                                    throw new InvalidOperationException("Assets of network have inconsistent area units.");
                                }

                                var sectionId = reader.GetInt32(4);
                                helper.AssetPerSectionId.Add(sectionId, asset);
                            }
                        }

                        network.SpatialWeightUnit = areaUnit;
                    }

                    void fillAssetHistories()
                    {
                        const string SECTIONID_COLUMN_NAME = "sectionid";

                        var columnSchema = Enumerable.Range(0, reader.FieldCount)
                            .Select(columnIndex => new DbColumn(reader.GetName(columnIndex), columnIndex))
                            .ToArray();

                        var sectionidColumnIndex = columnSchema.Single(column => StringComparer.OrdinalIgnoreCase.Equals(column.ColumnName, SECTIONID_COLUMN_NAME)).ColumnOrdinal;

                        var latestValueColumnPerAttribute = columnSchema
                            .Where(column => helper.AllAttributeNames.Contains(column.ColumnName))
                            .ToDictionary(column => column.ColumnName, column => column.ColumnOrdinal);

                        var historyColumnsPerAttribute = columnSchema
                            .Where(column => !StringComparer.OrdinalIgnoreCase.Equals(column.ColumnName, SECTIONID_COLUMN_NAME) && !helper.AllAttributeNames.Contains(column.ColumnName))
                            .Select(column =>
                            {
                                var columnOrdinal = column.ColumnOrdinal;
                                var yearSeparatorIndex = column.ColumnName.LastIndexOf('_');
                                var yearString = column.ColumnName.Substring(yearSeparatorIndex + 1);
                                var year = int.Parse(yearString);
                                var attributeName = column.ColumnName.Substring(0, yearSeparatorIndex);
                                return (columnOrdinal, year, attributeName);
                            })
                            .ToLookup(columnDatum => columnDatum.attributeName);

                        while (reader.Read())
                        {
                            // With respect to memory footprint, it may be worthwhile to abbreviate
                            // (where possible) the full history of each attribute in each asset,
                            // at least for the purpose of analysis. The analysis requires the
                            // historical values only for roll-forward, and roll-forward does not
                            // require all history before a certain point. (See roll-forward logic
                            // for exact details. At the very least, it requires all history back to
                            // the earliest most-recent year.) Attributes with no history should be
                            // ignored if applying this logic.

                            var sectionId = reader.GetInt32(sectionidColumnIndex);
                            if (helper.AssetPerSectionId.TryGetValue(sectionId, out var asset))
                            {
                                fillHistories(network.Explorer.NumberAttributes, reader.GetDouble);
                                fillHistories(network.Explorer.TextAttributes, reader.GetNullableString);

                                void fillHistories<T>(IEnumerable<Attribute<T>> attributes, Func<int, T> getValue)
                                {
                                    foreach (var attribute in attributes)
                                    {
                                        var history = asset.GetHistory(attribute);

                                        foreach (var (historyColumnOrdinal, year, _) in historyColumnsPerAttribute[attribute.Name])
                                        {
                                            if (!reader.IsDBNull(historyColumnOrdinal))
                                            {
                                                var value = getValue(historyColumnOrdinal);
                                                history.Add(year, value);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    IDictionary<int, Simulation> createSimulations()
                    {
                        var simulations = new SortedList<int, Simulation>();

                        SortedSet<int> requestedSimulations = null;

                        if (RequestedSimulationsPerNetwork?.TryGetValue(networkId, out requestedSimulations) ?? true)
                        {
                            while (reader.Read())
                            {
                                var simulationId = reader.GetInt32(0);
                                if (requestedSimulations?.Contains(simulationId) ?? true)
                                {
                                    var simulation = network.AddSimulation();
                                    simulations.Add(simulationId, simulation);
                                    IdPerSimulation.Add(simulation, simulationId);

                                    simulation.Name = reader.GetNullableString(1);
                                    simulation.AnalysisMethod.Filter.Expression = reader.GetNullableString(2);

                                    var optimizationStrategyLabel = reader.GetNullableString(3);
                                    simulation.AnalysisMethod.OptimizationStrategy = OptimizationStrategyLookup.Instance[optimizationStrategyLabel];

                                    var spendingStrategyLabel = reader.GetNullableString(4);
                                    simulation.AnalysisMethod.SpendingStrategy = SpendingStrategyLookup.Instance[spendingStrategyLabel];

                                    var weightingName = reader.GetNullableString(5);
                                    if (helper.NumericAttributePerName.TryGetValue(weightingName, out var weighting))
                                    {
                                        simulation.AnalysisMethod.Weighting = weighting;
                                    }

                                    var benefitName = reader.GetNullableString(6);
                                    if (benefitName is object && helper.NumericAttributePerName.TryGetValue(benefitName, out var benefit))
                                    {
                                        simulation.AnalysisMethod.Benefit.Attribute = benefit;
                                    }

                                    simulation.AnalysisMethod.Benefit.Limit = reader.GetDouble(7);
                                    simulation.AnalysisMethod.ShouldApplyMultipleFeasibleCosts = reader.GetNullableBoolean(8) ?? false;
                                    simulation.AnalysisMethod.ShouldUseExtraFundsAcrossBudgets = reader.GetNullableBoolean(9) ?? false;
                                }
                            }
                        }

                        return simulations;
                    }

                    #endregion Helper functions
                }
            }

            var timer = GetTimer();

            foreach (var (simulationId, simulation) in simulationPerId)
            {
                timer.Time(() => FillSimulation(simulation, simulationId, helper), $"Fill simulation {simulationId}");
            }
        }

        private void FillSimulation(Simulation simulation, int simulationId, DataHelper helper)
        {
            using (var command = Connection.CreateCommand())
            {
                command.CommandText = $@"
select firstyear, numberyears, inflationrate, discountrate, budgetorder, minimum_project
from investments
where simulationid = {simulationId}

select attribute_, equationname, criteria, equation, shift
from performance
where simulationid = {simulationId}

select c.commitid, sectionid, years, treatmentname, yearsame, yearany, budget, cost_, attribute_, change_
from committed_ c join commit_consequences cc on c.commitid = cc.commitid
where simulationid = {simulationId}

select t.treatmentid, treatment, beforeany, beforesame, budget, description, attribute_, change_, equation, criteria
from treatments t join consequences c on t.treatmentid = c.treatmentid
where simulationid = {simulationId}

select t.treatmentid, cost_, criteria
from treatments t join costs c on t.treatmentid = c.treatmentid
where simulationid = {simulationId}

select t.treatmentid, criteria
from treatments t join feasibility f on t.treatmentid = f.treatmentid
where simulationid = {simulationId}

select t.treatmentid, scheduledyear, scheduledtreatmentid
from treatments t join scheduled s on t.treatmentid = s.treatmentid
where simulationid = {simulationId}

select t.treatmentid, supersede_treatment_id, criteria
from treatments t join supersedes s on t.treatmentid = s.treatment_id
where simulationid = {simulationId}

select p.priorityid, prioritylevel, criteria, years, budget, funding
from priority p join priorityfund pf on p.priorityid = pf.priorityid
where simulationid = {simulationId}

select attribute_, years, targetmean, targetname, criteria
from targets
where simulationid = {simulationId}

select attribute_, deficientname, deficient, percentdeficient, criteria
from deficients
where simulationid = {simulationId}

select attribute_, remaining_life_limit, criteria
from remaining_life_limits
where simulation_id = {simulationId}

select budgetname, year_, amount
from yearlyinvestment
where simulationid = {simulationId}

select budget_name, criteria
from budget_criteria
where simulationid = {simulationId}

select st.split_treatment_id, description, criteria, amount, percentage
from split_treatment st join split_treatment_limit stl on st.split_treatment_id = stl.split_treatment_id
where simulationid = {simulationId}
";

                using (var reader = command.ExecuteReader())
                {
                    var treatmentPerId = new Dictionary<int, SelectableTreatment>();

                    var readerTimer = GetTimer(reader);

                    readerTimer.Time(fillInvestmentPlan, nameof(fillInvestmentPlan));

                    var budgetPerName = simulation.InvestmentPlan.Budgets.ToDictionary(budget => budget.Name, SelectionEqualityComparer<string>.Create(name => name.Trim()));

                    readerTimer.Time(createPerformanceCurves, nameof(createPerformanceCurves));
                    readerTimer.Time(createCommittedProjects, nameof(createCommittedProjects));
                    readerTimer.Time(createTreatmentsWithConsequences, nameof(createTreatmentsWithConsequences));
                    readerTimer.Time(fillTreatmentCosts, nameof(fillTreatmentCosts));
                    readerTimer.Time(fillTreatmentFeasibilities, nameof(fillTreatmentFeasibilities));
                    readerTimer.Time(fillTreatmentSchedulings, nameof(fillTreatmentSchedulings));
                    readerTimer.Time(fillTreatmentSupersessions, nameof(fillTreatmentSupersessions));
                    readerTimer.Time(createBudgetPriorities, nameof(createBudgetPriorities));
                    readerTimer.Time(createTargetConditionGoals, nameof(createTargetConditionGoals));
                    readerTimer.Time(createDeficientConditionGoals, nameof(createDeficientConditionGoals));
                    readerTimer.Time(createRemainingLifeLimits, nameof(createRemainingLifeLimits));
                    readerTimer.Time(fillBudgetAmounts, nameof(fillBudgetAmounts));
                    readerTimer.Time(createBudgetConditions, nameof(createBudgetConditions));
                    readerTimer.Time(createCashFlowRules, nameof(createCashFlowRules));

                    foreach (var rule in simulation.InvestmentPlan.CashFlowRules)
                    {
                        if (rule.DistributionRules.Count > 0)
                        {
                            rule.DistributionRules.OrderBy(d => d.CostCeiling ?? decimal.MaxValue).Last().CostCeiling = null;
                        }
                    }

                    const string PASSIVE_TREATMENT_NAME = "No Treatment";
                    simulation.Treatments.Single(treatment => StringComparer.OrdinalIgnoreCase.Equals(treatment.Name.Trim(), PASSIVE_TREATMENT_NAME)).DesignateAsPassiveForSimulation();

                    #region Helper functions

                    void fillInvestmentPlan()
                    {
                        if (!reader.Read())
                        {
                            throw new InvalidOperationException("Invalid simulation ID.");
                        }

                        simulation.InvestmentPlan.FirstYearOfAnalysisPeriod = reader.GetInt32(0);
                        simulation.InvestmentPlan.NumberOfYearsInAnalysisPeriod = reader.GetInt32(1);
                        simulation.InvestmentPlan.InflationRatePercentage = reader.GetDouble(2);
                        simulation.InvestmentPlan.DiscountRatePercentage = reader.GetDouble(3);

                        var budgetOrder = reader.GetNullableString(4);
                        var budgetNamesInOrder = budgetOrder.Split(',');
                        foreach (var budgetName in budgetNamesInOrder)
                        {
                            var budget = simulation.InvestmentPlan.AddBudget();
                            budget.Name = budgetName;
                        }

                        var minimumProjectCostLimit = reader.GetNullableDouble(5);
                        simulation.InvestmentPlan.MinimumProjectCostLimit = (decimal)(minimumProjectCostLimit ?? default);
                    }

                    void createPerformanceCurves()
                    {
                        while (reader.Read())
                        {
                            var curve = simulation.AddPerformanceCurve();
                            var attributeName = reader.GetNullableString(0);
                            curve.Attribute = helper.NumberAttributePerName[attributeName];
                            curve.Name = reader.GetNullableString(1) ?? $"{curve.Attribute.Name} Performance Curve";
                            curve.Criterion.Expression = reader.GetNullableString(2);
                            curve.Equation.Expression = reader.GetNullableString(3);
                            curve.Shift = reader.GetBoolean(4);
                        }
                    }

                    void createCommittedProjects()
                    {
                        var projectPerId = new Dictionary<int, CommittedProject>();

                        while (reader.Read())
                        {
                            var id = reader.GetInt32(0);
                            if (!projectPerId.TryGetValue(id, out var project))
                            {
                                var budgetName = reader.GetNullableString(6);
                                if (budgetPerName.TryGetValue(budgetName, out var budget))
                                {
                                    var sectionId = reader.GetInt32(1);
                                    var asset = helper.AssetPerSectionId[sectionId];
                                    var year = reader.GetInt32(2);
                                    project = simulation.CommittedProjects.GetAdd(new CommittedProject(asset, year));
                                    project.Name = reader.GetNullableString(3);
                                    project.ShadowForSameTreatment = reader.GetInt32(4);
                                    project.ShadowForAnyTreatment = reader.GetInt32(5);
                                    project.Budget = budget;
                                    project.Cost = reader.GetDouble(7);

                                    projectPerId.Add(id, project);
                                }
                                else
                                {
                                    continue;
                                }
                            }

                            var consequence = project.Consequences.GetAdd(new TreatmentConsequence());
                            var attributeName = reader.GetNullableString(8);
                            consequence.Attribute = helper.AttributePerName[attributeName];
                            consequence.Change.Expression = reader.GetNullableString(9);
                        }
                    }

                    void createTreatmentsWithConsequences()
                    {
                        while (reader.Read())
                        {
                            var id = reader.GetInt32(0);
                            if (!treatmentPerId.TryGetValue(id, out var treatment))
                            {
                                treatment = simulation.AddTreatment();
                                treatment.Name = reader.GetNullableString(1);
                                treatment.ShadowForAnyTreatment = reader.GetInt32(2);
                                treatment.ShadowForSameTreatment = reader.GetInt32(3);

                                var budgetField = reader.GetNullableString(4);
                                var budgetNames = budgetField?.Split(',') ?? Array.Empty<string>();
                                foreach (var budgetName in budgetNames)
                                {
                                    if (budgetPerName.TryGetValue(budgetName, out var budget))
                                    {
                                        treatment.Budgets.Add(budget);
                                    }
                                }

                                treatment.Description = reader.GetNullableString(5);

                                treatmentPerId.Add(id, treatment);
                            }

                            var consequence = treatment.AddConsequence();
                            var attributeName = reader.GetNullableString(6);
                            consequence.Attribute = helper.AttributePerName[attributeName];
                            consequence.Change.Expression = reader.GetNullableString(7);
                            consequence.Equation.Expression = reader.GetNullableString(8);
                            consequence.Criterion.Expression = reader.GetNullableString(9);
                        }
                    }

                    void fillTreatmentCosts()
                    {
                        while (reader.Read())
                        {
                            var id = reader.GetInt32(0);
                            var treatment = treatmentPerId[id];
                            var cost = treatment.AddCost();
                            cost.Equation.Expression = reader.GetNullableString(1);
                            cost.Criterion.Expression = reader.GetNullableString(2);
                        }
                    }

                    // treatments in the new schema can only ever have one criterion library; so need to merge all legacy feasibility criteria into one
                    void fillTreatmentFeasibilities()
                    {
                        var expressionsPerTreatmentId = new Dictionary<int, List<string>>();
                        while (reader.Read())
                        {
                            var id = reader.GetInt32(0);
                            if (!expressionsPerTreatmentId.ContainsKey(id))
                            {
                                expressionsPerTreatmentId.Add(id, new List<string>());
                            }

                            var expression = reader.GetNullableString(1);
                            if (!string.IsNullOrEmpty(expression))
                            {
                                expressionsPerTreatmentId[id].Add(expression);
                            }
                        }

                        foreach (var treatmentId in expressionsPerTreatmentId.Keys)
                        {
                            if (treatmentPerId.TryGetValue(treatmentId, out var treatment))
                            {
                                var feasibility = treatment.AddFeasibilityCriterion();
                                feasibility.Expression = expressionsPerTreatmentId[treatmentId].Any()
                                    ? $"({string.Join(") OR (", expressionsPerTreatmentId[treatmentId])})"
                                    : string.Empty;
                            }
                        }
                    }

                    void fillTreatmentSchedulings()
                    {
                        while (reader.Read())
                        {
                            var id = reader.GetInt32(0);
                            var treatment = treatmentPerId[id];
                            var scheduling = treatment.Schedulings.GetAdd(new TreatmentScheduling());
                            scheduling.OffsetToFutureYear = reader.GetInt32(1);
                            var scheduledId = reader.GetInt32(2);
                            scheduling.Treatment = treatmentPerId[scheduledId];
                        }
                    }

                    void fillTreatmentSupersessions()
                    {
                        while (reader.Read())
                        {
                            var id = reader.GetInt32(0);
                            var treatment = treatmentPerId[id];
                            var supersession = treatment.AddSupersession();
                            var supersededId = reader.GetInt32(1);
                            supersession.Treatment = treatmentPerId[supersededId];
                            supersession.Criterion.Expression = reader.GetNullableString(2);
                        }
                    }

                    void createBudgetPriorities()
                    {
                        var priorityPerId = new Dictionary<int, BudgetPriority>();

                        while (reader.Read())
                        {
                            var id = reader.GetInt32(0);
                            if (!priorityPerId.TryGetValue(id, out var priority))
                            {
                                priority = simulation.AnalysisMethod.AddBudgetPriority();
                                priority.PriorityLevel = reader.GetInt32(1);
                                priority.Criterion.Expression = reader.GetNullableString(2);
                                priority.Year = reader.GetNullableInt32(3);

                                if (priority.Year == 0)
                                {
                                    priority.Year = null;
                                }

                                priorityPerId.Add(id, priority);
                            }

                            var budgetName = reader.GetNullableString(4);
                            if (budgetPerName.TryGetValue(budgetName, out var budget))
                            {
                                var pair = priority.GetBudgetPercentagePair(budget);
                                pair.Percentage = (decimal)reader.GetDouble(5);
                            }
                        }
                    }

                    void createTargetConditionGoals()
                    {
                        while (reader.Read())
                        {
                            var goal = simulation.AnalysisMethod.AddTargetConditionGoal();
                            var attributeName = reader.GetNullableString(0);
                            goal.Attribute = helper.NumericAttributePerName[attributeName];
                            goal.Year = reader.GetNullableInt32(1);
                            goal.Target = reader.GetDouble(2);
                            goal.Name = reader.GetNullableString(3) ?? $"{goal.Attribute.Name} Target Condition Goal";
                            goal.Criterion.Expression = reader.GetNullableString(4);
                        }
                    }

                    void createDeficientConditionGoals()
                    {
                        while (reader.Read())
                        {
                            var goal = simulation.AnalysisMethod.AddDeficientConditionGoal();
                            var attributeName = reader.GetNullableString(0);
                            goal.Attribute = helper.NumericAttributePerName[attributeName];
                            goal.Name = reader.GetNullableString(1) ?? $"{goal.Attribute.Name} Deficient Condition Goal";
                            goal.DeficientLimit = reader.GetDouble(2);
                            goal.AllowedDeficientPercentage = reader.GetDouble(3);
                            goal.Criterion.Expression = reader.GetNullableString(4);
                        }
                    }

                    void createRemainingLifeLimits()
                    {
                        while (reader.Read())
                        {
                            var limit = simulation.AnalysisMethod.AddRemainingLifeLimit();
                            var attributeName = reader.GetNullableString(0);
                            limit.Attribute = helper.NumericAttributePerName[attributeName];
                            limit.Value = reader.GetDouble(1);
                            limit.Criterion.Expression = reader.GetNullableString(2);
                        }
                    }

                    void fillBudgetAmounts()
                    {
                        while (reader.Read())
                        {
                            var budgetName = reader.GetNullableString(0);
                            var budget = budgetPerName[budgetName];
                            var year = reader.GetInt32(1);
                            var yearOffset = year - simulation.InvestmentPlan.FirstYearOfAnalysisPeriod;
                            budget.YearlyAmounts[yearOffset].Value = (decimal)reader.GetDouble(2);
                        }
                    }

                    void createBudgetConditions()
                    {
                        while (reader.Read())
                        {
                            var budgetName = reader.GetNullableString(0);
                            var budget = budgetPerName[budgetName];
                            var criterion = reader.GetNullableString(1);
                            var budgetCondition = simulation.InvestmentPlan.AddBudgetCondition();
                            budgetCondition.Budget = budget;
                            budgetCondition.Criterion.Expression = criterion;
                        }
                    }

                    void createCashFlowRules()
                    {
                        var rulePerId = new Dictionary<int, CashFlowRule>();

                        while (reader.Read())
                        {
                            var id = reader.GetInt32(0);
                            if (!rulePerId.TryGetValue(id, out var rule))
                            {
                                rule = simulation.InvestmentPlan.AddCashFlowRule();
                                rule.Name = reader.GetNullableString(1) ?? $"{simulation.Name} Cash Flow Rule";
                                rule.Criterion.Expression = reader.GetNullableString(2);

                                rulePerId.Add(id, rule);
                            }

                            var distributionRule = rule.DistributionRules.GetAdd(new CashFlowDistributionRule());
                            distributionRule.CostCeiling = (decimal)reader.GetDouble(3);
                            distributionRule.Expression = reader.GetNullableString(4);
                        }
                    }

                    #endregion Helper functions
                }
            }
        }

        private Timer GetTimer() => new Timer(OnProgress);

        private ReaderTimer GetTimer(IDataReader reader) => new ReaderTimer(OnProgress, reader);

        private sealed class DataHelper
        {
            public ISet<string> AllAttributeNames { get; set; }

            public IDictionary<string, Attribute> AttributePerName { get; set; }

            public IDictionary<string, NumberAttribute> NumberAttributePerName { get; set; }

            public IDictionary<string, INumericAttribute> NumericAttributePerName { get; set; }

            public IDictionary<int, AnalysisMaintainableAsset> AssetPerSectionId { get; set; }
        }

        private sealed class DbColumn
        {
            public DbColumn(string columnName, int columnOrdinal)
            {
                ColumnName = columnName;
                ColumnOrdinal = columnOrdinal;
            }

            public string ColumnName { get; }

            public int ColumnOrdinal { get; }
        }

        private sealed class ReaderTimer : Timer
        {
            public ReaderTimer(Action<TimeSpan, string> onProgress, IDataReader reader) : base(onProgress) => Reader = reader;

            protected override void OnTimed() => _ = Reader.NextResult();

            private readonly IDataReader Reader;
        }

        private class Timer
        {
            public Timer(Action<TimeSpan, string> onProgress) => OnProgress = onProgress;

            public void Time(Action action, string label)
            {
                _ = Time(actionAsFunc, label);

                object actionAsFunc()
                {
                    action();
                    return null;
                }
            }

            public T Time<T>(Func<T> func, string label)
            {
                Stopwatch.Restart();
                var result = func();
                var elapsed = Stopwatch.Elapsed;
                OnProgress?.Invoke(elapsed, label.Humanize() + ".");
                OnTimed();
                return result;
            }

            protected virtual void OnTimed()
            {
            }

            private readonly Action<TimeSpan, string> OnProgress;
            private readonly Stopwatch Stopwatch = new Stopwatch();
        }
    }
}
