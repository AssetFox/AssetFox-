using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAM.Analysis;

public sealed class Simulation : WeakEntity, IValidator
{
    public AnalysisMethod AnalysisMethod { get; }

    public ICollection<CommittedProject> CommittedProjects { get; } = new SetWithoutNulls<CommittedProject>();

    public SelectableTreatment DesignatedPassiveTreatment { get; internal set; }

    public InvestmentPlan InvestmentPlan { get; }

    public DateTime LastModifiedDate { get; set; }

    public DateTime LastRun { get; set; }

    public string Name { get; set; }

    public Network Network { get; }

    public int NumberOfYearsOfTreatmentOutlook { get; set; } = 100;

    public IReadOnlyCollection<PerformanceCurve> PerformanceCurves => _PerformanceCurves;

    public SimulationOutput Results
    {
        get
        {
            if (_Results.TryGetTarget(out var value))
            {
                return value;
            }

            value = ResultsOnDisk.GetOutput();
            _Results.SetTarget(value);
            return value;
        }
    }

    public string ShortDescription => Name;

    /// <summary>
    ///     Whether to always pre-apply the passive treatment just after deterioration. This
    ///     feature exists in order to provide v1-compatible analysis behavior.
    /// </summary>
    public bool ShouldPreapplyPassiveTreatment { get; set; } = true;

    public ValidatorBag Subvalidators => new ValidatorBag { AnalysisMethod, CommittedProjects, InvestmentPlan, PerformanceCurves, Treatments };

    public IReadOnlyCollection<SelectableTreatment> Treatments => _Treatments;

    public PerformanceCurve AddPerformanceCurve() => _PerformanceCurves.GetAdd(new PerformanceCurve(Network.Explorer));

    public SelectableTreatment AddTreatment() => _Treatments.GetAdd(new SelectableTreatment(this));

    public void ClearResults() => ResultsOnDisk.Clear();

    public IReadOnlyCollection<SelectableTreatment> GetActiveTreatments()
    {
        var result = Treatments.ToList();
        _ = result.Remove(DesignatedPassiveTreatment);
        result.Sort(TreatmentComparer);
        return result;
    }

    public ValidationResultBag GetDirectValidationResults()
    {
        var results = new ValidationResultBag();

        var treatmentsWithEmptyFeasibility = Treatments.Where(treatment => treatment.FeasibilityCriteria.Count == 0).ToList();
        if (treatmentsWithEmptyFeasibility.Count != 1)
        {
            results.Add(ValidationStatus.Warning, $"There are {treatmentsWithEmptyFeasibility.Count} treatments with empty feasibility.", this, nameof(Treatments));
        }
        else if (DesignatedPassiveTreatment != null && DesignatedPassiveTreatment != treatmentsWithEmptyFeasibility[0])
        {
            results.Add(ValidationStatus.Warning, "Designated passive treatment is not the single treatment with empty feasibility.", this, nameof(DesignatedPassiveTreatment));
        }

        if (DesignatedPassiveTreatment == null)
        {
            results.Add(ValidationStatus.Error, "Designated passive treatment is unset.", this, nameof(DesignatedPassiveTreatment));
        }
        else if (DesignatedPassiveTreatment.Costs.Count > 0)
        {
            results.Add(ValidationStatus.Error, "Designated passive treatment has costs.", this, nameof(DesignatedPassiveTreatment));
        }

        if (string.IsNullOrWhiteSpace(Name))
        {
            results.Add(ValidationStatus.Error, "Name is blank.", this, nameof(Name));
        }

        if (NumberOfYearsOfTreatmentOutlook < 1)
        {
            results.Add(ValidationStatus.Error, "Number of years of treatment outlook is less than one.", this, nameof(NumberOfYearsOfTreatmentOutlook));
        }
        else if (NumberOfYearsOfTreatmentOutlook < 10)
        {
            results.Add(ValidationStatus.Warning, "Number of years of treatment outlook is less than ten.", this, nameof(NumberOfYearsOfTreatmentOutlook));
        }

        if (Treatments.Select(treatment => treatment.Name).Distinct().Count() < Treatments.Count)
        {
            results.Add(ValidationStatus.Error, "Multiple selectable treatments have the same name.", this, nameof(Treatments));
        }

        if (CommittedProjects.Select(project => (project.Asset, project.Year)).Distinct().Count() < CommittedProjects.Count)
        {
            results.Add(ValidationStatus.Error, "Multiple projects are committed to the same asset in the same year.", this, nameof(CommittedProjects));
        }
        else if (InvestmentPlan.GetAllValidationResults(new List<string>()).All(result => result.Status != ValidationStatus.Error))
        {
            try
            {
                _ = GetBudgetContextsWithCostAllocationsForCommittedProjects();
            }
            catch (SimulationException e)
            {
                results.Add(ValidationStatus.Error, "At least one committed project cannot be funded: " + e.Message, this, nameof(CommittedProjects));
            }
        }

        return results;
    }

    public void Remove(SelectableTreatment treatment) => _Treatments.Remove(treatment);

    public void Remove(PerformanceCurve performanceCurve) => _PerformanceCurves.Remove(performanceCurve);

    internal Simulation(Network network)
    {
        Network = network ?? throw new ArgumentNullException(nameof(network));

        AnalysisMethod = new AnalysisMethod(this);
        InvestmentPlan = new InvestmentPlan(this);
    }

    internal SimulationOutputOnDisk ResultsOnDisk { get; } = new();

    internal BudgetContext[] GetBudgetContextsWithCostAllocationsForCommittedProjects()
    {
        var budgetContexts = InvestmentPlan.Budgets
            .Select(budget => new BudgetContext(budget, InvestmentPlan.FirstYearOfAnalysisPeriod))
            .ToArray();

        var committedProjectsPerBudget = CommittedProjects.ToLookup(committedProject => committedProject.Budget);

        foreach (var context in budgetContexts)
        {
            var committedProjectsPerYear = committedProjectsPerBudget[context.Budget].ToLookup(committedProject => committedProject.Year);

            if (committedProjectsPerYear.Any())
            {
                foreach (var year in InvestmentPlan.YearsOfAnalysis)
                {
                    var committedProjects = committedProjectsPerYear[year];
                    if (committedProjects.Any())
                    {
                        var cost = committedProjects.Sum(committedProject => (decimal)committedProject.Cost);
                        context.AllocateCost(cost, year);
                    }
                }
            }
        }

        return budgetContexts;
    }

    private static readonly IComparer<SelectableTreatment> TreatmentComparer = SelectionComparer<SelectableTreatment>.Create(treatment => treatment.Name);

    private readonly List<PerformanceCurve> _PerformanceCurves = new();
    private readonly WeakReference<SimulationOutput> _Results = new(null);
    private readonly List<SelectableTreatment> _Treatments = new();
}
