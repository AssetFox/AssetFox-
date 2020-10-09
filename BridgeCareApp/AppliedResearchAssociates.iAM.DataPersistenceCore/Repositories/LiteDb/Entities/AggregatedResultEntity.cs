using System;
using System.Collections.Generic;
using System.Text;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.LiteDb.Entities
{
    public class AggregatedResultEntity<T>
    {
        public IEnumerable<(AttributeEntity attribute, (int year, T value))> AggregatedData { get; set; }
        public MaintainableAssetEntity MaintainableAssetEntity { get; set; }
    }
}
