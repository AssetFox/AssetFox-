using System;
using System.Collections.Generic;
using System.Text;

namespace AppliedResearchAssociates.iAM.Data.Aggregation
{
    public class AggregationLogEventArgs : EventArgs
    {
        public AggregationLogEventArgs(AggregationLogMessageBuilder messageBuilder) {
            MessageBuilder = messageBuilder;
        }
        public AggregationLogMessageBuilder MessageBuilder { get; set; }
    }
}
