using System;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public class KeySegmentDatum
    {
        public Guid SegmentId { get; set; }
        public SegmentAttributeDatum KeyValue { get; set; }
    }
}
