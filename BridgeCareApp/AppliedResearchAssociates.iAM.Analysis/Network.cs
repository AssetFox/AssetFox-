using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAM.Analysis
{
    public sealed class Network : WeakEntity, IValidator
    {
        internal Network(Explorer explorer) => Explorer = explorer ?? throw new ArgumentNullException(nameof(explorer));

        public static string SpatialWeightIdentifier => "AREA";

        public Explorer Explorer { get; }

        public IReadOnlyCollection<MaintainableAsset> Assets => _Assets;

        public string Name { get; set; }

        public IReadOnlyCollection<Simulation> Simulations => _Simulations;

        public string SpatialWeightUnit
        {
            get => _SpatialWeightUnit;
            set => _SpatialWeightUnit = value?.Trim() ?? "";
        }

        public ValidatorBag Subvalidators => new ValidatorBag { Assets, Simulations };

        public MaintainableAsset AddAsset() => _Assets.GetAdd(new MaintainableAsset(this));

        public Simulation AddSimulation() => _Simulations.GetAdd(new Simulation(this));

        public void ClearAssets() => _Assets.Clear();

        public ValidationResultBag GetDirectValidationResults()
        {
            var results = new ValidationResultBag();

            if (string.IsNullOrWhiteSpace(Name))
            {
                results.Add(ValidationStatus.Error, "Name is blank.", this, nameof(Name));
            }

            if (Assets.Count == 0)
            {
                results.Add(ValidationStatus.Error, "There are no assets.", this, nameof(Assets));
            }
            else if (Assets.Select(asset => asset.Name).Distinct().Count() < Assets.Count)
            {
                results.Add(ValidationStatus.Error, "Multiple assets have the same name.", this, nameof(Assets));
            }

            if (Simulations.Select(simulation => simulation.Name).Distinct().Count() < Simulations.Count)
            {
                results.Add(ValidationStatus.Error, "Multiple simulations have the same name.", this, nameof(Simulations));
            }

            return results;
        }

        public void Remove(Simulation simulation) => _Simulations.Remove(simulation);

        private readonly List<MaintainableAsset> _Assets = new List<MaintainableAsset>();

        private readonly List<Simulation> _Simulations = new List<Simulation>();

        private string _SpatialWeightUnit = "";

        public string ShortDescription => Name;
    }
}
