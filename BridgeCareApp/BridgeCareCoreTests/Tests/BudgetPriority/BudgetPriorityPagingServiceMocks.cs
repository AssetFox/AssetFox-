using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BridgeCareCore.Interfaces;
using Moq;

namespace BridgeCareCoreTests.Tests.BudgetPriority
{
    public static class BudgetPriorityPagingServiceMocks
    {
        public static Mock<IBudgetPriortyPagingService> DefaultMock()
            => new Mock<IBudgetPriortyPagingService>();
    }
}
