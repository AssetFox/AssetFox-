using System;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.Generics
{
    public class KeySegmentDatum
    {
        public Guid AssetId { get; set; }
        public SegmentAttributeDatum KeyValue { get; set; }
    }
}
