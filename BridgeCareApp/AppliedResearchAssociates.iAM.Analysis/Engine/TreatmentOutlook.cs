using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AppliedResearchAssociates.iAM.Analysis.Engine;

internal sealed class TreatmentOutlook
{
    private const bool InternalSummaryShouldBeDumped = false;

    private static readonly ConcurrentDictionary<SimulationRunner, string> DumpFolderNamePerSimulationRun;

    private readonly InternalSummary Summary;

    static TreatmentOutlook()
    {
        if (InternalSummaryShouldBeDumped)
        {
            DumpFolderNamePerSimulationRun = new();
        }
    }

    public TreatmentOutlook(SimulationRunner simulationRunner, AssetContext templateContext, Treatment initialTreatment, int initialYear, IEnumerable<RemainingLifeCalculator.Factory> remainingLifeCalculatorFactories)
    {
        SimulationRunner = simulationRunner ?? throw new ArgumentNullException(nameof(simulationRunner));
        TemplateContext = templateContext ?? throw new ArgumentNullException(nameof(templateContext));
        InitialTreatment = initialTreatment ?? throw new ArgumentNullException(nameof(initialTreatment));
        InitialYear = initialYear;

        if (remainingLifeCalculatorFactories is null)
        {
            throw new ArgumentNullException(nameof(remainingLifeCalculatorFactories));
        }

        AccumulationContext = new AssetContext(TemplateContext);

        RemainingLifeCalculators = remainingLifeCalculatorFactories.Select(factory => factory.Create(AccumulationContext)).ToArray();

        if (InternalSummaryShouldBeDumped)
        {
            var assetName = TemplateContext.Asset.AssetName;
            if (string.IsNullOrWhiteSpace(assetName))
            {
                assetName = TemplateContext.Asset.Id.ToString();
            }

            Summary =
                new(
                SimulationRunner.Simulation.Name,
                SimulationRunner.Simulation.AnalysisMethod.Benefit.Attribute.Name,
                SimulationRunner.Simulation.AnalysisMethod.Benefit.Attribute.IsDecreasingWithDeterioration,
                SimulationRunner.Simulation.AnalysisMethod.Benefit.Limit,
                SimulationRunner.Simulation.AnalysisMethod.Weighting?.Name ?? "n/a",
                InitialYear,
                assetName,
                InitialTreatment.Name);
        }

        Run();

        if (InternalSummaryShouldBeDumped)
        {
            Summary.CumulativeBenefit = CumulativeBenefit;

            var folderName = DumpFolderNamePerSimulationRun.GetOrAdd(
                SimulationRunner,
                $"simulation data dump {DateTime.Now:yyyy-MM-dd HH-mm-ss-fff}");

            var folderPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                "Downloads",
                folderName);

            Summary.WriteToTsv(folderPath);
        }
    }

    public TreatmentOption GetOptionRelativeToBaseline(TreatmentOutlook baseline)
    {
        if (TemplateContext != baseline.TemplateContext)
        {
            throw new ArgumentException("Template context does not match.", nameof(baseline));
        }

        return new TreatmentOption(
            TemplateContext,
            InitialTreatment,
            CumulativeCost - baseline.CumulativeCost,
            CumulativeBenefit - baseline.CumulativeBenefit,
            RemainingLife - baseline.RemainingLife,
            ConditionChange);
    }

    private readonly SimulationRunner SimulationRunner;
    private readonly AssetContext TemplateContext;
    private readonly AssetContext AccumulationContext;
    private readonly Treatment InitialTreatment;
    private readonly int InitialYear;
    private readonly RemainingLifeCalculator[] RemainingLifeCalculators;

    private double ConditionChange;
    private double CumulativeBenefit;
    private double CumulativeCost;
    private double MostRecentBenefit;
    private double? RemainingLife;

    private void AccumulateBenefit(int year)
    {
        // The "cumulative benefit" is the "area under the curve" (a key phrase from the
        // legacy system). To accumulate, we want to add the trapezoidal area between the
        // previous data point and the current data point.

        // Start with the rectangle area of (benefit_0) * 1 year.
        var additionalBenefit = MostRecentBenefit;

        var (rawBenefit, _, weight, benefit) = AccumulationContext.GetBenefitData();

        Summary?.Details.Add(new(year, rawBenefit, weight));
        MostRecentBenefit = benefit; // Update benefit_1.

        // Add the right triangle area of ((b_1 - b_0) * 1 year) / 2.
        additionalBenefit += (MostRecentBenefit - additionalBenefit) / 2;

        CumulativeBenefit += additionalBenefit;
    }

    private void ApplyTreatment(Treatment treatment, int year)
    {
        var cost = AccumulationContext.GetCostOfTreatment(treatment);
        CumulativeCost += cost * SimulationRunner.GetInflationFactor(year);

        AccumulationContext.ApplyTreatment(treatment, year);
    }

    private void Run()
    {
        Action updateRemainingLife = null;

        if (RemainingLifeCalculators.Length > 0)
        {
            RemainingLife = 0;

            updateRemainingLife = delegate
            {
                foreach (var calculator in RemainingLifeCalculators)
                {
                    calculator.UpdateValue();
                }

                var minimumFractionalRemainingLife = Enumerable.Min(
                    from calculator in RemainingLifeCalculators
                    where calculator.CurrentValueIsBeyondLimit
                    select calculator.GetLimitLocationRelativeToLatestValues().AsNullable());

                if (minimumFractionalRemainingLife.HasValue)
                {
                    RemainingLife += minimumFractionalRemainingLife.Value;
                    RemainingLife = Math.Max(RemainingLife.Value, 0);
                    updateRemainingLife = null;
                }
                else
                {
                    RemainingLife += 1;
                }
            };
        }

        var beforeTreatment = AccumulationContext.GetBenefitData();
        ApplyTreatment(InitialTreatment, InitialYear);
        var afterTreatment = AccumulationContext.GetBenefitData();

        ConditionChange = afterTreatment.lruBenefit - beforeTreatment.lruBenefit;

        Summary?.Details.Add(new(InitialYear, afterTreatment.rawBenefit, afterTreatment.weight));
        MostRecentBenefit = afterTreatment.benefit;

        updateRemainingLife?.Invoke();

        foreach (var year in Enumerable.Range(InitialYear + 1, AccumulationContext.SimulationRunner.Simulation.NumberOfYearsOfTreatmentOutlook))
        {
            var yearIsScheduled = AccumulationContext.EventSchedule.TryGetValue(year, out var scheduledEvent);

            if (yearIsScheduled && scheduledEvent.IsT2())
            {
                throw new InvalidOperationException(MessageStrings.TreatmentOutlookIsConsumingAProgressEvent);
            }

            AccumulationContext.PrepareForTreatment();

            if (yearIsScheduled && scheduledEvent.IsT1(out var treatment))
            {
                ApplyTreatment(treatment, year);
            }
            else if (!SimulationRunner.Simulation.ShouldPreapplyPassiveTreatment)
            {
                AccumulationContext.ApplyPassiveTreatment(year);
            }

            AccumulationContext.ApplyTreatmentMetadataIfPending(year);
            AccumulationContext.UnfixCalculatedFieldValues();

            AccumulateBenefit(year);

            updateRemainingLife?.Invoke();
        }
    }

    public sealed record InternalSummary(
        string ScenarioName,
        string BenefitAttributeName,
        bool BenefitDecreasesWithDeterioration,
        double BenefitLimit,
        string WeightAttributeName,
        int TreatmentYear,
        string AssetName,
        string TreatmentName)
    {
        public double CumulativeBenefit { get; set; }

        public List<Detail> Details { get; } = new();

        public void WriteToTsv(string folderPath)
        {
            var fileBaseName = $"{ScenarioName}+{TreatmentYear}+{AssetName}+{TreatmentName}";
            var fileName = new StringBuilder(fileBaseName);
            for (var i = 0; i < fileName.Length; ++i)
            {
                if (Array.IndexOf(InvalidFileNameChars, fileName[i]) >= 0)
                {
                    fileName[i] = '_';
                }
            }

            var filePath = Path.Combine(folderPath, fileName.Append(".tsv").ToString());
            var tsv = ToTsv();

            _ = Directory.CreateDirectory(folderPath);
            File.WriteAllText(filePath, tsv);
        }

        public string ToTsv()
        {
            var tsv = new StringBuilder()
                .AppendLine($"Scenario\t{ScenarioName}")
                .AppendLine()
                .AppendLine($"Benefit attribute\t{BenefitAttributeName}")
                .AppendLine($"Benefit decreases with deterioration\t{BenefitDecreasesWithDeterioration}")
                .AppendLine($"Benefit limit\t{BenefitLimit}")
                .AppendLine($"Weight attribute\t{WeightAttributeName}")
                .AppendLine()
                .AppendLine($"Year\t{TreatmentYear}")
                .AppendLine($"Asset\t{AssetName}")
                .AppendLine()
                .AppendLine($"Treatment\t{TreatmentName}")
                .AppendLine($"Cumulative benefit\t{CumulativeBenefit}")
                .AppendLine()
                .AppendLine($"Outlook year\tRaw benefit\tWeight")
                ;

            foreach (var detail in Details)
            {
                _ = tsv.AppendLine(detail.ToTsvRow());
            }

            return tsv.ToString();
        }

        public sealed record Detail(int OutlookYear, double RawBenefit, double Weight)
        {
            public string ToTsvRow() => $"{OutlookYear}\t{RawBenefit}\t{Weight}";
        }

        private static readonly char[] InvalidFileNameChars = Path.GetInvalidFileNameChars();
    }
}
