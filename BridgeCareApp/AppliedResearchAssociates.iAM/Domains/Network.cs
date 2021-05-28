using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAM.Domains
{
    public sealed class Network : WeakEntity, IValidator
    {
        internal Network(Explorer explorer) => Explorer = explorer ?? throw new ArgumentNullException(nameof(explorer));

        public static string SpatialWeightIdentifier => "AREA";

        public Explorer Explorer { get; }

        public IReadOnlyCollection<Facility> Facilities => _Facilities;

        public string Name { get; set; }

        public IReadOnlyCollection<Simulation> Simulations => _Simulations;

        public string SpatialWeightUnit
        {
            get => _SpatialWeightUnit;
            set => _SpatialWeightUnit = value?.Trim() ?? "";
        }

        public ValidatorBag Subvalidators => new ValidatorBag { Facilities, Simulations };

        public IEnumerable<Section> Sections => Facilities.SelectMany(facility => facility.Sections);

        public Facility AddFacility() => _Facilities.GetAdd(new Facility(this));

        public Simulation AddSimulation() => _Simulations.GetAdd(new Simulation(this));

        public void ClearFacilities() => _Facilities.Clear();

        public ValidationResultBag GetDirectValidationResults()
        {
            var results = new ValidationResultBag();

            if (string.IsNullOrWhiteSpace(Name))
            {
                results.Add(ValidationStatus.Error, "Name is blank.", this, nameof(Name));
            }

            if (Facilities.Count == 0)
            {
                results.Add(ValidationStatus.Error, "There are no facilities.", this, nameof(Facilities));
            }
            else if (Facilities.Select(facility => facility.Name).Distinct().Count() < Facilities.Count)
            {
                results.Add(ValidationStatus.Error, "Multiple facilities have the same name.", this, nameof(Facilities));
            }

            if (Simulations.Select(simulation => simulation.Name).Distinct().Count() < Simulations.Count)
            {
                results.Add(ValidationStatus.Error, "Multiple simulations have the same name.", this, nameof(Simulations));
            }

            return results;
        }

        public void Remove(Simulation simulation) => _Simulations.Remove(simulation);

        private readonly List<Facility> _Facilities = new List<Facility>();

        private readonly List<Simulation> _Simulations = new List<Simulation>();

        private string _SpatialWeightUnit = "";

        public string ShortDescription => Name;
    }
}
