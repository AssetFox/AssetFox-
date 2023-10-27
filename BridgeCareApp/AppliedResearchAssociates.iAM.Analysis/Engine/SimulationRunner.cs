//#define dump_analysis_input
//#define dump_analysis_output

#if dump_analysis_input || dump_analysis_output
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using AppliedResearchAssociates.iAM.Analysis.Input.DataTransfer;
#endif

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AppliedResearchAssociates.iAM.DTOs.Enums;
using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAM.Analysis.Engine;

public sealed class SimulationRunner
{
    public SimulationRunner(Simulation simulation) => Simulation = simulation ?? throw new ArgumentNullException(nameof(simulation));

    public event EventHandler<ProgressEventArgs> Progress;

    public event EventHandler<SimulationLogEventArgs> SimulationLog;

#if !DEBUG
    private static readonly int MaxThreadsForSimulation = GetMaxThreadsForSimulation();

    private static int GetMaxThreadsForSimulation()
    {
        int processorCount = Environment.ProcessorCount;
        return processorCount >= 8 ? processorCount - 2 : processorCount - 1;
    }
#endif

    public Simulation Simulation { get;  }

    public void HandleValidationFailures(ValidationResultBag simulationValidationResults)
    {
        var numberOfErrors = simulationValidationResults.Count(result => result.Status == ValidationStatus.Error);
        if (numberOfErrors > 0)
        {
            var errorsWord = numberOfErrors == 1 ? "error" : "errors";
            MessageBuilder = new SimulationMessageBuilder($"Simulation has {numberOfErrors} validation {errorsWord}. Download the log to see all validation results.")
            {
                ItemName = Simulation.Name,
                ItemId = Simulation.Id,
            };

            var logMessageBuilder = SimulationLogMessageBuilders.HasValidationErrors(MessageBuilder, Simulation.Id);
            Send(logMessageBuilder);
        }

        var numberOfWarnings = simulationValidationResults.Count(result => result.Status == ValidationStatus.Warning);
        if (numberOfWarnings > 0)
        {
            var warningsWord = numberOfWarnings == 1 ? "warning" : "warnings";
            MessageBuilder = new SimulationMessageBuilder($"Simulation has {numberOfWarnings} validation {warningsWord}.")
            {
                ItemName = Simulation.Name,
                ItemId = Simulation.Id,
            };

            var logBuilder = new SimulationLogMessageBuilder
            {
                Message = MessageBuilder.ToString(),
                SimulationId = Simulation.Id,
                Status = SimulationLogStatus.Warning,
                Subject = SimulationLogSubject.Validation,
            };

            Send(logBuilder);
        }
    }

    public void Run(bool withValidation = true, CancellationToken cancellationToken = default)
    {
#if dump_analysis_input
        var inputDumpPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
            "iAM analysis dumps",
            $"{DateTime.Now:yyyy-MM-dd-HHmmssfff} iAM analysis input dump.json");

        _ = Directory.CreateDirectory(Path.GetDirectoryName(inputDumpPath));

        var inputData = Scenario.ConvertIn(Simulation);
        var inputJson = JsonSerializer.Serialize(inputData, new JsonSerializerOptions
        {
            WriteIndented = true,
            Converters = { new JsonStringEnumConverter() },
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        });

        File.WriteAllText(inputDumpPath, inputJson);
#endif

        // During the execution of this method and its dependencies, the "SimulationException"
        // type is used for errors that are caused by invalid user input. Other types like
        // "InvalidOperationException" are used for errors that are caused by internal or
        // external programming mistakes (i.e. errors in this implementation or errors in other
        // code that uses this code).

        if (Interlocked.Exchange(ref StatusCode, STATUS_CODE_RUNNING) == STATUS_CODE_RUNNING)
        {
            throw new InvalidOperationException("Runner is already running.");
        }

        ReportProgress(ProgressStatus.Started);

        if (withValidation)
        {
            RunValidation();
        }

        ActiveTreatments = Simulation.GetActiveTreatments();

