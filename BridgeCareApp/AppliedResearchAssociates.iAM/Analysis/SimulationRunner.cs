using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace AppliedResearchAssociates.iAM.Analysis
{
    public sealed class SimulationRunner
    {
        // [REVIEW] Are cash flow rules only considered when the non-cash-flow payment logic isn't
        // sufficient to cover the treatment cost? Or are they always used if their criteria are met?

        // [REVIEW] A treatment's "any" shadow applies to *all* treatments, including that same
        // treatment, right?

        // [REVIEW] What is the "Actual_Spent" budget? It is not present in the budget order, yet it
        // appears in the treatment table.

        // [REVIEW] What is the "No_Funds" budget?

        // [REVIEW] Is a treatment feasible only when *all* of its feasibility criteria are met? or
        // when *any* are met?

        // [REVIEW] How are inflation rate and discount rate used in the analysis logic?

        // [REVIEW] What is the "PerformanceCurve.Shift" bool supposed to do?

        // [REVIEW] How should the change/equation pair on a consequence be handled?

        // [REVIEW] What happens when one attribute has multiple consequences whose criteria are met?

        // [REVIEW] What should happen when there are multiple applicable cash flow rules?
        // Currently, it throws.

        // [REVIEW] What happens when one calculated field has multiple equations whose criteria are met?

        // [REVIEW] When a single budget has multiple conditions, do all of them have to be
        // satisfied? or just one? Currently, it's "met" when there are either no conditions or at
        // least one condition is met.

        // [REVIEW] When a given budget has no condition rows entered, is there effectively an
        // implicit blank condition, i.e. the condition is always met? or does it mean the budget
        // can never be used?

        // [REVIEW] What is the relationship between the two "Route/Section Definition" sub-nodes
        // and the set of all networks?

        // [REVIEW] Budget conditions DO NOT have to be met for committed projects, right?

        public SimulationRunner(Simulation simulation) => Simulation = simulation ?? throw new ArgumentNullException(nameof(simulation));

        public event EventHandler<InformationEventArgs> Information;

        public event EventHandler<WarningEventArgs> Warning;

        public Simulation Simulation { get; }

        public void Run()
        {
            if (Interlocked.Exchange(ref StatusCode, STATUS_CODE_RUNNING) == STATUS_CODE_RUNNING)
            {
                throw new InvalidOperationException("Runner is already running.");
            }

            Inform("Simulation initializing ...");

            // [REVIEW] Ensure these pre-computed data structures are as exhaustive as possible.

            ActiveTreatments = Simulation.GetActiveTreatments();
            BudgetContexts = Simulation.InvestmentPlan.Budgets.Select(budget => new BudgetContext(budget)).ToArray();
            CommittedProjectsPerSection = Simulation.CommittedProjects.ToLookup(committedProject => committedProject.Section);
            ConditionsPerBudget = Simulation.InvestmentPlan.BudgetConditions.ToLookup(budgetCondition => budgetCondition.Budget);
            CurvesPerAttribute = Simulation.PerformanceCurves.ToLookup(curve => curve.Attribute);
            NumberAttributeByName = Simulation.Network.Explorer.NumberAttributes.ToDictionary(attribute => attribute.Name, StringComparer.OrdinalIgnoreCase);
            SortedDistributionRulesPerCashFlowRule = Simulation.InvestmentPlan.CashFlowRules.ToDictionary(Static.Identity, rule => rule.DistributionRules.ToSortedDictionary(distributionRule => distributionRule.CostCeiling ?? decimal.MaxValue));

            SectionContexts = Simulation.Network.Sections
#if !DEBUG
                .AsParallel()
#endif
                .Select(section => new SectionContext(section, this))
                .Where(context => Simulation.AnalysisMethod.Filter.EvaluateOrDefault(context))
                .OrderBy(context => (context.Section.Facility.Name, context.Section.Name))
                .ToArray();

            if (SectionContexts.Count == 0)
            {
                throw new SimulationException("There are no sections.");
            }

            if (SectionContexts.Select(context => context.Section.AreaUnit).Distinct().Count() > 1)
            {
                throw new SimulationException("Sections have multiple distinct area units.");
            }

            InParallel(SectionContexts, context => context.RollForward());

            switch (Simulation.AnalysisMethod.SpendingStrategy)
            {
            case SpendingStrategy.NoSpending:
                AllowedSpending = Spending.None;
                ConditionGoalsAreMet = () => false;
                break;

            case SpendingStrategy.UnlimitedSpending:
                AllowedSpending = Spending.Unlimited;
                ConditionGoalsAreMet = () => false;
                break;

            case SpendingStrategy.UntilTargetAndDeficientConditionGoalsMet:
                AllowedSpending = Spending.Unlimited;
                ConditionGoalsAreMet = () => GoalsAreMet(TargetConditionActuals) && GoalsAreMet(DeficientConditionActuals);
                break;

            case SpendingStrategy.UntilTargetConditionGoalsMet:
                AllowedSpending = Spending.Unlimited;
                ConditionGoalsAreMet = () => GoalsAreMet(TargetConditionActuals);
                break;

            case SpendingStrategy.UntilDeficientConditionGoalsMet:
                AllowedSpending = Spending.Unlimited;
                ConditionGoalsAreMet = () => GoalsAreMet(DeficientConditionActuals);
                break;

            case SpendingStrategy.AsBudgetPermits:
                AllowedSpending = Spending.Budgeted;
                ConditionGoalsAreMet = () => false;
                break;

            default:
                throw new SimulationException(MessageStrings.InvalidSpendingStrategy);
            }

            Simulation.Results.Clear();

            foreach (var year in Simulation.InvestmentPlan.YearsOfAnalysis)
            {
                Inform($"Simulating {year} ...");

                var detail = new SimulationYearDetail(year);
                Simulation.Results.Add(detail);

                InParallel(SectionContexts, context => context.ResetDetail());

                MoveBudgetsToNextYear();
                var unhandledContexts = ApplyRequiredEvents(year);
                ConsiderSelectableTreatments(unhandledContexts, year);

                InParallel(unhandledContexts, context =>
                {
                    context.EventSchedule.Add(year, Simulation.DesignatedPassiveTreatment);
                    context.ApplyPassiveTreatment(year);
                });

                UpdateConditionActuals(year);
                RecordStatusOfConditionGoals(detail);

                InParallel(SectionContexts, context => context.CopyAttributeValuesToDetail());

                detail.Sections.AddRange(SectionContexts.Select(context => context.Detail));
            }

            Inform("Simulation complete.");

            StatusCode = STATUS_CODE_NOT_RUNNING;
        }

        internal ILookup<Section, CommittedProject> CommittedProjectsPerSection { get; private set; }

        internal ILookup<NumberAttribute, PerformanceCurve> CurvesPerAttribute { get; private set; }

        internal IReadOnlyDictionary<string, NumberAttribute> NumberAttributeByName { get; private set; }

        internal void Inform(string message) => OnInformation(new InformationEventArgs(message));

        internal void Warn(string message) => OnWarning(new WarningEventArgs(message));

        private const int STATUS_CODE_NOT_RUNNING = 0;

        private const int STATUS_CODE_RUNNING = 1;

        private static readonly IComparer<BudgetPriority> BudgetPriorityComparer = SelectionComparer<BudgetPriority>.Create(priority => priority.PriorityLevel);

        private IReadOnlyCollection<SelectableTreatment> ActiveTreatments;

        private Spending AllowedSpending;

        private IReadOnlyCollection<BudgetContext> BudgetContexts;

        private Func<bool> ConditionGoalsAreMet;

        private ILookup<Budget, BudgetCondition> ConditionsPerBudget;

        private IReadOnlyCollection<ConditionActual> DeficientConditionActuals;

        private IReadOnlyCollection<SectionContext> SectionContexts;

        private IReadOnlyDictionary<CashFlowRule, SortedDictionary<decimal, CashFlowDistributionRule>> SortedDistributionRulesPerCashFlowRule;

        private int StatusCode;

        private IReadOnlyCollection<ConditionActual> TargetConditionActuals;

        private enum CostCoverage
        {
            None,
            Full,
            CashFlow,
        }

        private enum Spending
        {
            None,
            Budgeted,
            Unlimited,
        }

        private static bool GoalsAreMet(IEnumerable<ConditionActual> conditionActuals) => conditionActuals.All(actual => actual.GoalIsMet);

        private static void InParallel<T>(IEnumerable<T> items, Action<T> action)
        {
#if DEBUG
            foreach (var item in items)
            {
                action(item);
            }
#else
            _ = System.Threading.Tasks.Parallel.ForEach(items, action);
#endif
        }

        private ICollection<SectionContext> ApplyRequiredEvents(int year)
        {
            var unhandledContexts = new List<SectionContext>();

            foreach (var context in SectionContexts)
            {
                var yearIsScheduled = context.EventSchedule.TryGetValue(year, out var scheduledEvent);

                if (yearIsScheduled && scheduledEvent.IsT2(out var progress))
                {
                    var costCoverage = TryToPayForTreatment(
                        context,
                        progress.Treatment,
                        year,
                        budgetContext => budgetContext.CurrentAmount,
                        progress.Cost);

                    if (costCoverage == CostCoverage.None)
                    {
                        // [TODO] Add code so that this "should" never happen. In other words, if it
                        // was going to happen, then cash flow would not be used. The code will
                        // basically track what budgets are used for the first year, then try to use
                        // those budgets (same order) in subsequent years, unconditionally. The
                        // trick to ensuring funding will be to immediately deduct the future cost
                        // from the future amounts in the budgets' contexts. If those future amounts
                        // can't cover the cost, then the cash flow doesn't happen at all.
                        throw new SimulationException("Cash flow project progress is unfunded.");
                    }

                    if (progress.IsComplete)
                    {
                        context.ApplyTreatment(progress.Treatment, year);
                    }
                    else
                    {
                        context.LogTreatmentProgression(progress.Treatment);
                    }

                    context.Detail.TreatmentSource = TreatmentSource.CashFlowProject;
                }
                else
                {
                    context.ApplyPerformanceCurves();

                    if (yearIsScheduled && scheduledEvent.IsT1(out var treatment))
                    {
                        var costCoverage = TryToPayForTreatment(
                            context,
                            treatment,
                            year,
                            budgetContext => budgetContext.CurrentAmount);

                        if (costCoverage == CostCoverage.None)
                        {
                            if (treatment is CommittedProject)
                            {
                                // [TODO] Add code so that this "should" never happen. In other
                                // words, if it was going to happen, an error would be thrown
                                // up-front when committed projects were loading. Similar "future
                                // deduction" logic as described above for cash flow should be used.
                                throw new SimulationException("Committed project is unfunded.");
                            }

                            context.Detail.NameOfUnfundedScheduledTreatment = treatment.Name;
                            _ = context.EventSchedule.Remove(year);
                            unhandledContexts.Add(context);
                        }
                        else
                        {
                            if (costCoverage == CostCoverage.Full)
                            {
                                context.ApplyTreatment(treatment, year);
                            }
                            else
                            {
                                context.LogTreatmentProgression(treatment);
                            }

                            context.Detail.TreatmentSource = treatment is CommittedProject
                                ? TreatmentSource.CommittedProject
                                : TreatmentSource.ScheduledTreatment;
                        }
                    }
                    else
                    {
                        unhandledContexts.Add(context);
                    }
                }
            }

            return unhandledContexts;
        }

        private void ConsiderSelectableTreatments(ICollection<SectionContext> unhandledContexts, int year)
        {
            var treatmentOptions = GetTreatmentOptionsInOptimalOrder(unhandledContexts, year);

            UpdateConditionActuals(year);

            if (AllowedSpending != Spending.None && !ConditionGoalsAreMet())
            {
                var applicablePriorities = new List<BudgetPriority>();
                foreach (var levelPriorities in Simulation.AnalysisMethod.BudgetPriorities.GroupBy(priority => priority.PriorityLevel))
                {
                    var priority = Option
                        .Of(levelPriorities.SingleOrDefault(p => p.Year == year))
                        .Coalesce(() => levelPriorities.SingleOrDefault(p => p.Year == null));

                    priority.Handle(applicablePriorities.Add, Inaction.Delegate);
                }

                applicablePriorities.Sort(BudgetPriorityComparer);

                foreach (var priority in applicablePriorities)
                {
                    foreach (var context in BudgetContexts)
                    {
                        context.Priority = priority;
                    }

                    foreach (var option in treatmentOptions)
                    {
                        if (unhandledContexts.Contains(option.Context) && priority.Criterion.EvaluateOrDefault(option.Context))
                        {
                            var costCoverage = TryToPayForTreatment(
                                option.Context,
                                option.CandidateTreatment,
                                year,
                                context => context.CurrentPrioritizedAmount.Value);

                            if (costCoverage != CostCoverage.None)
                            {
                                if (costCoverage == CostCoverage.Full)
                                {
                                    option.Context.EventSchedule.Add(year, option.CandidateTreatment);
                                    option.Context.ApplyTreatment(option.CandidateTreatment, year);
                                    UpdateConditionActuals(year);

                                    if (ConditionGoalsAreMet())
                                    {
                                        return;
                                    }
                                }
                                else
                                {
                                    option.Context.LogTreatmentProgression(option.CandidateTreatment);
                                }

                                option.Context.Detail.TreatmentSource = TreatmentSource.SelectedTreatment;
                                _ = unhandledContexts.Remove(option.Context);
                            }
                        }
                    }
                }
            }
        }

        private IReadOnlyCollection<ConditionActual> GetDeficientConditionActuals()
        {
            var results = new List<ConditionActual>();

            foreach (var goal in Simulation.AnalysisMethod.DeficientConditionGoals)
            {
                var goalContexts = SectionContexts
#if !DEBUG
                    .AsParallel()
#endif
                    .Where(context => goal.Criterion.EvaluateOrDefault(context))
                    .ToArray();

                var goalArea = goalContexts.Sum(context => context.GetAreaOfSection());
                var deficientContexts = goalContexts.Where(context => goal.LevelIsDeficient(context.GetNumber(goal.Attribute.Name)));
                var deficientArea = deficientContexts.Sum(context => context.GetAreaOfSection());
                var deficientPercentageActual = deficientArea / goalArea * 100;

                results.Add(new ConditionActual(goal, deficientPercentageActual));
            }

            return results;
        }

        private IReadOnlyCollection<ConditionActual> GetTargetConditionActuals(int year)
        {
            var results = new List<ConditionActual>();

            foreach (var goal in Simulation.AnalysisMethod.TargetConditionGoals)
            {
                if (goal.Year.HasValue && goal.Year.Value != year)
                {
                    continue;
                }

                var goalContexts = SectionContexts
#if !DEBUG
                    .AsParallel()
#endif
                    .Where(context => goal.Criterion.EvaluateOrDefault(context))
                    .ToArray();

                var goalAreaValues = goalContexts.Select(context => context.GetAreaOfSection()).ToArray();
                var averageArea = goalAreaValues.Average();
                var goalAreaWeights = goalAreaValues.Select(area => area / averageArea);
                var averageActual = goalContexts.Zip(goalAreaWeights, (context, weight) => context.GetNumber(goal.Attribute.Name) * weight).Average();

                results.Add(new ConditionActual(goal, averageActual));
            }

            return results;
        }

        private IReadOnlyCollection<TreatmentOption> GetTreatmentOptionsInOptimalOrder(IEnumerable<SectionContext> contexts, int year)
        {
            Func<TreatmentOption, double> objectiveFunction;
            switch (Simulation.AnalysisMethod.OptimizationStrategy)
            {
            case OptimizationStrategy.Benefit:
                objectiveFunction = option => option.Benefit;
                break;

            case OptimizationStrategy.BenefitToCostRatio:
                objectiveFunction = option => option.Benefit / option.CostPerUnitArea;
                break;

            case OptimizationStrategy.RemainingLife:
                ValidateRemainingLifeOptimization();
                objectiveFunction = option => option.RemainingLife.Value;
                break;

            case OptimizationStrategy.RemainingLifeToCostRatio:
                ValidateRemainingLifeOptimization();
                objectiveFunction = option => option.RemainingLife.Value / option.CostPerUnitArea;
                break;

            default:
                throw new SimulationException(MessageStrings.InvalidOptimizationStrategy);
            }

            var treatmentOptionsBag = new ConcurrentBag<TreatmentOption>();
            void addTreatmentOptions(SectionContext context)
            {
                if (context.YearIsWithinShadowForAnyTreatment(year))
                {
                    return;
                }

                var feasibleTreatments = ActiveTreatments.ToHashSet();

                _ = feasibleTreatments.RemoveWhere(treatment => context.YearIsWithinShadowForSameTreatment(year, treatment));
                _ = feasibleTreatments.RemoveWhere(treatment => !treatment.IsFeasible(context));

                var supersededTreatments = Enumerable.ToArray(
                    from treatment in feasibleTreatments
                    from supersession in treatment.Supersessions
                    where supersession.Criterion.EvaluateOrDefault(context)
                    select supersession.Treatment);

                feasibleTreatments.ExceptWith(supersededTreatments);

                if (feasibleTreatments.Count > 0)
                {
                    var remainingLifeCalculatorFactories = Enumerable.ToArray(
                        from limit in Simulation.AnalysisMethod.RemainingLifeLimits
                        where limit.Criterion.EvaluateOrDefault(context)
                        group limit.Value by limit.Attribute into attributeLimitValues
                        select new RemainingLifeCalculator.Factory(attributeLimitValues));

                    var baselineOutlook = new TreatmentOutlook(context, Simulation.DesignatedPassiveTreatment, year, remainingLifeCalculatorFactories);

                    foreach (var treatment in feasibleTreatments)
                    {
                        var outlook = new TreatmentOutlook(context, treatment, year, remainingLifeCalculatorFactories);
                        var option = outlook.GetOptionRelativeToBaseline(baselineOutlook);
                        treatmentOptionsBag.Add(option);

                        context.Detail.TreatmentOptions.Add(option.Detail);
                    }
                }
            }

            InParallel(contexts, addTreatmentOptions);
            var treatmentOptions = treatmentOptionsBag.OrderByDescending(objectiveFunction).ToArray();

            return treatmentOptions;
        }

        private void MoveBudgetsToNextYear()
        {
            foreach (var context in BudgetContexts)
            {
                context.MoveToNextYear();
            }
        }

        private void OnInformation(InformationEventArgs e) => Information?.Invoke(this, e);

        private void OnWarning(WarningEventArgs e) => Warning?.Invoke(this, e);

        private void RecordStatusOfConditionGoals(SimulationYearDetail detail)
        {
            detail.TargetConditionGoals.AddRange(TargetConditionActuals.Select(actual => new TargetConditionGoalDetail
            {
                GoalName = actual.Goal.Name,
                AttributeName = actual.Goal.Attribute.Name,
                GoalIsMet = actual.GoalIsMet,
                TargetValue = (actual.Goal as TargetConditionGoal).Target,
                ActualValue = actual.Value,
            }));

            detail.DeficientConditionGoals.AddRange(DeficientConditionActuals.Select(actual => new DeficientConditionGoalDetail
            {
                GoalName = actual.Goal.Name,
                AttributeName = actual.Goal.Attribute.Name,
                GoalIsMet = actual.GoalIsMet,
                DeficientLimit = (actual.Goal as DeficientConditionGoal).DeficientLimit,
                AllowedDeficientPercentage = (actual.Goal as DeficientConditionGoal).AllowedDeficientPercentage,
                ActualDeficientPercentage = actual.Value,
            }));
        }

        private CostCoverage TryToPayForTreatment(SectionContext sectionContext, Treatment treatment, int year, Func<BudgetContext, decimal> getAvailableAmount, decimal? indivisibleCost = null)
        {
            var treatmentConsideration = new TreatmentConsiderationDetail(treatment.Name);
            sectionContext.Detail.TreatmentConsiderations.Add(treatmentConsideration);

            treatmentConsideration.Budgets.AddRange(BudgetContexts.Select(budgetContext => new BudgetDetail(budgetContext.Budget.Name)
            {
                BudgetReason = treatment.CanUseBudget(budgetContext.Budget) ? BudgetReason.NotNeeded : BudgetReason.NotUsable
            }));

            decimal remainingCost;
            Action scheduleCashFlowEvents = null;

            if (indivisibleCost.HasValue)
            {
                remainingCost = indivisibleCost.Value;
            }
            else
            {
                remainingCost = (decimal)sectionContext.GetCostOfTreatment(treatment);

                var cashFlowRule = Simulation.InvestmentPlan.CashFlowRules.SingleOrDefault(rule => rule.Criterion.EvaluateOrDefault(sectionContext));
                if (cashFlowRule != null)
                {
                    var distributionRule = SortedDistributionRulesPerCashFlowRule[cashFlowRule].First(kv => remainingCost <= kv.Key).Value;
                    if (distributionRule.YearlyPercentages.Count > 1)
                    {
                        var lastYearOfCashFlow = year + distributionRule.YearlyPercentages.Count - 1;
                        if (lastYearOfCashFlow <= Simulation.InvestmentPlan.LastYearOfAnalysisPeriod)
                        {
                            var scheduleIsClear = !Enumerable.Range(year, distributionRule.YearlyPercentages.Count).Any(sectionContext.EventSchedule.ContainsKey);
                            if (scheduleIsClear)
                            {
                                var costPerYear = distributionRule.YearlyPercentages.Select(percentage => percentage / 100 * remainingCost).ToArray();

                                if (costPerYear.Sum() != remainingCost)
                                {
                                    throw new InvalidOperationException("Sum of yearly costs from cash flow split does not equal original total cost.");
                                }

                                remainingCost = costPerYear[0];

                                var progression = costPerYear.Select(cost => new TreatmentProgress(treatment, cost)).ToArray();
                                progression.Last().IsComplete = true;

                                scheduleCashFlowEvents = () =>
                                {
                                    foreach (var (yearProgress, yearOffset) in Zip.Short(progression, Static.Count(0)))
                                    {
                                        sectionContext.EventSchedule.Add(year + yearOffset, yearProgress);
                                    }
                                };
                            }
                        }
                    }
                }
            }

            var costAllocators = new List<Action>();
            void addCostAllocator(decimal cost, BudgetContext budgetContext)
            {
                remainingCost -= cost;
                costAllocators.Add(() => budgetContext.AllocateCost(cost));
            }

            foreach (var (budgetContext, budgetDetail) in Zip.Strict(BudgetContexts, treatmentConsideration.Budgets))
            {
                if (remainingCost <= 0)
                {
                    break;
                }

                if (budgetDetail.BudgetReason == BudgetReason.NotUsable)
                {
                    continue;
                }

                var budgetConditions = ConditionsPerBudget[budgetContext.Budget];
                var budgetConditionIsMet = treatment is CommittedProject || budgetConditions.Count() == 0 || budgetConditions.Any(condition => condition.Criterion.EvaluateOrDefault(sectionContext));
                if (!budgetConditionIsMet)
                {
                    budgetDetail.BudgetReason = BudgetReason.ConditionNotMet;
                    continue;
                }

                var availableAmount = getAvailableAmount(budgetContext);
                if (AllowedSpending == Spending.Unlimited || remainingCost <= availableAmount)
                {
                    budgetDetail.BudgetReason = BudgetReason.CostCoveredInFull;
                    budgetDetail.CoveredCost = remainingCost;
                    addCostAllocator(remainingCost, budgetContext);
                    break;
                }

                if (Simulation.AnalysisMethod.ShouldUseExtraFundsAcrossBudgets)
                {
                    budgetDetail.BudgetReason = BudgetReason.CostCoveredInPart;
                    budgetDetail.CoveredCost = availableAmount;
                    addCostAllocator(availableAmount, budgetContext);
                }
                else
                {
                    budgetDetail.BudgetReason = BudgetReason.CostNotCovered;
                }
            }

            if (remainingCost < 0)
            {
                throw new SimulationException(MessageStrings.RemainingCostIsNegative);
            }

            if (remainingCost > 0)
            {
                return CostCoverage.None;
            }

            foreach (var costAllocator in costAllocators)
            {
                costAllocator();
            }

            if (scheduleCashFlowEvents != null)
            {
                scheduleCashFlowEvents();
                return CostCoverage.CashFlow;
            }

            return CostCoverage.Full;
        }

        private void UpdateConditionActuals(int year)
        {
            TargetConditionActuals = GetTargetConditionActuals(year);
            DeficientConditionActuals = GetDeficientConditionActuals();
        }

        private void ValidateRemainingLifeOptimization()
        {
            if (Simulation.AnalysisMethod.RemainingLifeLimits.Count == 0)
            {
                throw new SimulationException(MessageStrings.RemainingLifeOptimizationHasNoLimits);
            }
        }
    }
}
