using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using Humanizer;

namespace AppliedResearchAssociates.iAM.DataAccess
{
    public sealed class DataAccessor
    {
        public Simulation GetStandAloneSimulation(IDbConnection connection, int networkId, int simulationId, Action<TimeSpan, string> onProgress = null)
        {
            var formattedCommandText = $@"
select type_, attribute_, calculated, ascending, default_value, minimum_, maximum
from attributes_

select attribute_, equation, criteria
from attributes_calculated

select network_name
from networks
where networkid = {networkId}

select facility, section, area, units, sectionid
from section_{networkId}
order by sectionid

select *
from segment_{networkId}_ns0

select simulation, jurisdiction, analysis, budget_constraint, weighting, benefit_variable, benefit_limit, use_cumulative_cost, use_across_budget
from simulations
where simulationid = {simulationId}

select firstyear, numberyears, inflationrate, discountrate, budgetorder
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

            using (var command = connection.CreateCommand())
            {
                command.CommandText = formattedCommandText;

                using (var reader = command.ExecuteReader())
                {
                    var timer = new Stopwatch();
                    void time(Action action, string label)
                    {
                        timer.Restart();
                        action();
                        var elapsed = timer.Elapsed;
                        onProgress?.Invoke(elapsed, label.Humanize() + ".");
                        //onProgress?.Invoke(elapsed, label + ".");
                        _ = reader.NextResult();
                    }

                    var sectionById = new Dictionary<int, Section>();
                    var treatmentById = new Dictionary<int, SelectableTreatment>();

                    var simulation = new Explorer().AddNetwork().AddSimulation();

                    time(createAttributes, nameof(createAttributes));
                    time(fillCalculatedFields, nameof(fillCalculatedFields));
                    time(fillNetwork, nameof(fillNetwork));
                    time(createSections, nameof(createSections));
                    time(fillSectionHistories, nameof(fillSectionHistories));
                    time(fillAnalysisMethod, nameof(fillAnalysisMethod));
                    time(fillInvestmentPlan, nameof(fillInvestmentPlan));
                    time(createPerformanceCurves, nameof(createPerformanceCurves));
                    time(createCommittedProjects, nameof(createCommittedProjects));
                    time(createTreatmentsWithConsequences, nameof(createTreatmentsWithConsequences));
                    time(fillTreatmentCosts, nameof(fillTreatmentCosts));
                    time(fillTreatmentFeasibilities, nameof(fillTreatmentFeasibilities));
                    time(fillTreatmentSchedulings, nameof(fillTreatmentSchedulings));
                    time(fillTreatmentSupersessions, nameof(fillTreatmentSupersessions));
                    time(createBudgetPriorities, nameof(createBudgetPriorities));
                    time(createTargetConditionGoals, nameof(createTargetConditionGoals));
                    time(createDeficientConditionGoals, nameof(createDeficientConditionGoals));
                    time(createRemainingLifeLimits, nameof(createRemainingLifeLimits));
                    time(fillBudgetAmounts, nameof(fillBudgetAmounts));
                    time(createBudgetConditions, nameof(createBudgetConditions));
                    time(createCashFlowRules, nameof(createCashFlowRules));

                    foreach (var rule in simulation.InvestmentPlan.CashFlowRules)
                    {
                        if (rule.DistributionRules.Count > 0)
                        {
                            rule.DistributionRules.OrderBy(d => d.CostCeiling ?? decimal.MaxValue).Last().CostCeiling = null;
                        }
                    }

                    simulation.Treatments.Single(treatment => StringComparer.OrdinalIgnoreCase.Equals(treatment.Name.Trim(), PASSIVE_TREATMENT_NAME)).DesignateAsPassiveForSimulation();

                    return simulation;

                    #region Helper functions

                    void createAttributes()
                    {
                        while (reader.Read())
                        {
                            var type = reader.GetNullableString(0);
                            var name = reader.GetNullableString(1);

                            if (name == simulation.Network.Explorer.AgeAttribute.Name)
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
                                    var calculatedField = simulation.Network.Explorer.AddCalculatedField(name);
                                    calculatedField.IsDecreasingWithDeterioration = reader.GetBoolean(3);
                                }
                                else
                                {
                                    var numberAttribute = simulation.Network.Explorer.AddNumberAttribute(name);
                                    numberAttribute.IsDecreasingWithDeterioration = reader.GetBoolean(3);
                                    numberAttribute.DefaultValue = double.Parse(reader.GetNullableString(4));
                                    numberAttribute.Minimum = reader.GetNullableDouble(5);
                                    numberAttribute.Maximum = reader.GetNullableDouble(6);

                                    if (numberAttribute.Minimum > numberAttribute.Maximum)
                                    {
                                        var swap = numberAttribute.Minimum;
                                        numberAttribute.Minimum = numberAttribute.Maximum;
                                        numberAttribute.Maximum = swap;
                                    }
                                }
                                break;

                            case STRING_ATTRIBUTE_TYPE_NAME:
                                var textAttribute = simulation.Network.Explorer.AddTextAttribute(name);
                                textAttribute.DefaultValue = reader.GetNullableString(4);
                                break;

                            default:
                                throw new InvalidOperationException($"Invalid attribute type \"{type}\".");
                            }
                        }
                    }

                    void fillCalculatedFields()
                    {
                        var calculatedFieldByName = simulation.Network.Explorer.CalculatedFields.ToDictionary(field => field.Name, StringComparer.OrdinalIgnoreCase);

                        while (reader.Read())
                        {
                            var name = reader.GetNullableString(0);
                            if (!calculatedFieldByName.TryGetValue(name, out var calculatedField))
                            {
                                throw new InvalidOperationException("Unknown calculated field.");
                            }

                            var source = calculatedField.AddValueSource();
                            source.Equation.Expression = reader.GetNullableString(1);
                            source.Criterion.Expression = reader.GetNullableString(2);
                        }
                    }

                    void fillNetwork()
                    {
                        if (!reader.Read())
                        {
                            throw new InvalidOperationException("Invalid network ID.");
                        }

                        simulation.Network.Name = reader.GetNullableString(0);
                    }

                    void createSections()
                    {
                        var facilityByName = new Dictionary<string, Facility>();

                        while (reader.Read())
                        {
                            var facilityName = reader.GetNullableString(0);
                            if (!facilityByName.TryGetValue(facilityName, out var facility))
                            {
                                facility = simulation.Network.AddFacility();
                                facility.Name = facilityName;
                                facilityByName.Add(facilityName, facility);
                            }

                            var sectionName = reader.GetNullableString(1);
                            if (facility.Sections.All(section => section.Name != sectionName))
                            {
                                var section = facility.AddSection();
                                section.Name = sectionName;
                                section.Area = reader.GetDouble(2);
                                section.AreaUnit = reader.GetNullableString(3);

                                var sectionId = reader.GetInt32(4);
                                sectionById.Add(sectionId, section);
                            }
                        }
                    }

                    void fillSectionHistories()
                    {
                        var attributeNames = simulation.Network.Explorer.AllAttributes.Select(attribute => attribute.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);

                        var columnSchema = Enumerable.Range(0, reader.FieldCount)
                            .Select(columnIndex => new DbColumn(reader.GetName(columnIndex), columnIndex))
                            .ToArray();

                        var sectionidColumnIndex = columnSchema.Single(column => StringComparer.OrdinalIgnoreCase.Equals(column.ColumnName, SECTIONID_COLUMN_NAME)).ColumnOrdinal;

                        var latestValueColumnPerAttribute = columnSchema
                            .Where(column => attributeNames.Contains(column.ColumnName))
                            .ToDictionary(column => column.ColumnName, column => column.ColumnOrdinal);

                        var historyColumnsPerAttribute = columnSchema
                            .Where(column => !StringComparer.OrdinalIgnoreCase.Equals(column.ColumnName, SECTIONID_COLUMN_NAME) && !attributeNames.Contains(column.ColumnName))
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
                            // (where possible) the full history of each attribute in each section,
                            // at least for the purpose of analysis. The analysis requires the
                            // historical values only for roll-forward, and roll-forward does not
                            // require all history before a certain point. (See roll-forward logic
                            // for exact details. At the very least, it requires all history back to
                            // the earliest most-recent year.) Attributes with no history should be
                            // ignored if applying this logic.

                            var sectionId = reader.GetInt32(sectionidColumnIndex);
                            if (sectionById.TryGetValue(sectionId, out var section))
                            {
                                fillHistories(simulation.Network.Explorer.NumberAttributes, reader.GetDouble);
                                fillHistories(simulation.Network.Explorer.TextAttributes, reader.GetNullableString);

                                void fillHistories<T>(IEnumerable<Attribute<T>> attributes, Func<int, T> getValue)
                                {
                                    foreach (var attribute in attributes)
                                    {
                                        var history = section.GetHistory(attribute);

                                        var latestValueColumnOrdinal = latestValueColumnPerAttribute[attribute.Name];

                                        if (!reader.IsDBNull(latestValueColumnOrdinal))
                                        {
                                            history.MostRecentValue = getValue(latestValueColumnOrdinal);
                                        }

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

                    void fillAnalysisMethod()
                    {
                        if (!reader.Read())
                        {
                            throw new InvalidOperationException("Invalid simulation ID.");
                        }

                        var attributeByName = simulation.Network.Explorer.NumericAttributes.ToDictionary(attribute => attribute.Name, StringComparer.OrdinalIgnoreCase);

                        simulation.Name = reader.GetNullableString(0);
                        simulation.AnalysisMethod.Filter.Expression = reader.GetNullableString(1);

                        var optimizationStrategyLabel = reader.GetNullableString(2);
                        simulation.AnalysisMethod.OptimizationStrategy = OptimizationStrategyLookup.Instance[optimizationStrategyLabel];

                        var spendingStrategyLabel = reader.GetNullableString(3);
                        simulation.AnalysisMethod.SpendingStrategy = SpendingStrategyLookup.Instance[spendingStrategyLabel];

                        var weightingName = reader.GetNullableString(4);
                        if (attributeByName.TryGetValue(weightingName, out var weighting))
                        {
                            simulation.AnalysisMethod.Weighting = weighting;
                        }

                        var benefitName = reader.GetNullableString(5);
                        if (attributeByName.TryGetValue(benefitName, out var benefit))
                        {
                            simulation.AnalysisMethod.Benefit.Attribute = benefit;
                        }

                        simulation.AnalysisMethod.Benefit.Limit = reader.GetDouble(6);
                        simulation.AnalysisMethod.ShouldApplyMultipleFeasibleCosts = reader.GetNullableBoolean(7) ?? false;
                        simulation.AnalysisMethod.ShouldUseExtraFundsAcrossBudgets = reader.GetNullableBoolean(8) ?? false;
                    }

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
                    }

                    void createPerformanceCurves()
                    {
                        var attributeByName = simulation.Network.Explorer.NumberAttributes.ToDictionary(attribute => attribute.Name);

                        while (reader.Read())
                        {
                            var curve = simulation.AddPerformanceCurve();
                            var attributeName = reader.GetNullableString(0);
                            curve.Attribute = attributeByName[attributeName];
                            curve.Name = reader.GetNullableString(1);
                            curve.Criterion.Expression = reader.GetNullableString(2);
                            curve.Equation.Expression = reader.GetNullableString(3);
                            curve.Shift = reader.GetBoolean(4);
                        }
                    }

                    void createCommittedProjects()
                    {
                        // per Jake 2020-07-30, ignore committed projects for now.
                        return;

                        var attributeByName = simulation.Network.Explorer.NumberAttributes.ToDictionary(attribute => attribute.Name);
                        var budgetByName = simulation.InvestmentPlan.Budgets.ToDictionary(budget => budget.Name);
                        var projectById = new Dictionary<int, CommittedProject>();

                        while (reader.Read())
                        {
                            var id = reader.GetInt32(0);
                            if (!projectById.TryGetValue(id, out var project))
                            {
                                var sectionId = reader.GetInt32(1);
                                var section = sectionById[sectionId];
                                var year = reader.GetInt32(2);
                                project = simulation.CommittedProjects.GetAdd(new CommittedProject(section, year)); // maybe modify this so that the project is ignored if the budget name is not found.
                                project.Name = reader.GetNullableString(3);
                                project.ShadowForSameTreatment = reader.GetInt32(4);
                                project.ShadowForAnyTreatment = reader.GetInt32(5);
                                var budgetName = reader.GetNullableString(6);
                                project.Budget = budgetByName[budgetName];
                                project.Cost = reader.GetDouble(7);

                                projectById.Add(id, project);
                            }

                            var consequence = project.Consequences.GetAdd(new TreatmentConsequence());
                            var attributeName = reader.GetNullableString(8);
                            consequence.Attribute = attributeByName[attributeName];
                            consequence.Change.Expression = reader.GetNullableString(9);
                        }
                    }

                    void createTreatmentsWithConsequences()
                    {
                        var attributeByName = simulation.Network.Explorer.NumberAttributes.ToDictionary(attribute => attribute.Name);
                        var budgetByName = simulation.InvestmentPlan.Budgets.ToDictionary(budget => budget.Name, SelectionEqualityComparer<string>.Create(name => name.Trim()));

                        while (reader.Read())
                        {
                            var id = reader.GetInt32(0);
                            if (!treatmentById.TryGetValue(id, out var treatment))
                            {
                                treatment = simulation.AddTreatment();
                                treatment.Name = reader.GetNullableString(1);
                                treatment.ShadowForAnyTreatment = reader.GetInt32(2);
                                treatment.ShadowForSameTreatment = reader.GetInt32(3);

                                var budgetField = reader.GetNullableString(4);
                                var budgetNames = budgetField.Split(',');
                                foreach (var budgetName in budgetNames)
                                {
                                    if (budgetByName.TryGetValue(budgetName, out var budget))
                                    {
                                        treatment.Budgets.Add(budget);
                                    }
                                }

                                treatment.Description = reader.GetNullableString(5);

                                treatmentById.Add(id, treatment);
                            }

                            var consequence = treatment.AddConsequence();
                            var attributeName = reader.GetNullableString(6);
                            consequence.Attribute = attributeByName[attributeName];
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
                            var treatment = treatmentById[id];
                            var cost = treatment.AddCost();
                            cost.Equation.Expression = reader.GetNullableString(1);
                            cost.Criterion.Expression = reader.GetNullableString(2);
                        }
                    }

                    void fillTreatmentFeasibilities()
                    {
                        while (reader.Read())
                        {
                            var id = reader.GetInt32(0);
                            var treatment = treatmentById[id];
                            var feasibility = treatment.AddFeasibilityCriterion();
                            feasibility.Expression = reader.GetNullableString(1);
                        }
                    }

                    void fillTreatmentSchedulings()
                    {
                        while (reader.Read())
                        {
                            var id = reader.GetInt32(0);
                            var treatment = treatmentById[id];
                            var scheduling = treatment.Schedulings.GetAdd(new TreatmentScheduling());
                            scheduling.OffsetToFutureYear = reader.GetInt32(1);
                            var scheduledId = reader.GetInt32(2);
                            scheduling.Treatment = treatmentById[scheduledId];
                        }
                    }

                    void fillTreatmentSupersessions()
                    {
                        while (reader.Read())
                        {
                            var id = reader.GetInt32(0);
                            var treatment = treatmentById[id];
                            var supersession = treatment.AddSupersession();
                            var supersededId = reader.GetInt32(1);
                            supersession.Treatment = treatmentById[supersededId];
                            supersession.Criterion.Expression = reader.GetNullableString(2);
                        }
                    }

                    void createBudgetPriorities()
                    {
                        var budgetByName = simulation.InvestmentPlan.Budgets.ToDictionary(budget => budget.Name);
                        var priorityById = new Dictionary<int, BudgetPriority>();

                        while (reader.Read())
                        {
                            var id = reader.GetInt32(0);
                            if (!priorityById.TryGetValue(id, out var priority))
                            {
                                priority = simulation.AnalysisMethod.AddBudgetPriority();
                                priority.PriorityLevel = reader.GetInt32(1);
                                priority.Criterion.Expression = reader.GetNullableString(2);
                                priority.Year = reader.GetNullableInt32(3);

                                if (priority.Year == 0)
                                {
                                    priority.Year = null;
                                }

                                priorityById.Add(id, priority);
                            }

                            var budgetName = reader.GetNullableString(4);
                            if (budgetByName.TryGetValue(budgetName, out var budget))
                            {
                                var pair = priority.GetBudgetPercentagePair(budget);
                                pair.Percentage = (decimal)reader.GetDouble(5);
                            }
                        }
                    }

                    void createTargetConditionGoals()
                    {
                        var attributeByName = simulation.Network.Explorer.NumericAttributes.ToDictionary(attribute => attribute.Name, StringComparer.OrdinalIgnoreCase);

                        while (reader.Read())
                        {
                            var goal = simulation.AnalysisMethod.AddTargetConditionGoal();
                            var attributeName = reader.GetNullableString(0);
                            goal.Attribute = attributeByName[attributeName];
                            goal.Year = reader.GetNullableInt32(1);
                            goal.Target = reader.GetDouble(2);
                            goal.Name = reader.GetNullableString(3);
                            goal.Criterion.Expression = reader.GetNullableString(4);
                        }
                    }

                    void createDeficientConditionGoals()
                    {
                        var attributeByName = simulation.Network.Explorer.NumericAttributes.ToDictionary(attribute => attribute.Name, StringComparer.OrdinalIgnoreCase);

                        while (reader.Read())
                        {
                            var goal = simulation.AnalysisMethod.AddDeficientConditionGoal();
                            var attributeName = reader.GetNullableString(0);
                            goal.Attribute = attributeByName[attributeName];
                            goal.Name = reader.GetNullableString(1);
                            goal.DeficientLimit = reader.GetDouble(2);
                            goal.AllowedDeficientPercentage = reader.GetDouble(3);
                            goal.Criterion.Expression = reader.GetNullableString(4);
                        }
                    }

                    void createRemainingLifeLimits()
                    {
                        var attributeByName = simulation.Network.Explorer.NumericAttributes.ToDictionary(attribute => attribute.Name, StringComparer.OrdinalIgnoreCase);

                        while (reader.Read())
                        {
                            var limit = simulation.AnalysisMethod.AddRemainingLifeLimit();
                            var attributeName = reader.GetNullableString(0);
                            limit.Attribute = attributeByName[attributeName];
                            limit.Value = reader.GetDouble(1);
                            limit.Criterion.Expression = reader.GetNullableString(2);
                        }
                    }

                    void fillBudgetAmounts()
                    {
                        var budgetByName = simulation.InvestmentPlan.Budgets.ToDictionary(budget => budget.Name);

                        while (reader.Read())
                        {
                            var budgetName = reader.GetNullableString(0);
                            var budget = budgetByName[budgetName];
                            var year = reader.GetInt32(1);
                            var yearOffset = year - simulation.InvestmentPlan.FirstYearOfAnalysisPeriod;
                            budget.YearlyAmounts[yearOffset].Value = (decimal)reader.GetDouble(2);
                        }
                    }

                    void createBudgetConditions()
                    {
                        var budgetByName = simulation.InvestmentPlan.Budgets.ToDictionary(budget => budget.Name);

                        while (reader.Read())
                        {
                            var budgetName = reader.GetNullableString(0);
                            var budget = budgetByName[budgetName];
                            var criterion = reader.GetNullableString(1);
                            var budgetCondition = simulation.InvestmentPlan.AddBudgetCondition();
                            budgetCondition.Budget = budget;
                            budgetCondition.Criterion.Expression = criterion;
                        }
                    }

                    void createCashFlowRules()
                    {
                        var ruleById = new Dictionary<int, CashFlowRule>();

                        while (reader.Read())
                        {
                            var id = reader.GetInt32(0);
                            if (!ruleById.TryGetValue(id, out var rule))
                            {
                                rule = simulation.InvestmentPlan.AddCashFlowRule();
                                rule.Name = reader.GetNullableString(1);
                                rule.Criterion.Expression = reader.GetNullableString(2);

                                ruleById.Add(id, rule);
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

        private const string NUMBER_ATTRIBUTE_TYPE_NAME = "NUMBER";

        private const string PASSIVE_TREATMENT_NAME = "No Treatment";

        private const string SECTIONID_COLUMN_NAME = "sectionid";

        private const string STRING_ATTRIBUTE_TYPE_NAME = "STRING";

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
    }
}
