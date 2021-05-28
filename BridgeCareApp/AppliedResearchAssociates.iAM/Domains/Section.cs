using System;
using System.Collections.Generic;
using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAM.Domains
{
    public sealed class Section : WeakEntity, IValidator
    {
        internal Section(Facility facility)
        {
            Facility = facility ?? throw new ArgumentNullException(nameof(facility));

            SpatialWeighting = new Equation(Facility.Network.Explorer);
        }

        public Facility Facility { get; }

        public IEnumerable<Attribute> HistoricalAttributes => HistoryPerAttribute.Keys;

        public string Name { get; set; }

        public Equation SpatialWeighting { get; }

        public ValidatorBag Subvalidators => new ValidatorBag { SpatialWeighting };

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

        private readonly Dictionary<Attribute, object> HistoryPerAttribute = new Dictionary<Attribute, object>();
        public string ShortDescription => Name;
    }
}
