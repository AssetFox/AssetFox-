using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Services;
using Moq;

namespace BridgeCareCoreTests.Tests.General_Work_Queue
{
    public static class GeneralWorkQueueServiceMocks
    {
        public static Mock<IGeneralWorkQueueService> New()
        {
            return new Mock<IGeneralWorkQueueService>();
        }
    }
}
