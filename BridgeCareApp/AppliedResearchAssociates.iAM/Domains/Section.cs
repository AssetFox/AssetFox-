using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAM.Domains
{
    public sealed class Section : WeakEntity, IValidator
    {
        public Section(Facility facility) => Facility = facility ?? throw new ArgumentNullException(nameof(facility));

        public Facility Facility { get; }

        public IEnumerable<Attribute> HistoricalAttributes => HistoryPerAttribute.Keys;

        public string Name { get; set; }

        public ValidatorBag Subvalidators => new ValidatorBag();

        public void ClearHistory() => HistoryPerAttribute.Clear();

        public ValidationResultBag GetDirectValidationResults()
        {
            var results = new ValidationResultBag();

            if (string.IsNullOrWhiteSpace(Name))
            {
                results.Add(ValidationStatus.Error, "Name is blank.", this, nameof(Name));
            }

            return results;
        }

        public AttributeValueHistory<T> GetHistory<T>(Attribute<T> attribute)
        {
            if (!HistoryPerAttribute.TryGetValue(attribute, out var history))
            {
                history = new AttributeValueHistory<T>(attribute);
                HistoryPerAttribute.Add(attribute, history);
            }

            return (AttributeValueHistory<T>)history;
        }

        public bool Remove(Attribute attribute) => HistoryPerAttribute.Remove(attribute);

        internal bool HasSpatialWeight => _SpatialWeight.HasValue;

        internal double SpatialWeight
        {
            get => _SpatialWeight.Value;
            set
            {
                _SpatialWeight = value;

                if (double.IsNaN(SpatialWeight))
                {
                    throw new SimulationException("Spatial weight is not a number.");
                }
                else if (double.IsInfinity(SpatialWeight))
                {
                    throw new SimulationException("Spatial weight is infinite.");
                }
                else if (SpatialWeight <= 0)
                {
                    throw new SimulationException("Spatial weight is less than or equal to zero.");
                }
            }
        }

        internal void ClearSpatialWeight() => _SpatialWeight = null;

        private readonly Dictionary<Attribute, object> HistoryPerAttribute = new Dictionary<Attribute, object>();

        private double? _SpatialWeight;
    }
}
