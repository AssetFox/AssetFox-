using System;
using System.Collections.Generic;
using System.Text;

namespace AppliedResearchAssociates.iAM.Data.Aggregation
{
    public static class AggregationLogMessageBuilders
    {
        internal static AggregationLogMessageBuilder RuntimeWarning(AggregationLogMessageBuilder inner)
            => new AggregationLogMessageBuilder
            {
                AggregationId = inner.AggregationId,
                Message = inner.Message,
            };
    }
}
