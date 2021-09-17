using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAM.Domains
{
    public sealed class Simulation : WeakEntity, IValidator
    {
        public AnalysisMethod AnalysisMethod { get; }

        public ICollection<CommittedProject> CommittedProjects { get; } = new SetWithoutNulls<CommittedProject>();

        public SelectableTreatment DesignatedPassiveTreatment { get; internal set; }

        public InvestmentPlan InvestmentPlan { get; }

        public string Name { get; set; }

        public DateTime LastRun { get; set; }

        public DateTime LastModifiedDate { get; set; }

        public Network Network { get; }

        public int NumberOfYearsOfTreatmentOutlook { get; set; } = 100;

        public IReadOnlyCollection<PerformanceCurve> PerformanceCurves => _PerformanceCurves;

        public SimulationOutput Results { get; private set; } = new SimulationOutput();

        public string ShortDescription => Name;

        /// <summary>
        ///     Whether to always pre-apply the passive treatment just after deterioration. This
        ///     feature exists in order to provide v1-compatible analysis behavior. If this feature
        ///     is enabled, the v2 feature based on <see cref="CalculatedField.Timing"/> is disabled.
        /// </summary>
        public bool ShouldPreapplyPassiveTreatment { get; set; }

        public ValidatorBag Subvalidators => new ValidatorBag { AnalysisMethod, CommittedProjects, InvestmentPlan, PerformanceCurves, Treatments };

        public IReadOnlyCollection<SelectableTreatment> Treatments => _Treatments;

        public PerformanceCurve AddPerformanceCurve() => _PerformanceCurves.GetAdd(new PerformanceCurve(Network.Explorer));

        public SelectableTreatment AddTreatment() => _Treatments.GetAdd(new SelectableTreatment(this));

        public void ClearResults() => Results = new SimulationOutput();

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

            if (Treatments.Select(treatment => treatment.Name).Distinct().Count() < Treatments.Count)
            {
                results.Add(ValidationStatus.Error, "Multiple selectable treatments have the same name.", this, nameof(Treatments));
            }

            if (CommittedProjects.Select(project => (project.Section, project.Year)).Distinct().Count() < CommittedProjects.Count)
            {
                results.Add(ValidationStatus.Error, "Multiple projects are committed to the same section in the same year.", this, nameof(CommittedProjects));
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

        private readonly List<PerformanceCurve> _PerformanceCurves = new List<PerformanceCurve>();

        private readonly List<SelectableTreatment> _Treatments = new List<SelectableTreatment>();
    }
}
