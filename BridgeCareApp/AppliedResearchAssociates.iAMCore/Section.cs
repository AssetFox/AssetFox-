using System;
using System.Collections.Generic;
using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAMCore
{
    public sealed class Section : IValidator
    {
        public Section(Facility facility) => Facility = facility ?? throw new ArgumentNullException(nameof(facility));

        public static string AreaIdentifier => "AREA";

        public double Area { get; set; }

        public string AreaUnit
        {
            get => _AreaUnit;
            set => _AreaUnit = value?.Trim() ?? "";
        }

        public Facility Facility { get; }

        public IEnumerable<Attribute> HistoricalAttributes => HistoryPerAttribute.Keys;

        public string Name { get; set; }

        public ValidatorBag Subvalidators => new ValidatorBag();

        public void ClearHistory() => HistoryPerAttribute.Clear();

        public ValidationResultBag GetDirectValidationResults()
        {
            var results = new ValidationResultBag();

            if (double.IsNaN(Area))
            {
                results.Add(ValidationStatus.Error, "Area is not a number.", this, nameof(Area));
            }
            else if (double.IsInfinity(Area))
            {
                results.Add(ValidationStatus.Error, "Area is infinite.", this, nameof(Area));
            }
            else if (Area <= 0)
            {
                results.Add(ValidationStatus.Error, "Area is less than or equal to zero.", this, nameof(Area));
            }

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

        private string _AreaUnit = "";
    }
}
