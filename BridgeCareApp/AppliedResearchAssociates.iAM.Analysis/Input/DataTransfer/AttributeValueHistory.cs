using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.Analysis.Input.DataTransfer
{
    public sealed class AttributeValueHistory<T>
    {
        public string AttributeID { get; set; }

        public List<HistoricalValue<T>> History { get; set; }
    }
}
