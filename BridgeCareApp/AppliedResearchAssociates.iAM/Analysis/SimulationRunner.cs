﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AppliedResearchAssociates.iAM.DataAccess;
using AppliedResearchAssociates.iAM.Domains;
using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAM.Analysis
{
    public sealed class SimulationRunner
    {
        public SimulationRunner(Simulation simulation) => Simulation = simulation ?? throw new ArgumentNullException(nameof(simulation));

        public event EventHandler<FailureEventArgs> Failure;

        public event EventHandler<InformationEventArgs> Information;

        public event EventHandler<WarningEventArgs> Warning;

        public Simulation Simulation { get; }

        public void Run()
        {
            // During the execution of this method and its dependencies, the "SimulationException"
            // type is used for errors that are caused by invalid user input. Other types like
            // "InvalidOperationException" are used for errors that are caused by internal or
            // external programming mistakes (i.e. errors in this implementation or errors in other
            // code that uses this code).

            if (Interlocked.Exchange(ref StatusCode, STATUS_CODE_RUNNING) == STATUS_CODE_RUNNING)
            {
                throw new InvalidOperationException("Runner is already running.");
            }

            MessageBuilder = new SimulationMessageBuilder("Simulation initializing ...")
            {
                ItemName = Simulation.Name,
                ItemId = Simulation.Id,
            };

            Inform(MessageBuilder.ToString());

            var simulationValidationResults = Simulation.GetAllValidationResults();

            var numberOfErrors = simulationValidationResults.Count(result => result.Status == ValidationStatus.Error);
            if (numberOfErrors > 0)
            {
                MessageBuilder = new SimulationMessageBuilder($"Simulation has {numberOfErrors} validation errors.")
                {
                    ItemName = Simulation.Name,
                    ItemId = Simulation.Id,
                };

                Fail(MessageBuilder.ToString());
            }

            var numberOfWarnings = simulationValidationResults.Count(result => result.Status == ValidationStatus.Warning);
            if (numberOfWarnings > 0)
            {
                MessageBuilder = new SimulationMessageBuilder($"Simulation has {numberOfWarnings} validation warnings.")
                {
                    ItemName = Simulation.Name,
                    ItemId = Simulation.Id,
                };

                Warn(MessageBuilder.ToString());
            }

            ActiveTreatments = Simulation.GetActiveTreatments();

            try
            {
                BudgetContexts = Simulation.GetBudgetContextsWithCostAllocationsForCommittedProjects();
            }
            catch (SimulationException e)
            {
                Fail(e.Message, false);
                throw;
            }

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
                MessageBuilder = new SimulationMessageBuilder("Simulation filter passed no sections.")
                {
                    ItemName = Simulation.Name,
                    ItemId = Simulation.Id,
                };

                Fail(MessageBuilder.ToString());
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
                throw new InvalidOperationException(MessageStrings.InvalidSpendingStrategy);
            }

            ObjectiveFunction = Simulation.AnalysisMethod.ObjectiveFunction;

            Simulation.ClearResults();

            Simulation.Results.InitialConditionOfNetwork = Simulation.AnalysisMethod.Benefit.GetNetworkCondition(SectionContexts);
            Simulation.Results.InitialSectionSummaries.AddRange(SectionContexts.Select(context => context.SummaryDetail));

            foreach (var year in Simulation.InvestmentPlan.YearsOfAnalysis)
            {
                MessageBuilder = new SimulationMessageBuilder($"Simulating {year} ...")
                {
                    ItemName = Simulation.Name,
                    ItemId = Simulation.Id,
                };

                Inform(MessageBuilder.ToString());

                var unhandledContexts = ApplyRequiredEvents(year);
                var treatmentOptions = GetBeneficialTreatmentOptionsInOptimalOrder(unhandledContexts, year);
                ConsiderTreatmentOptions(unhandledContexts, treatmentOptions, year);

                Snapshot(year);
            }

            foreach (var treatment in Simulation.Treatments)
            {
                treatment.UnsetConsequencesPerAttribute();
            }

            MessageBuilder = new SimulationMessageBuilder("Simulation complete.")
            {
                ItemName = Simulation.Name,
                ItemId = Simulation.Id,
            };

            Inform(MessageBuilder.ToString());

            StatusCode = STATUS_CODE_NOT_RUNNING;
        }

        internal ILookup<Section, CommittedProject> CommittedProjectsPerSection { get; private set; }

        internal ILookup<NumberAttribute, PerformanceCurve> CurvesPerAttribute { get; private set; }

        internal IReadOnlyDictionary<string, NumberAttribute> NumberAttributeByName { get; private set; }

        internal void Fail(string message, bool shouldThrow = true)
        {
            OnFailure(new FailureEventArgs(message));

            if (shouldThrow)
            {
                throw new SimulationException(message);
            }
        }

        internal double GetInflationFactor(int year) => Simulation.InvestmentPlan.GetInflationFactor(year);

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

        private SimulationMessageBuilder MessageBuilder;

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

        private static bool TryConvertToDecimal(double value, out decimal convertedValue)
        {
            try
            {
                convertedValue = (decimal)value;
                return true;
            }
            catch (OverflowException)
            {
                convertedValue = default;
                return false;
            }
        }

        private ICollection<SectionContext> ApplyRequiredEvents(int year)
        {
            var unhandledContexts = new List<SectionContext>();

            foreach (var context in SectionContexts)
            {
                var yearIsScheduled = context.EventSchedule.TryGetValue(year, out var scheduledEvent);

                if (yearIsScheduled && scheduledEvent.IsT2(out var progress))
                {
                    if (Simulation.AnalysisMethod.ShouldDeteriorateDuringCashFlow)
                    {
                        context.ApplyPerformanceCurves();
                    }

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
                            MessageBuilder = new SimulationMessageBuilder($"Treatment scheduled for year {year} cannot be funded normally. Spending limits will be temporarily removed to fund this treatment.")
                            {
                                ItemName = treatment.Name,
                                ItemId = treatment.Id,
                            };

                            Warn(MessageBuilder.ToString());

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

        private void ConsiderTreatmentOptions(IEnumerable<SectionContext> baselineContexts, IEnumerable<TreatmentOption> treatmentOptions, int year)
        {
            var workingContextPerBaselineContext = baselineContexts.ToDictionary(_ => _, _ => new SectionContext(_));

            InParallel(workingContextPerBaselineContext, _ =>
            {
                var (baseline, working) = _;
                working.CopyDetailFrom(baseline);
            });

            InParallel(baselineContexts, context =>
            {
                context.Detail.TreatmentCause = TreatmentCause.NoSelection;
                context.EventSchedule.Add(year, Simulation.DesignatedPassiveTreatment);
                context.ApplyPassiveTreatment(year);
            });

            if (SpendingLimit != SpendingLimit.Zero && !ConditionGoalsAreMet(year))
            {
                foreach (var priority in BudgetPrioritiesPerYear[year])
                {
                    foreach (var context in BudgetContexts)
                    {
                        context.SetPriority(priority);
                    }

                    foreach (var option in treatmentOptions)
                    {
                        if (workingContextPerBaselineContext.TryGetValue(option.Context, out var workingContext) && priority.Criterion.EvaluateOrDefault(workingContext))
                        {
                            var costCoverage = TryToPayForTreatment(
                                workingContext,
                                option.CandidateTreatment,
                                year,
                                context => context.CurrentPrioritizedAmount ?? context.CurrentAmount);

                            var considerationDetail = workingContext.Detail.TreatmentConsiderations.Last();
                            considerationDetail.BudgetPriorityLevel = priority.PriorityLevel;

                            if (costCoverage == CostCoverage.None)
                            {
                                option.Context.Detail.TreatmentConsiderations.Add(considerationDetail);
                            }
                            else
                            {
                                _ = workingContextPerBaselineContext.Remove(option.Context);

                                _ = SectionContexts.Remove(option.Context);
                                SectionContexts.Add(workingContext);

                                workingContext.Detail.TreatmentCause = TreatmentCause.SelectedTreatment;

                                if (costCoverage == CostCoverage.Full)
                                {
                                    workingContext.EventSchedule.Add(year, option.CandidateTreatment);
                                    workingContext.ApplyTreatment(option.CandidateTreatment, year);

                                    if (ConditionGoalsAreMet(year))
                                    {
                                        return;
                                    }
                                }
                                else
                                {
                                    workingContext.MarkTreatmentProgress(option.CandidateTreatment);
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
                if (context.YearIsWithinShadowForAnyTreatment(year))
                {
                    var rejections = ActiveTreatments.Select(treatment => new TreatmentRejectionDetail(treatment.Name, TreatmentRejectionReason.WithinShadowForAnyTreatment));
                    context.Detail.TreatmentRejections.AddRange(rejections);
                    return;
                }

                var feasibleTreatments = ActiveTreatments.ToHashSet();

                _ = feasibleTreatments.RemoveWhere(treatment =>
                {
                    var isRejected = context.YearIsWithinShadowForSameTreatment(year, treatment);
                    if (isRejected)
                    {
                        context.Detail.TreatmentRejections.Add(new TreatmentRejectionDetail(treatment.Name, TreatmentRejectionReason.WithinShadowForSameTreatment));
                    }

                    return isRejected;
                });

                _ = feasibleTreatments.RemoveWhere(treatment =>
                {
                    var isFeasible = treatment.IsFeasible(context);
                    if (!isFeasible)
                    {
                        context.Detail.TreatmentRejections.Add(new TreatmentRejectionDetail(treatment.Name, TreatmentRejectionReason.NotFeasible));
                    }

                    return !isFeasible;
                });

                var supersededTreatmentsQuery =
                    from treatment in feasibleTreatments
                    from supersession in treatment.Supersessions
                    where supersession.Criterion.EvaluateOrDefault(context)
                    select supersession.Treatment;

                var supersededTreatments = supersededTreatmentsQuery.ToHashSet();

                _ = feasibleTreatments.RemoveWhere(treatment =>
                {
                    var isSuperseded = supersededTreatments.Contains(treatment);
                    if (isSuperseded)
                    {
                        context.Detail.TreatmentRejections.Add(new TreatmentRejectionDetail(treatment.Name, TreatmentRejectionReason.Superseded));
                    }

                    return isSuperseded;
                });

                _ = feasibleTreatments.RemoveWhere(treatment =>
                {
                    var cost = context.GetCostOfTreatment(treatment);
                    if (TryConvertToDecimal(cost, out var convertedCost) && convertedCost > 0)
                    {
                        if (convertedCost < Simulation.InvestmentPlan.MinimumProjectCostLimit)
                        {
                            context.Detail.TreatmentRejections.Add(new TreatmentRejectionDetail(treatment.Name, TreatmentRejectionReason.CostIsBelowMinimumProjectCostLimit));
                            return true;
                        }

                        return false;
                    }

                    context.Detail.TreatmentRejections.Add(new TreatmentRejectionDetail(treatment.Name, TreatmentRejectionReason.InvalidCost));
                    return true;
                });

                if (feasibleTreatments.Count > 0)
                {
                    var remainingLifeCalculatorFactories = Enumerable.ToArray(
                        from limit in Simulation.AnalysisMethod.RemainingLifeLimits
                        where limit.Criterion.EvaluateOrDefault(context)
                        group limit.Value by limit.Attribute into attributeLimitValues
                        select new RemainingLifeCalculator.Factory(attributeLimitValues));

                    var baselineOutlook = new TreatmentOutlook(this, context, Simulation.DesignatedPassiveTreatment, year, remainingLifeCalculatorFactories);

                    foreach (var treatment in feasibleTreatments)
                    {
                        var outlook = new TreatmentOutlook(this, context, treatment, year, remainingLifeCalculatorFactories);
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
                .OrderByDescending(_ => _.value * _.option.Context.GetSpatialWeight())
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

                var goalSpatialWeight = goalContexts.Sum(context => context.GetSpatialWeight());
                var deficientContexts = goalContexts.Where(context => goal.LevelIsDeficient(context.GetNumber(goal.Attribute.Name)));
                var deficientSpatialWeight = deficientContexts.Sum(context => context.GetSpatialWeight());
                var deficientPercentageActual = deficientSpatialWeight / goalSpatialWeight * 100;

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

                var goalSpatialWeight = goalContexts.Sum(context => context.GetSpatialWeight());
                var averageActual = goalContexts.Sum(context => context.GetNumber(goal.Attribute.Name) * context.GetSpatialWeight()) / goalSpatialWeight;

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

        private void OnFailure(FailureEventArgs e) => Failure?.Invoke(this, e);

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

        private void Snapshot(int year)
        {
            var yearDetail = Simulation.Results.Years.GetAdd(new SimulationYearDetail(year));

            yearDetail.Budgets.AddRange(BudgetContexts.Select(context => new BudgetDetail(context.Budget, context.CurrentAmount)));
            yearDetail.ConditionOfNetwork = Simulation.AnalysisMethod.Benefit.GetNetworkCondition(SectionContexts);

            RecordStatusOfConditionGoals(yearDetail);

            InParallel(SectionContexts, context => context.CopyAttributeValuesToDetail());
            yearDetail.Sections.AddRange(SectionContexts.Select(context => context.Detail));
            InParallel(SectionContexts, context => context.ResetDetail());

            if (year < Simulation.InvestmentPlan.LastYearOfAnalysisPeriod)
            {
                MoveBudgetsToNextYear();
            }
        }

        private CostCoverage TryToPayForTreatment(SectionContext sectionContext, Treatment treatment, int year, Func<BudgetContext, decimal> getAvailableAmount)
        {
            var treatmentCost = sectionContext.GetCostOfTreatment(treatment);

            // This variable is updated as payment for the treatment is arranged.
            var remainingCost = (decimal)(treatmentCost * GetInflationFactor(year));

            var treatmentConsideration = new TreatmentConsiderationDetail(treatment.Name);

            treatmentConsideration.BudgetUsages.AddRange(BudgetContexts.Select(budgetContext => new BudgetUsageDetail(budgetContext.Budget.Name)
            {
                Status = treatment.CanUseBudget(budgetContext.Budget) ? BudgetUsageStatus.NotNeeded : BudgetUsageStatus.NotUsable
            }));

            if (treatment is CommittedProject)
            {
                sectionContext.Detail.TreatmentConsiderations.Add(treatmentConsideration);
                var budgetUsageDetail = treatmentConsideration.BudgetUsages.Single(budgetUsage => budgetUsage.Status != BudgetUsageStatus.NotUsable);
                budgetUsageDetail.Status = BudgetUsageStatus.CostCoveredInFull;
                budgetUsageDetail.CoveredCost = (decimal)treatmentCost; // Cost is assumed to already include all appropriate adjustments, e.g. for inflation.
                return CostCoverage.Full;
            }

            var applicableBudgets = Zip.Strict(BudgetContexts, treatmentConsideration.BudgetUsages).Where(budgetIsApplicable).ToArray();

            bool budgetIsApplicable((BudgetContext, BudgetUsageDetail) _)
            {
                var (budgetContext, budgetUsageDetail) = _;

                if (budgetUsageDetail.Status == BudgetUsageStatus.NotUsable)
                {
                    return false;
                }

                var budgetConditions = ConditionsPerBudget[budgetContext.Budget];
                var budgetConditionIsMet = treatment is CommittedProject || budgetConditions.Count() == 0 || budgetConditions.Any(condition => condition.Criterion.EvaluateOrDefault(sectionContext));
                if (!budgetConditionIsMet)
                {
                    budgetUsageDetail.Status = BudgetUsageStatus.ConditionNotMet;
                    return false;
                }

                return true;
            }

            var cashFlowConsiderations = new List<CashFlowConsiderationDetail>();

            if (remainingCost > 0)
            {
                // First, attempt any applicable cash flows.

                Action scheduleCashFlowEvents = null;

                var applicableCashFlowRules = Simulation.InvestmentPlan.CashFlowRules.Where(rule => rule.Criterion.EvaluateOrDefault(sectionContext));
                foreach (var cashFlowRule in applicableCashFlowRules)
                {
                    var cashFlowConsideration = cashFlowConsiderations.GetAdd(new CashFlowConsiderationDetail(cashFlowRule.Name));

                    cashFlowConsideration.ReasonAgainstCashFlow = scheduleCashFlowEvents is null
                        ? handleCashFlowRule()
                        : ReasonAgainstCashFlow.NotNeeded;

                    ReasonAgainstCashFlow handleCashFlowRule()
                    {
                        var distributionRule = SortedDistributionRulesPerCashFlowRule[cashFlowRule].FirstOrDefault(kv => remainingCost <= kv.Key).Value;
                        if (distributionRule is null)
                        {
                            return ReasonAgainstCashFlow.NoApplicableDistributionRule;
                        }

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
                        var considerationPerYear = costPerYear.Select(_ => new TreatmentConsiderationDetail(treatmentConsideration)).ToList();

                        var workingBudgetContexts = applicableBudgets.Select(_ => new BudgetContext(_.Item1)).ToArray();
                        var applicableBudgetNames = applicableBudgets.Select(_ => _.Item2.BudgetName).ToHashSet();

                        var originalBudgetContextPerWorkingBudgetContext = new Dictionary<BudgetContext, BudgetContext>();
                        foreach (var ((original, _), working) in Zip.Strict(applicableBudgets, workingBudgetContexts))
                        {
                            originalBudgetContextPerWorkingBudgetContext[working] = original;
                        }

                        BudgetContext firstYearBudget = null;

                        foreach (var (futureYearConsideration, futureYearCost, futureYear) in Zip.Short(considerationPerYear, costPerYear, Static.Count(year)))
                        {
                            foreach (var budgetContext in workingBudgetContexts)
                            {
                                budgetContext.SetYear(futureYear);
                            }

                            var workingBudgetDetails = futureYearConsideration.BudgetUsages.Where(detail => applicableBudgetNames.Contains(detail.BudgetName));
                            var workingBudgets = Zip.Strict(workingBudgetContexts, workingBudgetDetails).ToList();

                            if (firstYearBudget is object)
                            {
                                _ = workingBudgets.RemoveAll(pair => pair.Item1 != firstYearBudget);
                            }

                            var remainingYearCost = futureYearCost;

                            void addFutureCostAllocator(decimal cost, BudgetContext workingBudgetContext)
                            {
                                remainingYearCost -= cost;
                                workingBudgetContext.AllocateCost(cost);

                                var originalBudgetContext = originalBudgetContextPerWorkingBudgetContext[workingBudgetContext];
                                futureCostAllocators.Add(() => originalBudgetContext.AllocateCost(cost, futureYear));
                            }

                            tryToAllocateCost(workingBudgets, ref remainingYearCost, addFutureCostAllocator);

                            if (futureYear == year && Simulation.AnalysisMethod.ShouldRestrictCashFlowToFirstYearBudget)
                            {
                                firstYearBudget = workingBudgets.SingleOrDefault(pair => pair.Item2.Status == BudgetUsageStatus.CostCoveredInFull).Item1;
                            }

                            if (remainingYearCost > 0)
                            {
                                return ReasonAgainstCashFlow.FundingIsNotAvailable;
                            }
                        }

                        considerationPerYear[0].CashFlowConsiderations.AddRange(cashFlowConsiderations);
                        sectionContext.Detail.TreatmentConsiderations.Add(considerationPerYear[0]);

                        var progression = costPerYear.Zip(considerationPerYear, (cost, consideration) => new TreatmentProgress(treatment, consideration)).ToArray();
                        progression.Last().IsComplete = true;

                        scheduleCashFlowEvents = () =>
                        {
                            foreach (var futureCostAllocator in futureCostAllocators)
                            {
                                futureCostAllocator();
                            }

                            foreach (var (yearProgress, yearOffset) in Zip.Short(progression, Static.Count()))
                            {
                                sectionContext.EventSchedule.Add(year + yearOffset, yearProgress);
                            }
                        };

                        return ReasonAgainstCashFlow.None;
                    }
                }

                if (scheduleCashFlowEvents is object)
                {
                    scheduleCashFlowEvents();
                    return CostCoverage.CashFlow;
                }
            }

            // At this point, no cash flow could be used. So try to pay the normal way.

            treatmentConsideration.CashFlowConsiderations.AddRange(cashFlowConsiderations);
            sectionContext.Detail.TreatmentConsiderations.Add(treatmentConsideration);

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

            return CostCoverage.Full;

            void tryToAllocateCost(IEnumerable<(BudgetContext, BudgetUsageDetail)> budgets, ref decimal cost, Action<decimal, BudgetContext> costAllocationAction)
            // "cost" is a variable that is being *indirectly* updated by "costAllocationAction".
            {
                foreach (var (budgetContext, budgetUsageDetail) in budgets)
                {
                    if (cost <= 0)
                    {
                        break;
                    }

                    var availableAmount = getAvailableAmount(budgetContext);
                    if (SpendingLimit == SpendingLimit.NoLimit || cost <= availableAmount)
                    {
                        budgetUsageDetail.Status = BudgetUsageStatus.CostCoveredInFull;
                        budgetUsageDetail.CoveredCost = cost;
                        costAllocationAction(cost, budgetContext);
                        break;
                    }

                    if (Simulation.AnalysisMethod.ShouldUseExtraFundsAcrossBudgets)
                    {
                        budgetUsageDetail.Status = BudgetUsageStatus.CostCoveredInPart;
                        budgetUsageDetail.CoveredCost = availableAmount;
                        costAllocationAction(availableAmount, budgetContext);
                    }
                    else
                    {
                        budgetUsageDetail.Status = BudgetUsageStatus.CostNotCovered;
                    }
                }

                if (cost < 0)
                {
                    throw new InvalidOperationException(MessageStrings.RemainingCostIsNegative);
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
