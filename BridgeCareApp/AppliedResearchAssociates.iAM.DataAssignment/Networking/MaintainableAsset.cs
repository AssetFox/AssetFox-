using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataAssignment.Aggregation;
using AppliedResearchAssociates.iAM.DataMiner;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;
using DataMinerAttribute = AppliedResearchAssociates.iAM.DataMiner.Attributes.Attribute;

namespace AppliedResearchAssociates.iAM.DataAssignment.Networking
{
    public class MaintainableAsset
    {
        public MaintainableAsset(Guid id, Guid networkId, Location location)
        {
            Id = id;
            NetworkId = networkId;
            Location = location;
        }

        public AggregatedResult<T> GetAggregatedValuesByYear<T>(DataMinerAttribute attribute, AggregationRule<T> aggregationRule)
        {
            var specifiedData = AssignedData.Where(_ => _.Attribute.Id == attribute.Id);
            return new AggregatedResult<T>(Guid.NewGuid(), this, aggregationRule.Apply(specifiedData, attribute));
        }

        // TODO: side effect => mutate (get area equation; calculate spatial weighting)
        public void AssignSpatialWeighting()
        {
            // run spatial weighting equation to assign SpatialWeighting value here
        }

        public void AssignAttributeData(IEnumerable<IAttributeDatum> attributeData)
        {
            foreach (var datum in attributeData)
            {
                if (datum.Location.MatchOn(Location))
                {
                    AssignedData.Add(datum);
                }
                else
                {
                    // TODO: No matching maintainable asset for the current data. What do we do?
                }
            }
        }

        public void AssignAttributeDataFromDataSource(IEnumerable<IAttributeDatum> attributeData) => AssignedData.AddRange(attributeData);

        public List<IAttributeDatum> AssignedData { get; } = new List<IAttributeDatum>();

        public Guid Id { get; }

        public Guid NetworkId { get; }

        public SpatialWeighting SpatialWeighting { get; set; }

        public Location Location { get; }
    }
}
