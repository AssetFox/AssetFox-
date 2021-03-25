using System;
using System.Collections.Generic;
using System.Text;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public class KeySegmentDatum
    {
        public Guid SegmentId { get; set; }
        public SegmentAttributeDatum KeyValue { get; set; }
    }
}
