using System;
using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.DataMiner.Attributes
{
    public static class AttributeDatumBuilder<T>
    {
        public static List<IAttributeDatum> CreateAttributeData(Attribute attribute, IEnumerable<(Location location, T value)> locationsAndValues)
        {
            var attributeData = new List<IAttributeDatum>();
            foreach (var locationAndValue in locationsAndValues)
            {
                attributeData.Add(new AttributeDatum<T>(attribute, locationAndValue.value, locationAndValue.location, DateTime.Now));
            }
            return attributeData;
        }
    }
}
