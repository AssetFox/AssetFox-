using System;
using System.Collections.Generic;
using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAM.Analysis
{
    public sealed class AnalysisMaintainableAsset : WeakEntity, IValidator
    {
        internal AnalysisMaintainableAsset(Network network)
        {
            Network = network ?? throw new ArgumentNullException(nameof(network));
            SpatialWeighting = new Equation(Network.Explorer);
        }

        public Network Network { get; }

        public IEnumerable<Attribute> HistoricalAttributes => HistoryPerAttribute.Keys;

        public string AssetName { get; set; }

        public Equation SpatialWeighting { get; }

        public ValidatorBag Subvalidators => new ValidatorBag { SpatialWeighting };

        public void ClearHistory() => HistoryPerAttribute.Clear();

        public ValidationResultBag GetDirectValidationResults()
        {
            var results = new ValidationResultBag();

            if (string.IsNullOrWhiteSpace(AssetName))
            {
                results.Add(ValidationStatus.Error, "Name is blank.", this, nameof(AssetName));
            }

            return results;
        }

        public AttributeValueHistory<T> GetHistory<T>(Attribute<T> attribute)
        {
            if (!HistoryPerAttribute.TryGetValue(attribute, out var history))
            {
                history = new AttributeValueHistory<T>(this, attribute);
                HistoryPerAttribute.Add(attribute, history);
            }

            return (AttributeValueHistory<T>)history;
        }

        public bool Remove(Attribute attribute) => HistoryPerAttribute.Remove(attribute);

        private readonly Dictionary<Attribute, object> HistoryPerAttribute = new Dictionary<Attribute, object>();

        public string ShortDescription => AssetName;
    }
}
