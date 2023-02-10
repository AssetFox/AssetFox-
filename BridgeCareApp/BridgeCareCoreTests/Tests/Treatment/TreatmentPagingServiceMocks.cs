using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BridgeCareCore.Interfaces;
using Moq;

namespace BridgeCareCoreTests.Tests.Treatment
{
    public static class TreatmentPagingServiceMocks
    {
        public static Mock<ITreatmentPagingService> EmptyMock => new Mock<ITreatmentPagingService>();

    }
}
