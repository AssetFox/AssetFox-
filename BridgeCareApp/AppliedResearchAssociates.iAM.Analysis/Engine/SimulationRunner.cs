//#define dump_analysis_input
//#define dump_analysis_output

#if !DEBUG
#define use_parallelism
#endif

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
using AppliedResearchAssociates.iAM.Analysis.Logic;
using AppliedResearchAssociates.iAM.DTOs.Enums;
using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAM.Analysis.Engine;

public sealed class SimulationRunner
{
    public SimulationRunner(Simulation simulation) => Simulation = simulation ?? throw new ArgumentNullException(nameof(simulation));

    public event EventHandler<ProgressEventArgs> Progress;

    public event EventHandler<SimulationLogEventArgs> SimulationLog;

#if use_parallelism
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
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            "Downloads",
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

        CalculatedFieldsWithoutPreDeteriorationTiming = Simulation.Network.Explorer.CalculatedFields.Where(cf => cf.Timing != CalculatedFieldTiming.PreDeterioration).ToList();
        CalculatedFieldsWithPostDeteriorationTiming = Simulation.Network.Explorer.CalculatedFields.Where(cf => cf.Timing == CalculatedFieldTiming.PostDeterioration).ToList();
        CalculatedFieldsWithPreDeteriorationTiming = Simulation.Network.Explorer.CalculatedFields.Where(cf => cf.Timing == CalculatedFieldTiming.PreDeterioration).ToList();

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

                priority.Handle(applicablePriorities.Add, static () => { });
            }

            applicablePriorities.Sort(BudgetPriorityComparer);
            return applicablePriorities;
        });

        CommittedProjectsPerAsset = Simulation.CommittedProjects.ToLookupAsDictionary(committedProject => committedProject.Asset);
        ConfigureCashFlowCommittedProjects();

        ConditionsPerBudget = Simulation.InvestmentPlan.BudgetConditions.ToLookupAsDictionary(budgetCondition => budgetCondition.Budget);
        CurvesPerAttribute = Simulation.PerformanceCurves.ToLookupAsDictionary(curve => curve.Attribute);
        NumberAttributeByName = Simulation.Network.Explorer.NumberAttributes.ToDictionary(attribute => attribute.Name, StringComparer.OrdinalIgnoreCase);

        SortedDistributionRulesPerCashFlowRule = Simulation.InvestmentPlan.CashFlowRules.ToDictionary(
            _ => _,
            rule => rule.DistributionRules.ToSortedDictionary(distributionRule => distributionRule.CostCeiling ?? decimal.MaxValue));

        foreach (var treatment in Simulation.Treatments)
        {
            treatment.SetConsequencesPerAttribute();
        }

        AssetContexts = Simulation.Network.Assets