        try
        {
            BudgetContexts = Simulation.GetBudgetContextsWithCostAllocationsForCommittedProjects();
        }
        catch (SimulationException e)
        {
            var logMessage = SimulationLogMessageBuilders.Exception(e, Simulation.Id);
            Send(logMessage, false);
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

        CommittedProjectsPerAsset = Simulation.CommittedProjects.ToLookup(committedProject => committedProject.Asset);
        ConditionsPerBudget = Simulation.InvestmentPlan.BudgetConditions.ToLookup(budgetCondition => budgetCondition.Budget);
        CurvesPerAttribute = Simulation.PerformanceCurves.ToLookup(curve => curve.Attribute);
        NumberAttributeByName = Simulation.Network.Explorer.NumberAttributes.ToDictionary(attribute => attribute.Name, StringComparer.OrdinalIgnoreCase);
        SortedDistributionRulesPerCashFlowRule = Simulation.InvestmentPlan.CashFlowRules.ToDictionary(_ => _, rule => rule.DistributionRules.ToSortedDictionary(distributionRule => distributionRule.CostCeiling ?? decimal.MaxValue));

        foreach (var treatment in Simulation.Treatments)
        {
            treatment.SetConsequencesPerAttribute();
        }

        AssetContexts = Simulation.Network.Assets
#if !DEBUG
            .AsParallel()
            .WithDegreeOfParallelism(MaxThreadsForSimulation)
#endif
            .Select(asset => new AssetContext(asset, this))
            .Where(context => Simulation.AnalysisMethod.Filter.EvaluateOrDefault(context))
            .ToSortedSet(SelectionComparer<AssetContext>.Create(context => context.Asset.Id));

        if (AssetContexts.Count == 0)
        {
            MessageBuilder = new SimulationMessageBuilder("Simulation filter passed no assets.")
            {
                ItemName = Simulation.Name,
                ItemId = Simulation.Id,
            };

            var logMessage = SimulationLogMessageBuilders.RuntimeFatal(MessageBuilder, Simulation.Id);
            Send(logMessage);
        }

        var rollForwardEvents = new ConcurrentBag<RollForwardEventDetail>();
        InParallel(AssetContexts, context =>
        {
            context.RollForward(rollForwardEvents);
            context.Asset.HistoryProvider.ClearHistory();
        });

        SpendingLimit = Simulation.AnalysisMethod.SpendingLimit;

        ConditionGoalsEvaluator = Simulation.AnalysisMethod.SpendingStrategy switch
        {
            SpendingStrategy.NoSpending => () => false,
            SpendingStrategy.UnlimitedSpending => () => false,
            SpendingStrategy.UntilTargetAndDeficientConditionGoalsMet => () => GoalsAreMet(TargetConditionActuals) && GoalsAreMet(DeficientConditionActuals),
            SpendingStrategy.UntilTargetConditionGoalsMet => () => GoalsAreMet(TargetConditionActuals),
            SpendingStrategy.UntilDeficientConditionGoalsMet => () => GoalsAreMet(DeficientConditionActuals),
            SpendingStrategy.AsBudgetPermits => () => false,
            _ => throw new InvalidOperationException(MessageStrings.InvalidSpendingStrategy),
        };

        ObjectiveFunction = Simulation.AnalysisMethod.ObjectiveFunction;

        Simulation.ClearResults();

        SimulationOutput output = new();
        output.RollForwardEvents.AddRange(rollForwardEvents.OrderBy(e => e.Year).ThenBy(e => e.AssetId));

        output.InitialConditionOfNetwork = Simulation.AnalysisMethod.Benefit.GetNetworkCondition(AssetContexts);
        output.InitialAssetSummaries.AddRange(AssetContexts.Select(context => context.SummaryDetail));

        Simulation.ResultsOnDisk.Initialize(output);
        output = null;

        foreach (var year in Simulation.InvestmentPlan.YearsOfAnalysis)
        {
            if (CheckCanceled(cancellationToken))
            {
                return;
            }

            var percentComplete = (double)(year - Simulation.InvestmentPlan.FirstYearOfAnalysisPeriod) / Simulation.InvestmentPlan.NumberOfYearsInAnalysisPeriod * 100;
            ReportProgress(ProgressStatus.Running, percentComplete, year);

            var unhandledContexts = ApplyRequiredEvents(year);

            if (CheckCanceled(cancellationToken))
            {
                return;
            }

            var treatmentOptions = GetBeneficialTreatmentOptionsInOptimalOrder(unhandledContexts, year);

            if (CheckCanceled(cancellationToken))
            {
                return;
            }

            ConsiderTreatmentOptions(unhandledContexts, treatmentOptions, year);

            if (CheckCanceled(cancellationToken))
            {
                return;
            }

            treatmentOptions = null;

            InParallel(AssetContexts, context =>
            {
                context.ApplyTreatmentMetadataIfPending(year);
                context.UnfixCalculatedFieldValues();
            });

            if (CheckCanceled(cancellationToken))
            {
                return;
            }

            Snapshot(year);
        }

        foreach (var treatment in Simulation.Treatments)
        {
            treatment.UnsetConsequencesPerAttribute();
        }

        ReportProgress(ProgressStatus.Completed, 100);

        StatusCode = STATUS_CODE_NOT_RUNNING;

#if dump_analysis_output
        var outputDumpPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
            "iAM analysis dumps",
            $"{DateTime.Now:yyyy-MM-dd-HHmmssfff} iAM analysis output dump.json");

