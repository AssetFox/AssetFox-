using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppliedResearchAssociates.iAM.Common.Logging
{
    public class DoNotWorkQueueLog : IWorkQueueLog
    {
        public void UpdateWorkQueueStatus(Guid workId, string statusMessage) { }
    }
}
