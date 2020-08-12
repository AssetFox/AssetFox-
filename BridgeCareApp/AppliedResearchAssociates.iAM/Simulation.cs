using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAM
{
    public sealed class Simulation : IValidator
    {
        public Simulation(Network network)
        {
            Network = network ?? throw new ArgumentNullException(nameof(network));

            AnalysisMethod = new AnalysisMethod(this);
            InvestmentPlan = new InvestmentPlan(this);
        }

        public AnalysisMethod AnalysisMethod { get; }

        public ICollection<CommittedProject> CommittedProjects { get; } = new SetWithoutNulls<CommittedProject>();

        public SelectableTreatment DesignatedPassiveTreatment { get; internal set; }

        public InvestmentPlan InvestmentPlan { get; }

        public string Name { get; set; }

        public Network Network { get; }

        public double NetworkCondition
        {
            get
            {
                var network = Results.Last().Sections;
                var networkArea = network.Sum(section => section.Area);
                var networkCondition = network.Sum(section => AnalysisMethod.Benefit.LimitValue(section.ValuePerNumericAttribute[AnalysisMethod.Benefit.Attribute.Name]) * section.Area / networkArea);
                return networkCondition;
            }
        }

        public int NumberOfYearsOfTreatmentOutlook { get; set; } = 100;

        public IReadOnlyCollection<PerformanceCurve> PerformanceCurves => _PerformanceCurves;

        public ICollection<SimulationYearDetail> Results { get; } = new SetWithoutNulls<SimulationYearDetail>();

        public ValidatorBag Subvalidators => new ValidatorBag { AnalysisMethod, CommittedProjects, InvestmentPlan, PerformanceCurves, Treatments };

        public IReadOnlyCollection<SelectableTreatment> Treatments => _Treatments;

        public PerformanceCurve AddPerformanceCurve() => _PerformanceCurves.GetAdd(new PerformanceCurve(Network.Explorer));

        public SelectableTreatment AddTreatment() => _Treatments.GetAdd(new SelectableTreatment(this));

        public IReadOnlyCollection<SelectableTreatment> GetActiveTreatments()
        {
            var result = Treatments.ToHashSet();
            _ = result.Remove(DesignatedPassiveTreatment);
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
            else if (InvestmentPlan.GetAllValidationResults().All(result => result.Status != ValidationStatus.Error))
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

        internal BudgetContext[] GetBudgetContextsWithCostAllocationsForCommittedProjects()
        {
            var budgetContexts = InvestmentPlan.Budgets
                .Select(budget => new BudgetContext(budget, InvestmentPlan.FirstYearOfAnalysisPeriod))
                .ToArray();

            var committedProjectsPerBudget = CommittedProjects.ToLookup(committedProject => committedProject.Budget);

            Func<decimal, BudgetContext, bool> costCanBeAllocated;
            switch (AnalysisMethod.SpendingLimit)
            {
            case SpendingLimit.Zero:
                costCanBeAllocated = (cost, context) => cost == 0;
                break;

            case SpendingLimit.Budget:
                costCanBeAllocated = (cost, context) => cost <= context.CurrentAmount;
                break;

            case SpendingLimit.NoLimit:
                costCanBeAllocated = (cost, context) => true;
                break;

            default:
                throw new InvalidOperationException("Invalid spending limit.");
            }

            foreach (var context in budgetContexts)
            {
                var committedProjectPerYear = committedProjectsPerBudget[context.Budget].ToSortedDictionary(committedProject => committedProject.Year);

                if (committedProjectPerYear.Any())
                {
                    foreach (var year in Static.BoundRange(committedProjectPerYear.Last().Value.Year, InvestmentPlan.FirstYearOfAnalysisPeriod, -1))
                    {
                        context.SetYear(year);

                        if (committedProjectPerYear.TryGetValue(year, out var committedProject))
                        {
                            var cost = (decimal)committedProject.Cost;

                            if (!costCanBeAllocated(cost, context))
                            {
                                throw new SimulationException($"Committed project \"{committedProject.Name}\" cannot be funded.");
                            }

                            context.AllocateCost(cost);
                        }

                        context.LimitPreviousAmountToCurrentAmount();
                    }
                }
            }

            return budgetContexts;
        }

        private readonly List<PerformanceCurve> _PerformanceCurves = new List<PerformanceCurve>();

        private readonly List<SelectableTreatment> _Treatments = new List<SelectableTreatment>();
    }
}