        _ = Directory.CreateDirectory(Path.GetDirectoryName(outputDumpPath));

        var outputData = Simulation.Results;
        var outputJson = JsonSerializer.Serialize(outputData, new JsonSerializerOptions
        {
            WriteIndented = true,
            Converters = { new JsonStringEnumConverter() },
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        });

        File.WriteAllText(outputDumpPath, outputJson);
#endif

        bool CheckCanceled(CancellationToken cancellationToken)
        {
            var canceled = cancellationToken.IsCancellationRequested;
            if (canceled)
            {
                ReportProgress(ProgressStatus.Canceled);
            }
            return canceled;
        }
    }

    public ValidationResultBag RunValidation()
    {
        var simulationValidationResults = Simulation.GetAllValidationResults(Enumerable.Empty<string>());

        HandleValidationFailures(simulationValidationResults);
        return simulationValidationResults;
    }

    internal ILookup<AnalysisMaintainableAsset, CommittedProject> CommittedProjectsPerAsset { get; private set; }

    internal ILookup<NumberAttribute, PerformanceCurve> CurvesPerAttribute { get; private set; }

    internal IReadOnlyDictionary<string, NumberAttribute> NumberAttributeByName { get; private set; }

    internal double GetInflationFactor(int year) => Simulation.InvestmentPlan.GetInflationFactor(year);

    internal void ReportProgress(ProgressStatus progressStatus, double percentComplete = 0, int? year = null) => OnProgress(new ProgressEventArgs(progressStatus, percentComplete, year));

    internal void Send(SimulationLogMessageBuilder message, bool throwOnFatal = true)
    {
        var args = new SimulationLogEventArgs(message);
        OnLog(args);
        if (message.Status == SimulationLogStatus.Fatal && throwOnFatal)
        {
            throw new SimulationException(message.Message);
        }
    }

    private const int STATUS_CODE_NOT_RUNNING = 0;

    private const int STATUS_CODE_RUNNING = 1;

    private static readonly IComparer<BudgetPriority> BudgetPriorityComparer = SelectionComparer<BudgetPriority>.Create(priority => priority.PriorityLevel);

    private IReadOnlyCollection<SelectableTreatment> ActiveTreatments;

    private IReadOnlyCollection<BudgetContext> BudgetContexts;

    private IReadOnlyDictionary<int, IEnumerable<BudgetPriority>> BudgetPrioritiesPerYear;

    private Func<bool> ConditionGoalsEvaluator;

    private ILookup<Budget, BudgetCondition> ConditionsPerBudget;

    private IReadOnlyCollection<ConditionActual> DeficientConditionActuals = Array.Empty<ConditionActual>();

    private SimulationMessageBuilder MessageBuilder;

    private Func<TreatmentOption, double> ObjectiveFunction;

    private ICollection<AssetContext> AssetContexts;

    private IReadOnlyDictionary<CashFlowRule, SortedDictionary<decimal, CashFlowDistributionRule>> SortedDistributionRulesPerCashFlowRule;

    private SpendingLimit SpendingLimit;

    private int StatusCode;

    private IReadOnlyCollection<ConditionActual> TargetConditionActuals = Array.Empty<ConditionActual>();

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
        _ = System.Threading.Tasks.Parallel.ForEach(items, new System.Threading.Tasks.ParallelOptions { MaxDegreeOfParallelism = MaxThreadsForSimulation }, action);
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

    private ICollection<AssetContext> ApplyRequiredEvents(int year)
    {
        var unhandledContexts = new List<AssetContext>();

        foreach (var context in AssetContexts)
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
                context.PrepareForTreatment();

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

                        var warning = SimulationLogMessageBuilders.RuntimeWarning(MessageBuilder, Simulation.Id);
                        Send(warning);

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

                    if (treatment is CommittedProject committedProject)
                    {
                        context.Detail.ProjectSource = committedProject.ProjectSource.ToString();
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

    private void ConsiderTreatmentOptions(IEnumerable<AssetContext> baselineContexts, IEnumerable<TreatmentOption> treatmentOptions, int year)
    {
        var workingContextPerBaselineContext = baselineContexts.ToDictionary(_ => _, _ => new AssetContext(_));

        InParallel(workingContextPerBaselineContext, _ =>
        {
            var (baseline, working) = _;
            working.CopyDetailFrom(baseline);
        });

        InParallel(baselineContexts, context =>
        {
            context.Detail.TreatmentCause = TreatmentCause.NoSelection;
            context.EventSchedule.Add(year, Simulation.DesignatedPassiveTreatment);

            if (!Simulation.ShouldPreapplyPassiveTreatment)
            {
                context.ApplyPassiveTreatment(year);
            }
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
                    var optionContextIsPending = workingContextPerBaselineContext.TryGetValue(option.Context, out var workingContext);
                    if (optionContextIsPending && priority.Criterion.EvaluateOrDefault(workingContext))
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

                            _ = AssetContexts.Remove(option.Context);
                            AssetContexts.Add(workingContext);

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

    private IReadOnlyCollection<TreatmentOption> GetBeneficialTreatmentOptionsInOptimalOrder(IEnumerable<AssetContext> contexts, int year)
    {
        var treatmentOptionsBag = new ConcurrentBag<TreatmentOption>();
        void addTreatmentOptions(AssetContext context)
        {
            if (context.YearIsWithinShadowForAnyTreatment(year))
            {
                var rejections = ActiveTreatments.Select(treatment => new TreatmentRejectionDetail(treatment.Name, TreatmentRejectionReason.WithinShadowForAnyTreatment, getBenefitImprovement(context, treatment)));
                context.Detail.TreatmentRejections.AddRange(rejections);
                return;
            }

            var feasibleTreatments = ActiveTreatments.ToHashSet();

            _ = feasibleTreatments.RemoveWhere(treatment =>
            {
                var isRejected = context.YearIsWithinShadowForSameTreatment(year, treatment);
                if (isRejected)
                {
                    context.Detail.TreatmentRejections.Add(new TreatmentRejectionDetail(treatment.Name, TreatmentRejectionReason.WithinShadowForSameTreatment, getBenefitImprovement(context, treatment)));
                }

                return isRejected;
            });

            _ = feasibleTreatments.RemoveWhere(treatment =>
            {
                var isFeasible = treatment.IsFeasible(context);
                if (!isFeasible)
                {
                    context.Detail.TreatmentRejections.Add(new TreatmentRejectionDetail(treatment.Name, TreatmentRejectionReason.NotFeasible, getBenefitImprovement(context, treatment)));
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
                    context.Detail.TreatmentRejections.Add(new TreatmentRejectionDetail(treatment.Name, TreatmentRejectionReason.Superseded, getBenefitImprovement(context, treatment)));
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
                        context.Detail.TreatmentRejections.Add(new TreatmentRejectionDetail(treatment.Name, TreatmentRejectionReason.CostIsBelowMinimumProjectCostLimit, getBenefitImprovement(context, treatment)));
                        return true;
                    }

                    return false;
                }

                context.Detail.TreatmentRejections.Add(new TreatmentRejectionDetail(treatment.Name, TreatmentRejectionReason.InvalidCost, getBenefitImprovement(context, treatment)));
                var messageBuilder = SimulationLogMessageBuilders.InvalidTreatmentCost(context.Asset, treatment, cost, context.SimulationRunner.Simulation.Id);
                Send(messageBuilder);
                return true;
            });

            static double getBenefitImprovement(AssetContext context, Treatment treatment)
            {
                var copyOfContext = new AssetContext(context);
                var benefitBeforeTreatment = copyOfContext.GetBenefit(false);
                copyOfContext.ApplyTreatmentConsequences(treatment);
                var benefitAfterTreatment = copyOfContext.GetBenefit(false);
                return benefitAfterTreatment - benefitBeforeTreatment;
            }

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

        treatmentOptionsBag.Clear();
        treatmentOptionsBag = null;

        return treatmentOptions;
    }

    private IReadOnlyCollection<ConditionActual> GetDeficientConditionActuals()
    {
        var results = new List<ConditionActual>();

        foreach (var goal in Simulation.AnalysisMethod.DeficientConditionGoals)
        {
            var goalContexts = AssetContexts
#if !DEBUG
                .AsParallel()
                .WithDegreeOfParallelism(MaxThreadsForSimulation)
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

            var goalContexts = AssetContexts
#if !DEBUG
                .AsParallel()
                .WithDegreeOfParallelism(MaxThreadsForSimulation)
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

    private void OnLog(SimulationLogEventArgs e) => SimulationLog?.Invoke(this, e);

    private void OnProgress(ProgressEventArgs e) => Progress?.Invoke(this, e);

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
        SimulationYearDetail yearDetail = new(year);

        yearDetail.Budgets.AddRange(BudgetContexts.Select(context => new BudgetDetail(context.Budget, context.CurrentAmount)));
        yearDetail.ConditionOfNetwork = Simulation.AnalysisMethod.Benefit.GetNetworkCondition(AssetContexts);

        RecordStatusOfConditionGoals(yearDetail);

        InParallel(AssetContexts, context => context.CopyAttributeValuesToDetail());
        yearDetail.Assets.AddRange(AssetContexts.Select(context => context.Detail));
        InParallel(AssetContexts, context => context.ResetDetail());

        if (year < Simulation.InvestmentPlan.LastYearOfAnalysisPeriod)
        {
            MoveBudgetsToNextYear();
        }

        Simulation.ResultsOnDisk.AddYearDetail(yearDetail);
    }

    private CostCoverage TryToPayForTreatment(AssetContext assetContext, Treatment treatment, int year, Func<BudgetContext, decimal> getAvailableAmount)
    {
        var treatmentCost = assetContext.GetCostOfTreatment(treatment);

        // This variable is updated as payment for the treatment is arranged.
        var remainingCost = (decimal)(treatmentCost * GetInflationFactor(year));

        var treatmentConsideration = new TreatmentConsiderationDetail(treatment.Name);

        treatmentConsideration.BudgetsAtDecisionTime.AddRange(
            BudgetContexts.Select(context => new BudgetDetail(context.Budget, context.CurrentAmount)));

        treatmentConsideration.BudgetUsages.AddRange(BudgetContexts.Select(budgetContext => new BudgetUsageDetail(budgetContext.Budget.Name)
        {
            Status = treatment.CanUseBudget(budgetContext.Budget) ? BudgetUsageStatus.NotNeeded : BudgetUsageStatus.NotUsable
        }));

        if (treatment is CommittedProject)
        {
            assetContext.Detail.TreatmentConsiderations.Add(treatmentConsideration);
            var budgetUsageDetail = treatmentConsideration.BudgetUsages.Single(budgetUsage => budgetUsage.Status != BudgetUsageStatus.NotUsable);
            budgetUsageDetail.Status = BudgetUsageStatus.CostCovered;
            budgetUsageDetail.CoveredCost = (decimal)treatmentCost; // Cost is assumed to already include all appropriate adjustments, e.g. for inflation.
            return CostCoverage.Full;
        }

        var applicableBudgets = BudgetContexts.Zip(treatmentConsideration.BudgetUsages, BudgetInfo.Create).Where(budgetIsApplicable).ToArray();

        bool budgetIsApplicable(BudgetInfo info)
        {
            var (budgetContext, budgetUsageDetail) = info;

            if (budgetUsageDetail.Status == BudgetUsageStatus.NotUsable)
            {
                return false;
            }

            var budgetConditions = ConditionsPerBudget[budgetContext.Budget];
            var budgetConditionIsMet = treatment is CommittedProject || !budgetConditions.Any() || budgetConditions.Any(condition => condition.Criterion.EvaluateOrDefault(assetContext));
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

            var applicableCashFlowRules = Simulation.InvestmentPlan.CashFlowRules.Where(rule => rule.Criterion.EvaluateOrDefault(assetContext));
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

                    var scheduleIsBlocked = Static.RangeFromBounds(year + 1, lastYearOfCashFlow).Any(assetContext.EventSchedule.ContainsKey);
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

                    var workingBudgetContexts = applicableBudgets.Select(_ => new BudgetContext(_.Context)).ToArray();
                    var applicableBudgetNames = applicableBudgets.Select(_ => _.UsageDetail.BudgetName).ToHashSet();

                    var originalBudgetContextPerWorkingBudgetContext = new Dictionary<BudgetContext, BudgetContext>();
                    foreach (var ((original, _), working) in Zip.Strict(applicableBudgets, workingBudgetContexts))
                    {
                        originalBudgetContextPerWorkingBudgetContext[working] = original;
                    }

                    Dictionary<BudgetContext, decimal> firstYearFractionPerBudget = null;

                    foreach (var (futureYearConsideration, futureYearCost, futureYear) in Zip.Short(considerationPerYear, costPerYear, Static.Count(year)))
                    {
                        foreach (var budgetContext in workingBudgetContexts)
                        {
                            budgetContext.SetYear(futureYear);
                        }

                        var workingBudgetDetails = futureYearConsideration.BudgetUsages.Where(detail => applicableBudgetNames.Contains(detail.BudgetName));
                        var workingBudgets = workingBudgetContexts.Zip(workingBudgetDetails, BudgetInfo.Create).ToList();

                        var remainingYearCost = futureYearCost;

                        void addFutureCostAllocator(decimal cost, BudgetContext workingBudgetContext)
                        {
                            remainingYearCost -= cost;
                            workingBudgetContext.AllocateCost(cost);

                            var originalBudgetContext = originalBudgetContextPerWorkingBudgetContext[workingBudgetContext];
                            futureCostAllocators.Add(() => originalBudgetContext.AllocateCost(cost, futureYear));
                        }

                        tryToAllocateCost(
                            workingBudgets,
                            ref remainingYearCost,
                            addFutureCostAllocator,
                            firstYearFractionPerBudget,
                            out var costCoverageFractionsWereSatisfied);

                        if (firstYearFractionPerBudget is not null && !costCoverageFractionsWereSatisfied)
                        {
                            return ReasonAgainstCashFlow.FirstYearFundingPatternFailedInFutureYear;
                        }

                        if (remainingYearCost > 0)
                        {
                            return ReasonAgainstCashFlow.FundingIsNotAvailable;
                        }

                        if (futureYear == year && Simulation.AnalysisMethod.ShouldRestrictCashFlowToFirstYearBudgets)
                        {
                            firstYearFractionPerBudget = workingBudgets
                                .Where(info => info.UsageDetail.Status == BudgetUsageStatus.CostCovered)
                                .ToDictionary(info => info.Context, info => info.UsageDetail.CoveredCost / futureYearCost);
                        }
                    }

                    considerationPerYear[0].CashFlowConsiderations.AddRange(cashFlowConsiderations);
                    assetContext.Detail.TreatmentConsiderations.Add(considerationPerYear[0]);

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
                            assetContext.EventSchedule.Add(year + yearOffset, yearProgress);
                        }
                    };

                    return ReasonAgainstCashFlow.None;
                }
            }

            if (scheduleCashFlowEvents is not null)
            {
                scheduleCashFlowEvents();
                return CostCoverage.CashFlow;
            }
        }

        // At this point, no cash flow could be used. So try to pay the normal way.

        treatmentConsideration.CashFlowConsiderations.AddRange(cashFlowConsiderations);
        assetContext.Detail.TreatmentConsiderations.Add(treatmentConsideration);

        var costAllocators = new List<Action>();
        void addCostAllocator(decimal cost, BudgetContext budgetContext)
        {
            remainingCost -= cost;
            costAllocators.Add(() => budgetContext.AllocateCost(cost));
        }

        tryToAllocateCost(
            applicableBudgets,
            ref remainingCost,
            addCostAllocator,
            null,
            out _);

        if (remainingCost > 0)
        {
            return CostCoverage.None;
        }

        foreach (var costAllocator in costAllocators)
        {
            costAllocator();
        }

        return CostCoverage.Full;

        void tryToAllocateCost(
            IEnumerable<BudgetInfo> budgets,
            ref decimal cost,
            Action<decimal, BudgetContext> costAllocationAction,
            IReadOnlyDictionary<BudgetContext, decimal> costCoverageFractionPerBudget,
            out bool costCoverageFractionsWereSatisfied)
        {
            // Assumed (or irrelevant, if no fractions are provided)
            costCoverageFractionsWereSatisfied = true;

            // "cost" is a variable that is being *indirectly* updated by "costAllocationAction".

            var originalCost = cost;

            foreach (var (budgetContext, budgetUsageDetail) in budgets)
            {
                if (cost <= 0)
                {
                    break;
                }

                var availableAmount = getAvailableAmount(budgetContext);

                if (costCoverageFractionPerBudget is null)
                {
                    if (SpendingLimit == SpendingLimit.NoLimit || cost <= availableAmount)
                    {
                        coverCost(cost);
                        break;
                    }

                    if (Simulation.AnalysisMethod.AllowFundingFromMultipleBudgets && availableAmount > 0)
                    {
                        coverCost(availableAmount);
                        continue;
                    }
                }
                else if (costCoverageFractionPerBudget.TryGetValue(budgetContext, out var coverageFraction))
                {
                    var fractionedCost = originalCost * coverageFraction;

                    if (fractionedCost <= availableAmount)
                    {
                        coverCost(fractionedCost);
                        continue;
                    }
                    else
                    {
                        // Any unsatisfied coverage fraction means the whole cost allocation has failed.
                        costCoverageFractionsWereSatisfied = false;
                        return;
                    }
                }

                budgetUsageDetail.Status = BudgetUsageStatus.CostNotCovered;

                void coverCost(decimal costToCover)
                {
                    budgetUsageDetail.Status = BudgetUsageStatus.CostCovered;
                    budgetUsageDetail.CoveredCost = costToCover;
                    costAllocationAction(costToCover, budgetContext);
                }
            }

            // Some floating-point error may accumulate that could unintentionally trigger this
            // error condition, but rounding (to the nearest tenth of a cent) should take care of
            // it. If rounding doesn't take care of it, then there's probably more to it than just
            // floating-point error.
            if (Math.Round(cost, 3) < 0)
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

    private sealed record BudgetInfo(BudgetContext Context, BudgetUsageDetail UsageDetail)
    {
        public static BudgetInfo Create(BudgetContext context, BudgetUsageDetail usageDetail) => new(context, usageDetail);
    }
}
