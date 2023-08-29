using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DTOs.Enums;

namespace AppliedResearchAssociates.iAM.WorkQueue
{
    public static class WorkQueueWorkIdFactory
    {
        public static string CreateId(Guid domainId, WorkType workType)
        {
            return domainId.ToString() + workType.ToString();
        }
    }
}
