﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace AppliedResearchAssociates.iAM.Analysis
{
    public sealed class SimulationRunner
    {
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

            ActiveTreatments = Simulation.GetActiveTreatments();
            BudgetContexts = Simulation.GetBudgetContextsWithCostAllocationsForCommittedProjects();

            BudgetPrioritiesPerYear = Simulation.InvestmentPlan.YearsOfAnalysis.ToDictionary(_ => _, year =>
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
                return applicablePriorities.AsEnumerable();
            });

            CommittedProjectsPerSection = Simulation.CommittedProjects.ToLookup(committedProject => committedProject.Section);
            ConditionsPerBudget = Simulation.InvestmentPlan.BudgetConditions.ToLookup(budgetCondition => budgetCondition.Budget);
            CurvesPerAttribute = Simulation.PerformanceCurves.ToLookup(curve => curve.Attribute);
            NumberAttributeByName = Simulation.Network.Explorer.NumberAttributes.ToDictionary(attribute => attribute.Name, StringComparer.OrdinalIgnoreCase);
            SortedDistributionRulesPerCashFlowRule = Simulation.InvestmentPlan.CashFlowRules.ToDictionary(_ => _, rule => rule.DistributionRules.ToSortedDictionary(distributionRule => distributionRule.CostCeiling ?? decimal.MaxValue));

            foreach (var treatment in Simulation.Treatments)
            {
                treatment.SetConsequencesPerAttribute();
            }

            SectionContexts = Simulation.Network.Sections
#if !DEBUG
                .AsParallel()
#endif
                .Select(section => new SectionContext(section, this))
                .Where(context => Simulation.AnalysisMethod.Filter.EvaluateOrDefault(context))
                .ToSortedSet(SelectionComparer<SectionContext>.Create(context => (context.Section.Facility.Name, context.Section.Name)));

            if (SectionContexts.Count == 0)
            {
                throw new SimulationException("There are no sections.");
            }

            if (SectionContexts.Select(context => context.Section.AreaUnit).Distinct().Count() > 1)
            {
                throw new SimulationException("Sections have multiple distinct area units.");
            }

            InParallel(SectionContexts, context => context.RollForward());

            SpendingLimit = Simulation.AnalysisMethod.SpendingLimit;

            switch (Simulation.AnalysisMethod.SpendingStrategy)
            {
            case SpendingStrategy.NoSpending:
                ConditionGoalsEvaluator = () => false;
                break;

            case SpendingStrategy.UnlimitedSpending:
                ConditionGoalsEvaluator = () => false;
                break;

            case SpendingStrategy.UntilTargetAndDeficientConditionGoalsMet:
                ConditionGoalsEvaluator = () => GoalsAreMet(TargetConditionActuals) && GoalsAreMet(DeficientConditionActuals);
                break;

            case SpendingStrategy.UntilTargetConditionGoalsMet:
                ConditionGoalsEvaluator = () => GoalsAreMet(TargetConditionActuals);
                break;

            case SpendingStrategy.UntilDeficientConditionGoalsMet:
                ConditionGoalsEvaluator = () => GoalsAreMet(DeficientConditionActuals);
                break;

            case SpendingStrategy.AsBudgetPermits:
                ConditionGoalsEvaluator = () => false;
                break;

            default:
                throw new SimulationException(MessageStrings.InvalidSpendingStrategy);
            }

            ObjectiveFunction = Simulation.AnalysisMethod.ObjectiveFunction;

            Simulation.Results.Clear();

            foreach (var year in Simulation.InvestmentPlan.YearsOfAnalysis)
            {
                Inform($"Simulating {year} ...");

                var yearDetail = Simulation.Results.GetAdd(new SimulationYearDetail(year));
                yearDetail.InitialConditionOfNetwork = Simulation.AnalysisMethod.Benefit.GetNetworkCondition(SectionContexts);

                var baselineContexts = ApplyRequiredEvents(year);
                var baselineContextPerWorkingContext = baselineContexts.ToDictionary(_ => new SectionContext(_), _ => _);

                InParallel(baselineContextPerWorkingContext, _ =>
                {
                    var (working, baseline) = _;
                    working.CopyDetailFrom(baseline);
                });

                InParallel(baselineContexts, context =>
                {
                    context.Detail.TreatmentCause = TreatmentCause.NoSelection;
                    context.EventSchedule.Add(year, Simulation.DesignatedPassiveTreatment);
                    context.ApplyPassiveTreatment(year);
                });

                ConsiderSelectableTreatments(baselineContextPerWorkingContext, year);

                RecordStatusOfConditionGoals(yearDetail);

                InParallel(SectionContexts, context => context.CopyAttributeValuesToDetail());

                yearDetail.Sections.AddRange(SectionContexts.Select(context => context.Detail));

                InParallel(SectionContexts, context => context.ResetDetail());

                MoveBudgetsToNextYear();
            }

            Inform("Simulation complete.");

            foreach (var treatment in Simulation.Treatments)
            {
                treatment.UnsetConsequencesPerAttribute();
            }

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

        private IReadOnlyCollection<BudgetContext> BudgetContexts;

        private IReadOnlyDictionary<int, IEnumerable<BudgetPriority>> BudgetPrioritiesPerYear;

        private Func<bool> ConditionGoalsEvaluator;

        private ILookup<Budget, BudgetCondition> ConditionsPerBudget;

        private IReadOnlyCollection<ConditionActual> DeficientConditionActuals;

        private Func<TreatmentOption, double> ObjectiveFunction;

        private ICollection<SectionContext> SectionContexts;

        private IReadOnlyDictionary<CashFlowRule, SortedDictionary<decimal, CashFlowDistributionRule>> SortedDistributionRulesPerCashFlowRule;

        private SpendingLimit SpendingLimit;

        private int StatusCode;

        private IReadOnlyCollection<ConditionActual> TargetConditionActuals;

        private enum CostCoverage
        {
            None,
            Full,
            CashFlow,
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
                    context.Detail.TreatmentConsiderations.Add(progress.TreatmentConsideration);

                    if (progress.IsComplete)
                    {
                        context.ApplyTreatment(progress.Treatment, year);
                    }
                    else
                    {
                        context.MarkTreatmentProgress(progress.Treatment);
                    }

                    context.Detail.TreatmentCause = TreatmentCause.CashFlowProject;
                }
                else
                {
                    context.ApplyPerformanceCurves();

                    if (yearIsScheduled && scheduledEvent.IsT1(out var treatment))
                    {
                        var costCoverage = TryToPayForTreatment(context, treatment, year, budgetContext => budgetContext.CurrentAmount);

                        if (costCoverage == CostCoverage.None)
                        {
                            Warn($"Treatment \"{treatment.Name}\" scheduled for year {year} cannot be funded normally. Spending limits will be temporarily removed to fund this treatment.");

                            var actualSpendingLimit = SpendingLimit;
                            SpendingLimit = SpendingLimit.NoLimit;
                            costCoverage = TryToPayForTreatment(context, treatment, year, budgetContext => budgetContext.CurrentAmount);
                            SpendingLimit = actualSpendingLimit;

                            context.Detail.TreatmentFundingIgnoresSpendingLimit = true;
                        }

                        if (costCoverage == CostCoverage.Full)
                        {
                            context.ApplyTreatment(treatment, year);
                        }
                        else if (costCoverage == CostCoverage.CashFlow)
                        {
                            context.MarkTreatmentProgress(treatment);
                        }
                        else
                        {
                            throw new InvalidOperationException("Analysis failed to fund scheduled event.");
                        }

                        context.Detail.TreatmentCause = treatment is CommittedProject
                            ? TreatmentCause.CommittedProject
                            : TreatmentCause.ScheduledTreatment;
                    }
                    else
                    {
                        unhandledContexts.Add(context);
                    }
                }
            }

            return unhandledContexts;
        }

        private bool ConditionGoalsAreMet(int year)
        {
            UpdateConditionActuals(year);
            return ConditionGoalsEvaluator();
        }

        private void ConsiderSelectableTreatments(IDictionary<SectionContext, SectionContext> baselineContextPerWorkingContext, int year)
        {
            var treatmentOptions = GetBeneficialTreatmentOptionsInOptimalOrder(baselineContextPerWorkingContext.Keys, year);

            if (SpendingLimit != SpendingLimit.Zero && !ConditionGoalsAreMet(year))
            {
                foreach (var priority in BudgetPrioritiesPerYear[year])
                {
                    foreach (var context in BudgetContexts)
                    {
                        context.Priority = priority;
                    }

                    foreach (var option in treatmentOptions)
                    {
                        if (baselineContextPerWorkingContext.ContainsKey(option.Context) && priority.Criterion.EvaluateOrDefault(option.Context))
                        {
                            var costCoverage = TryToPayForTreatment(
                                option.Context,
                                option.CandidateTreatment,
                                year,
                                context => context.CurrentPrioritizedAmount ?? context.CurrentAmount);

                            option.Context.Detail.TreatmentConsiderations.Last().BudgetPriorityLevel = priority.PriorityLevel;

                            if (costCoverage != CostCoverage.None)
                            {
                                _ = SectionContexts.Remove(baselineContextPerWorkingContext[option.Context]);
                                SectionContexts.Add(option.Context);
                                _ = baselineContextPerWorkingContext.Remove(option.Context);

                                option.Context.Detail.TreatmentCause = TreatmentCause.SelectedTreatment;

                                if (costCoverage == CostCoverage.Full)
                                {
                                    option.Context.EventSchedule.Add(year, option.CandidateTreatment);
                                    option.Context.ApplyTreatment(option.CandidateTreatment, year);

                                    if (ConditionGoalsAreMet(year))
                                    {
                                        return;
                                    }
                                }
                                else
                                {
                                    option.Context.MarkTreatmentProgress(option.CandidateTreatment);
                                }
                            }
                        }
                    }
                }
            }
        }

        private IReadOnlyCollection<TreatmentOption> GetBeneficialTreatmentOptionsInOptimalOrder(IEnumerable<SectionContext> contexts, int year)
        {
            var treatmentOptionsBag = new ConcurrentBag<TreatmentOption>();
            void addTreatmentOptions(SectionContext context)
            {
                if (context.Section.Name == "22008106500000")
                {
                    // For the district 2 simulation in penndot_light (id 1181), this section is
                    // getting ZERO feasible treatments in 2020 (the first analysis year), but the
                    // legacy analysis output shows that this section is assigned the "Painting
                    // (Joint/Spot/Zone)" treatment in year 2020.
                    ;
                }

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

            var treatmentOptions = treatmentOptionsBag
                .Select(option => (option, value: ObjectiveFunction(option)))
                .Where(_ => _.value > 0)
                .OrderByDescending(_ => _.value * _.option.Context.Section.Area)
                .Select(_ => _.option)
                .ToArray();

            return treatmentOptions;
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

                var goalArea = goalContexts.Sum(context => context.Section.Area);
                var deficientContexts = goalContexts.Where(context => goal.LevelIsDeficient(context.GetNumber(goal.Attribute.Name)));
                var deficientArea = deficientContexts.Sum(context => context.Section.Area);
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

                var goalArea = goalContexts.Sum(context => context.Section.Area);
                var averageActual = goalContexts.Sum(context => context.GetNumber(goal.Attribute.Name) * context.Section.Area) / goalArea;

                results.Add(new ConditionActual(goal, averageActual));
            }

            return results;
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

        private CostCoverage TryToPayForTreatment(SectionContext sectionContext, Treatment treatment, int year, Func<BudgetContext, decimal> getAvailableAmount)
        {
            var treatmentConsideration = sectionContext.Detail.TreatmentConsiderations.GetAdd(new TreatmentConsiderationDetail(treatment.Name));

            treatmentConsideration.Budgets.AddRange(BudgetContexts.Select(budgetContext => new BudgetDetail(budgetContext.Budget.Name)
            {
                BudgetReason = treatment.CanUseBudget(budgetContext.Budget) ? BudgetReason.NotNeeded : BudgetReason.NotUsable
            }));

            if (treatment is CommittedProject)
            {
                treatmentConsideration.Budgets.Single(budget => budget.BudgetReason == BudgetReason.NotNeeded).BudgetReason = BudgetReason.CostCoveredInFull;
                return CostCoverage.Full;
            }

            var applicableBudgets = Zip.Strict(BudgetContexts, treatmentConsideration.Budgets).Where(budgetIsApplicable).ToArray();

            bool budgetIsApplicable((BudgetContext, BudgetDetail) _)
            {
                var (budgetContext, budgetDetail) = _;

                if (budgetDetail.BudgetReason == BudgetReason.NotUsable)
                {
                    return false;
                }

                var budgetConditions = ConditionsPerBudget[budgetContext.Budget];
                var budgetConditionIsMet = treatment is CommittedProject || budgetConditions.Count() == 0 || budgetConditions.Any(condition => condition.Criterion.EvaluateOrDefault(sectionContext));
                if (!budgetConditionIsMet)
                {
                    budgetDetail.BudgetReason = BudgetReason.ConditionNotMet;
                    return false;
                }

                return true;
            }

            var remainingCost = (decimal)sectionContext.GetCostOfTreatment(treatment);

            Action scheduleCashFlowEvents = null;
            treatmentConsideration.ReasonAgainstCashFlow = decideCashFlow();

            ReasonAgainstCashFlow decideCashFlow()
            {
                if (treatment is CommittedProject)
                {
                    return ReasonAgainstCashFlow.TreatmentIsDueToCommittedProject;
                }

                var cashFlowRule = Simulation.InvestmentPlan.CashFlowRules.SingleOrDefault(rule => rule.Criterion.EvaluateOrDefault(sectionContext));
                if (cashFlowRule == null)
                {
                    return ReasonAgainstCashFlow.NoConditionMetForAnyCashFlowRule;
                }

                treatmentConsideration.NameOfApplicableCashFlowRule = cashFlowRule.Name;

                var distributionRule = SortedDistributionRulesPerCashFlowRule[cashFlowRule].First(kv => remainingCost <= kv.Key).Value;
                if (distributionRule.YearlyPercentages.Count == 1)
                {
                    return ReasonAgainstCashFlow.ApplicableDistributionRuleIsForOnlyOneYear;
                }

                var lastYearOfCashFlow = year + distributionRule.YearlyPercentages.Count - 1;
                if (lastYearOfCashFlow > Simulation.InvestmentPlan.LastYearOfAnalysisPeriod)
                {
                    return ReasonAgainstCashFlow.LastYearOfCashFlowIsOutsideOfAnalysisPeriod;
                }

                var scheduleIsBlocked = Static.RangeFromBounds(year + 1, lastYearOfCashFlow).Any(sectionContext.EventSchedule.ContainsKey);
                if (scheduleIsBlocked)
                {
                    return ReasonAgainstCashFlow.FutureEventScheduleIsBlocked;
                }

                var costPerYear = distributionRule.YearlyPercentages.Select(percentage => percentage / 100 * remainingCost).ToArray();

                if (costPerYear.Sum() != remainingCost)
                {
                    throw new InvalidOperationException("Sum of yearly costs from cash flow split does not equal original total cost.");
                }

                var futureCostAllocators = new List<Action>();
                var considerationPerYear = new List<TreatmentConsiderationDetail>();

                var workingBudgetContexts = applicableBudgets.Select(_ => new BudgetContext(_.Item1)).ToArray();
                var applicableBudgetNames = applicableBudgets.Select(_ => _.Item2.BudgetName).ToHashSet();

                var originalBudgetContextPerWorkingBudgetContext = new Dictionary<BudgetContext, BudgetContext>();
                foreach (var ((original, _), working) in Zip.Strict(applicableBudgets, workingBudgetContexts))
                {
                    originalBudgetContextPerWorkingBudgetContext[working] = original;
                }

                foreach (var (futureYearCost, futureYear) in Zip.Short(costPerYear, Static.Count(year)).Skip(1).Reverse())
                {
                    foreach (var budgetContext in workingBudgetContexts)
                    {
                        budgetContext.SetYear(futureYear);
                    }

                    var workingConsideration = considerationPerYear.GetAdd(new TreatmentConsiderationDetail(treatmentConsideration)
                    {
                        ReasonAgainstCashFlow = ReasonAgainstCashFlow.TreatmentIsPartOfActiveCashFlowProject
                    });

                    var workingBudgetDetails = workingConsideration.Budgets.Where(detail => applicableBudgetNames.Contains(detail.BudgetName));
                    var workingBudgets = Zip.Strict(workingBudgetContexts, workingBudgetDetails);

                    var remainingYearCost = futureYearCost;

                    void addFutureCostAllocator(decimal cost, BudgetContext workingBudgetContext)
                    {
                        remainingYearCost -= cost;
                        workingBudgetContext.AllocateCost(cost);
                        workingBudgetContext.LimitPreviousAmountToCurrentAmount();

                        var originalBudgetContext = originalBudgetContextPerWorkingBudgetContext[workingBudgetContext];
                        futureCostAllocators.Add(() => originalBudgetContext.AllocateCost(cost));
                    }

                    tryToAllocateCost(workingBudgets, ref remainingYearCost, addFutureCostAllocator);

                    if (remainingYearCost > 0)
                    {
                        return ReasonAgainstCashFlow.FutureFundingIsNotAvailable;
                    }
                }

                considerationPerYear.Add(treatmentConsideration);
                considerationPerYear.Reverse();

                var progression = costPerYear.Zip(considerationPerYear, (cost, consideration) => new TreatmentProgress(treatment, consideration)).ToArray();
                progression.Last().IsComplete = true;

                scheduleCashFlowEvents = () =>
                {
                    foreach (var futureCostAllocator in futureCostAllocators)
                    {
                        futureCostAllocator();
                    }

                    foreach (var (yearProgress, yearOffset) in Zip.Short(progression, Static.Count(0)))
                    {
                        sectionContext.EventSchedule.Add(year + yearOffset, yearProgress);
                    }
                };

                remainingCost = costPerYear[0];

                return ReasonAgainstCashFlow.None;
            }

            var costAllocators = new List<Action>();
            void addCostAllocator(decimal cost, BudgetContext budgetContext)
            {
                remainingCost -= cost;
                costAllocators.Add(() => budgetContext.AllocateCost(cost));
            }

            tryToAllocateCost(applicableBudgets, ref remainingCost, addCostAllocator);

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

            void tryToAllocateCost(IEnumerable<(BudgetContext, BudgetDetail)> budgets, ref decimal cost, Action<decimal, BudgetContext> costAllocationAction)
            // "cost" is a variable that is being *indirectly* updated by "costAllocationAction".
            {
                foreach (var (budgetContext, budgetDetail) in budgets)
                {
                    if (cost <= 0)
                    {
                        break;
                    }

                    var availableAmount = getAvailableAmount(budgetContext);
                    if (SpendingLimit == SpendingLimit.NoLimit || cost <= availableAmount)
                    {
                        budgetDetail.BudgetReason = BudgetReason.CostCoveredInFull;
                        budgetDetail.CoveredCost = cost;
                        costAllocationAction(cost, budgetContext);
                        break;
                    }

                    if (Simulation.AnalysisMethod.ShouldUseExtraFundsAcrossBudgets)
                    {
                        budgetDetail.BudgetReason = BudgetReason.CostCoveredInPart;
                        budgetDetail.CoveredCost = availableAmount;
                        costAllocationAction(availableAmount, budgetContext);
                    }
                    else
                    {
                        budgetDetail.BudgetReason = BudgetReason.CostNotCovered;
                    }
                }

                if (cost < 0)
                {
                    throw new SimulationException(MessageStrings.RemainingCostIsNegative);
                }
            }
        }

        private void UpdateConditionActuals(int year)
        {
            TargetConditionActuals = GetTargetConditionActuals(year);
            DeficientConditionActuals = GetDeficientConditionActuals();
        }
    }
}