#if use_parallelism
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
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            "Downloads",
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

    internal Dictionary<AnalysisMaintainableAsset, CommittedProject[]> CommittedProjectsPerAsset { get; private set; }

    internal Dictionary<NumberAttribute, PerformanceCurve[]> CurvesPerAttribute { get; private set; }

    internal Dictionary<string, NumberAttribute> NumberAttributeByName { get; private set; }

    internal Func<TreatmentOption, double> ObjectiveFunction { get; private set; }

    internal double GetInflationFactor(int year) => Simulation.InvestmentPlan.GetInflationFactor(year);

    internal void ReportProgress(ProgressStatus progressStatus, double percentComplete = 0, int? year = null)
        => OnProgress(new ProgressEventArgs(progressStatus, percentComplete, year));

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

    private List<SelectableTreatment> ActiveTreatments;

    private BudgetContext[] BudgetContexts;

    private Dictionary<int, List<BudgetPriority>> BudgetPrioritiesPerYear;

    private Func<bool> ConditionGoalsEvaluator;

    private Dictionary<Budget, BudgetCondition[]> ConditionsPerBudget;

    private List<ConditionActual> DeficientConditionActuals = new();

    internal SimulationMessageBuilder MessageBuilder;

    private SortedSet<AssetContext> AssetContexts;

    private Dictionary<CashFlowRule, SortedDictionary<decimal, CashFlowDistributionRule>> SortedDistributionRulesPerCashFlowRule;

    private SpendingLimit SpendingLimit;

    private int StatusCode;

    private List<ConditionActual> TargetConditionActuals = new();

    internal List<CalculatedField> CalculatedFieldsWithoutPreDeteriorationTiming;

    internal List<CalculatedField> CalculatedFieldsWithPreDeteriorationTiming;

    internal List<CalculatedField> CalculatedFieldsWithPostDeteriorationTiming;

    private enum CostCoverage
    {
        None,
        Full,
        CashFlow,
    }

    private static bool GoalsAreMet(IEnumerable<ConditionActual> conditionActuals) => conditionActuals.All(actual => actual.GoalIsMet);

    private static void InParallel<T>(IEnumerable<T> items, Action<T> action)
    {
#if use_parallelism
        _ = System.Threading.Tasks.Parallel.ForEach(items, new System.Threading.Tasks.ParallelOptions { MaxDegreeOfParallelism = MaxThreadsForSimulation }, action);
#else
        foreach (var item in items)
        {
            action(item);
        }
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

                        if (!committedProject.ShouldApplyConsequences)
                        {
                            context.Detail.TreatmentStatus = TreatmentStatus.Progressed;
                        }
                    }

                    context.Detail.TreatmentCause = treatment is CommittedProject or CommittedProjectBundle
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

    private void ConfigureCashFlowCommittedProjects()
    {
        // This feature currently requires that there be exactly one cash-flow rule whose criterion
        // is blank, i.e. exactly one "default" cash-flow rule. In addition, that rule must have
        // exactly one 1-year distribution, and that distribution must have a definite cost ceiling.

        var defaultCashFlowRules =
            Simulation.InvestmentPlan.CashFlowRules.Where(cf => cf.Criterion.ExpressionIsBlank).ToList();

        if (defaultCashFlowRules.Count != 1)
        {
            disabledCfcpWarning();
            return;
        }

        var oneYearDistributions =
            defaultCashFlowRules[0].DistributionRules.Where(d => d.YearlyPercentages.Count == 1).ToList();

        if (oneYearDistributions.Count != 1)
        {
            disabledCfcpWarning();
            return;
        }

        var costCeiling = oneYearDistributions[0].CostCeiling;

        if (costCeiling is null)
        {
            disabledCfcpWarning();
            return;
        }

        var cashFlowCostThreshold = (double)costCeiling.Value;

        // Cash-flow committed projects (CFCPs) are automatically identified by the system. The
        // pattern is a sequence of CPs in consecutive years on the same asset using the same
        // treatment where the total cost of the sequence is more than a given threshold (determined
        // by the cost ceiling value on the 1-year distribution on the default cash-flow rule). The
        // effect of identification is that each non-final CP of a cash-flow CP sequence will have
        // its consequences disabled.

        var cpGroups = Simulation.CommittedProjects
            .GroupBy(cp => (cp.Asset, cp.TemplateTreatment))
            .Select(g => g.OrderBy(cp => cp.Year).ToList())
            .ToList();

        foreach (var g in cpGroups)
        {
            var cpSequence = new List<CommittedProject>();
            var expectedYear = g[0].Year;

            foreach (var cp in g)
            {
                if (cp.Year == expectedYear)
                {
                    cpSequence.Add(cp);
                    ++expectedYear;
                }
                else
                {
                    // Process current sequence.
                    process(cpSequence, cashFlowCostThreshold);

                    // Gather new sequence.
                    cpSequence.Clear();
                    cpSequence.Add(cp);
                    expectedYear = cp.Year + 1;
                }
            }

            // Process last sequence.
            process(cpSequence, cashFlowCostThreshold);
        }

        static void process(List<CommittedProject> cpSequence, double cashFlowCostThreshold)
        {
            var totalCost = cpSequence.Sum(cp => cp.Cost);
            if (totalCost > cashFlowCostThreshold)
            {
                foreach (var cfcp in cpSequence.SkipLast(1))
                {
                    cfcp.ShouldApplyConsequences = false;
                }
            }
        }

        void disabledCfcpWarning()
        {
            MessageBuilder = new SimulationMessageBuilder($"Cash-flow committed projects identification is disabled; input requirements not met.")
            {
                ItemId = Simulation.InvestmentPlan.Id,
                ItemName = nameof(Simulation.InvestmentPlan),
            };

            var warning = SimulationLogMessageBuilders.RuntimeWarning(MessageBuilder, Simulation.Id);
            Send(warning);
        }
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
                            context => context.CurrentPriorityAmount ?? context.CurrentAmount);

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
                                _ = workingContext.EventSchedule.TryAdd(year, option.CandidateTreatment);
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

        InParallel(contexts, addTreatmentOptionsToBag);

        var treatmentOptions = treatmentOptionsBag.OrderByDescending(option => option.WeightedObjectiveValue).ToArray();

        treatmentOptionsBag.Clear();
        treatmentOptionsBag = null;

        return treatmentOptions;

        // --- local functions ---

        void addTreatmentOptionsToBag(AssetContext context)
        {
            context.Detail.SpatialWeightForOrderingOptions = context.GetSpatialWeight();

            if (context.YearIsWithinShadowForAnyTreatment(year))
            {
                foreach (var treatment in ActiveTreatments)
                {
                    addRejection(context, treatment, TreatmentRejectionReason.WithinShadowForAnyTreatment);
                }

                return;
            }

            var feasibleTreatments = getFeasibleTreatments(context, year);

            _ = feasibleTreatments.RemoveWhere(treatment =>
            {
                var cost = context.GetCostOfTreatment(treatment);
                if (TryConvertToDecimal(cost, out var convertedCost) && convertedCost > 0)
                {
                    if (convertedCost < Simulation.InvestmentPlan.MinimumProjectCostLimit)
                    {
                        addRejection(context, treatment, TreatmentRejectionReason.CostIsBelowMinimumProjectCostLimit);
                        return true;
                    }

                    return false;
                }

                addRejection(context, treatment, TreatmentRejectionReason.InvalidCost);

                var messageBuilder = SimulationLogMessageBuilders.InvalidTreatmentCost(
                    context.Asset,
                    treatment,
                    cost,
                    context.SimulationRunner.Simulation.Id);

                Send(messageBuilder);
                return true;
            });

            if (feasibleTreatments.Count > 0)
            {
                var remainingLifeCalculatorFactories = Enumerable.ToArray(
                    from limit in Simulation.AnalysisMethod.RemainingLifeLimits
                    where limit.Criterion.EvaluateOrDefault(context)
                    group limit.Value by limit.Attribute into attributeLimitValues
                    select new RemainingLifeCalculator.Factory(attributeLimitValues));

                var baselineOutlook = new TreatmentOutlook(
                    this,
                    context,
                    Simulation.DesignatedPassiveTreatment,
                    year,
                    remainingLifeCalculatorFactories);

                List<TreatmentOption> optionsToBundle;
                Action<TreatmentOption> addOption;
                if (Simulation.ShouldBundleFeasibleTreatments)
                {
                    optionsToBundle = new();
                    addOption = optionsToBundle.Add;
                }
                else
                {
                    optionsToBundle = null;
                    addOption = treatmentOptionsBag.Add;
                }

                foreach (var treatment in feasibleTreatments)
                {
                    evaluateTreatmentOutlook(treatment, addOption);
                }

                if (Simulation.ShouldBundleFeasibleTreatments)
                {
                    if (optionsToBundle.Count == 1)
                    {
                        treatmentOptionsBag.Add(optionsToBundle[0]);
                    }
                    else if (optionsToBundle.Count > 1)
                    {
                        var bundle = new TreatmentBundle(optionsToBundle.Select(option => option.CandidateTreatment));
                        evaluateTreatmentOutlook(bundle, treatmentOptionsBag.Add);
                    }
                }

                void evaluateTreatmentOutlook(Treatment treatment, Action<TreatmentOption> acceptOption)
                {
                    var outlook = new TreatmentOutlook(this, context, treatment, year, remainingLifeCalculatorFactories);
                    var option = outlook.GetOptionRelativeToBaseline(baselineOutlook);

                    if (option.WeightedObjectiveValue > 0)
                    {
                        context.Detail.TreatmentOptions.Add(option.Detail);
                        acceptOption(option);
                    }
                    else
                    {
                        addRejection(context, treatment, TreatmentRejectionReason.NonPositiveObjectiveValue);
                    }
                }
            }
        }

        HashSet<Treatment> getFeasibleTreatments(AssetContext context, int year)
        {
            var treatments = ActiveTreatments.ToHashSet();

            _ = treatments.RemoveWhere(treatment =>
            {
                var isRejected = context.YearIsWithinShadowForSameTreatment(year, treatment);
                if (isRejected)
                {
                    addRejection(context, treatment, TreatmentRejectionReason.WithinShadowForSameTreatment);
                }

                return isRejected;
            });

            _ = treatments.RemoveWhere(treatment =>
            {
                var isFeasible = treatment.IsFeasible(context);
                if (!isFeasible)
                {
                    addRejection(context, treatment, TreatmentRejectionReason.NotFeasible);
                }

                return !isFeasible;
            });

            var supersededTreatmentsQuery =
                from treatment in treatments
                from supersedeRule in treatment.SupersedeRules
                where supersedeRule.Criterion.EvaluateOrDefault(context)
                select supersedeRule.Treatment;

            var supersededTreatments = supersededTreatmentsQuery.ToHashSet();

            _ = treatments.RemoveWhere(treatment =>
            {
                var isSuperseded = supersededTreatments.Contains(treatment);
                if (isSuperseded)
                {
                    addRejection(context, treatment, TreatmentRejectionReason.Superseded);
                }

                return isSuperseded;
            });

            return treatments.ToHashSet<Treatment>();
        }

        static void addRejection(AssetContext context, Treatment treatment, TreatmentRejectionReason rejectionReason)
        {
            var conditionChange = getConditionChange(context, treatment);
            context.Detail.TreatmentRejections.Add(new(treatment.Name, rejectionReason, conditionChange));
        }

        static double getConditionChange(AssetContext context, Treatment treatment)
        {
            var copyOfContext = new AssetContext(context);
            var beforeTreatment = copyOfContext.GetBenefitData();
            copyOfContext.ApplyTreatmentConsequences(treatment);
            var afterTreatment = copyOfContext.GetBenefitData();
            return afterTreatment.lruBenefit - beforeTreatment.lruBenefit;
        }
    }

    private List<ConditionActual> GetDeficientConditionActuals()
    {
        var results = new List<ConditionActual>();

        foreach (var goal in Simulation.AnalysisMethod.DeficientConditionGoals)
        {
            var goalContexts = AssetContexts
#if use_parallelism
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

    private List<ConditionActual> GetTargetConditionActuals(int year)
    {
        var results = new List<ConditionActual>();

        foreach (var goal in Simulation.AnalysisMethod.TargetConditionGoals)
        {
            if (goal.Year.HasValue && goal.Year.Value != year)
            {
                continue;
            }

            var goalContexts = AssetContexts
#if use_parallelism
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

    private void UpdateConditionActuals(int year)
    {
        TargetConditionActuals = GetTargetConditionActuals(year);
        DeficientConditionActuals = GetDeficientConditionActuals();
    }

    #region Treatment funding logic

    private CostCoverage TryToPayForTreatment(
        AssetContext assetContext,
        Treatment treatment,
        int year,
        Func<BudgetContext, decimal> getAvailableAmount)
    {
        var treatmentConsideration = assetContext.Detail.TreatmentConsiderations.GetAdd(new(treatment.Name));

        // First, check for committed projects.

        if (treatment is CommittedProject cp)
        {
            treatmentConsideration.FundingCalculationOutput = new()
            {
                AllocationMatrix =
                {
                    // CP cost is assumed to already include all appropriate adjustments for inflation.
                    new(year, cp.Budget.Name, cp.Name, (decimal)cp.Cost)
                }
            };

            return CostCoverage.Full;
        }

        if (treatment is CommittedProjectBundle cpBundle)
        {
            treatmentConsideration.FundingCalculationOutput = new();

            foreach (var bp in cpBundle.BundledProjects)
            {
                treatmentConsideration.FundingCalculationOutput.AllocationMatrix.Add(
                    new(year, bp.Budget.Name, bp.Name, (decimal)bp.Cost));
            }

            return CostCoverage.Full;
        }

        // At this point, we know we are not dealing with committed projects.

        var totalBasicCost = (decimal)assetContext.GetCostOfTreatment(treatment);
        var inflationFactor = (decimal)GetInflationFactor(year);
        var totalTreatmentCost = totalBasicCost * inflationFactor;

        // Begin filling in the funding calculation input.

        var fundingSettings = new Funding.Settings
        {
            BudgetCarryoverIsAllowed = Simulation.InvestmentPlan.AllowFundingCarryover,
            MultipleBudgetsCanFundEachTreatment = Simulation.AnalysisMethod.AllowFundingFromMultipleBudgets,
            UnlimitedSpending = SpendingLimit == SpendingLimit.NoLimit,
        };

        var treatmentsToFund = new List<Treatment>();
        if (treatment is TreatmentBundle bundle)
        {
            treatmentsToFund.AddRange(bundle.BundledTreatments);
        }
        else
        {
            treatmentsToFund.Add(treatment);
        }

        treatmentConsideration.FundingCalculationInput = new();

        foreach (var treatmentToFund in treatmentsToFund)
        {
            var cost = (decimal)assetContext.GetCostOfTreatment(treatmentToFund) * inflationFactor;

            treatmentConsideration.FundingCalculationInput.TreatmentsToFund.Add(
                new(treatmentToFund.Name, cost));
        }

        var costPerTreatment = treatmentConsideration.FundingCalculationInput.TreatmentsToFund
            .Select(t => t.Cost)
            .ToArray();

        var amountPerBudgetOfCurrentYear = new decimal[BudgetContexts.Length];

        var allocationIsAllowedPerBudgetAndTreatment = new bool[BudgetContexts.Length, treatmentsToFund.Count];

        for (var b = 0; b < BudgetContexts.Length; ++b)
        {
            var budgetContext = BudgetContexts[b];

            var amount = getAvailableAmount(budgetContext);

            amountPerBudgetOfCurrentYear[b] = amount;

            treatmentConsideration.FundingCalculationInput.CurrentBudgetsToSpend.Add(
                new(budgetContext.Budget.Name, amount, year));

            var budgetConditionIsMet =
                !ConditionsPerBudget.TryGetValue(budgetContext.Budget, out var budgetConditions) ||
                budgetConditions.Any(condition => condition.Criterion.EvaluateOrDefault(assetContext));

            for (var t = 0; t < treatmentsToFund.Count; ++t)
            {
                var treatmentToFund = treatmentsToFund[t];

                if (!treatmentToFund.CanUseBudget(budgetContext.Budget))
                {
                    treatmentConsideration.FundingCalculationInput.ExclusionsMatrix.Add(new(
                        budgetContext.Budget.Name,
                        treatmentToFund.Name,
                        FundingCalculationInput.ExclusionReason.TreatmentSettings));
                }
                else if (!budgetConditionIsMet)
                {
                    treatmentConsideration.FundingCalculationInput.ExclusionsMatrix.Add(new(
                        budgetContext.Budget.Name,
                        treatmentToFund.Name,
                        FundingCalculationInput.ExclusionReason.BudgetConditions));
                }
                else
                {
                    allocationIsAllowedPerBudgetAndTreatment[b, t] = true;
                }
            }
        }

        // First, attempt any applicable cash flows.

        Action scheduleCashFlowEvents = null;
        var oneYearCashFlowIsBeingUsed = false;

        foreach (var cashFlowRule in Simulation.InvestmentPlan.CashFlowRules)
        {
            var cashFlowConsideration = treatmentConsideration.CashFlowConsiderations.GetAdd(new(cashFlowRule.Name));

            if (cashFlowRule.Criterion.EvaluateOrDefault(assetContext))
            {
                cashFlowConsideration.ReasonAgainstCashFlow = scheduleCashFlowEvents is null
                    ? handleCashFlowRule(cashFlowRule, cashFlowConsideration)
                    : ReasonAgainstCashFlow.NotNeeded;
            }
            else
            {
                cashFlowConsideration.ReasonAgainstCashFlow = ReasonAgainstCashFlow.ConditionNotMet;
            }
        }

        if (scheduleCashFlowEvents is not null)
        {
            scheduleCashFlowEvents();
            return oneYearCashFlowIsBeingUsed ? CostCoverage.Full : CostCoverage.CashFlow;
        }

        // At this point, no cash flow could be used. So try to pay the normal way.

        var fundable = Funding.TrySolve(
            allocationIsAllowedPerBudgetAndTreatment,
            amountPerBudgetOfCurrentYear,
            costPerTreatment,
            fundingSettings,
            out var allocationPerBudgetAndTreatment);

        if (!fundable)
        {
            return CostCoverage.None;
        }

        treatmentConsideration.FundingCalculationOutput = new();
        var costAllocators = new List<Action>();

        PrepareFundingCalculationOutput(
            treatmentsToFund,
            allocationPerBudgetAndTreatment,
            year,
            treatmentConsideration.FundingCalculationOutput,
            costAllocators);

        foreach (var costAllocator in costAllocators)
        {
            costAllocator();
        }

        return CostCoverage.Full;

        // --- local functions ---

        ReasonAgainstCashFlow handleCashFlowRule(CashFlowRule cashFlowRule, CashFlowConsiderationDetail cashFlowConsideration)
        {
            var distributionRule =
                SortedDistributionRulesPerCashFlowRule[cashFlowRule]
                .FirstOrDefault(kv => totalTreatmentCost <= kv.Key)
                .Value;

            // Do some basic validation.

            if (distributionRule is null)
            {
                return ReasonAgainstCashFlow.NoApplicableDistributionRule;
            }

            var lastYearOfCashFlow = year + distributionRule.YearlyPercentages.Count - 1;
            var futureYearsOfCashFlow = Static.RangeFromBounds(year + 1, lastYearOfCashFlow);
            var scheduleIsBlocked = futureYearsOfCashFlow.Any(assetContext.EventSchedule.ContainsKey);
            if (scheduleIsBlocked)
            {
                return ReasonAgainstCashFlow.FutureEventScheduleIsBlocked;
            }

            // Attempt a funding solution.

            cashFlowConsideration.FundingCalculationInputSupplement = new();

            var lastYearOfCashFlowInPeriod = Math.Min(lastYearOfCashFlow, Simulation.InvestmentPlan.LastYearOfAnalysisPeriod);
            var numberOfCashFlowYears = lastYearOfCashFlowInPeriod - year + 1;
            var costPercentagePerYear = new decimal[numberOfCashFlowYears];
            for (var y = 0; y < costPercentagePerYear.Length; ++y)
            {
                var costPercentage = distributionRule.YearlyPercentages[y];
                costPercentagePerYear[y] = costPercentage;

                cashFlowConsideration.FundingCalculationInputSupplement.CashFlowDistribution
                    .Add(new(year + y, costPercentage));
            }

            var amountPerBudgetPerYear = new decimal[costPercentagePerYear.Length][];
            amountPerBudgetPerYear[0] = amountPerBudgetOfCurrentYear;

            for (var y = 1; y < costPercentagePerYear.Length; ++y)
            {
                var amountPerBudget = new decimal[BudgetContexts.Length];
                var futureYear = year + y;

                for (var b = 0; b < BudgetContexts.Length; ++b)
                {
                    var budgetContext = BudgetContexts[b];
                    var amount = budgetContext.GetAmount(futureYear);
                    amountPerBudget[b] = amount;

                    cashFlowConsideration.FundingCalculationInputSupplement.FutureBudgetsToSpend
                        .Add(new(budgetContext.Budget.Name, amount, futureYear));
                }

                amountPerBudgetPerYear[y] = amountPerBudget;
            }

            var fundable = Funding.TrySolve(
                allocationIsAllowedPerBudgetAndTreatment,
                amountPerBudgetPerYear,
                costPerTreatment,
                costPercentagePerYear,
                fundingSettings,
                out var allocationPerBudgetAndTreatmentPerYear);

            if (!fundable)
            {
                return ReasonAgainstCashFlow.FundingIsNotAvailable;
            }

            // Successfully funded. Prepare the output.

            treatmentConsideration.FundingCalculationOutput = new();
            var futureCostAllocators = new List<Action>();

            for (var y = 0; y < allocationPerBudgetAndTreatmentPerYear.Length; ++y)
            {
                PrepareFundingCalculationOutput(
                    treatmentsToFund,
                    allocationPerBudgetAndTreatmentPerYear[y],
                    year + y,
                    treatmentConsideration.FundingCalculationOutput,
                    futureCostAllocators);
            }

            var progression = costPercentagePerYear.Select(_ => new TreatmentProgress(treatment)).ToArray();
            progression[^1].IsComplete = true;

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

            if (distributionRule.YearlyPercentages.Count == 1)
            {
                oneYearCashFlowIsBeingUsed = true;
            }

            return ReasonAgainstCashFlow.None;
        }
    }

    private void PrepareFundingCalculationOutput(
        IReadOnlyList<Treatment> treatments,
        decimal?[,] allocationPerBudgetAndTreatment,
        int year,
        FundingCalculationOutput output,
        List<Action> costAllocators)
    {
        for (var b = 0; b < BudgetContexts.Length; ++b)
        {
            var budgetContext = BudgetContexts[b];

            var budgetTotalSpending = 0m;
            for (var t = 0; t < treatments.Count; ++t)
            {
                if (allocationPerBudgetAndTreatment[b, t] is decimal allocation && allocation > 0)
                {
                    budgetTotalSpending += allocation;

                    output.AllocationMatrix.Add(
                        new(year, budgetContext.Budget.Name, treatments[t].Name, allocation));
                }
            }

            if (budgetTotalSpending > 0)
            {
                costAllocators.Add(() => budgetContext.AllocateCost(budgetTotalSpending, year));
            }
        }
    }

    #endregion
}
