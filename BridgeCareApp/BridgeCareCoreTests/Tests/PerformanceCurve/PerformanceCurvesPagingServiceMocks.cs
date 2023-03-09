using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BridgeCareCore.Interfaces;
using Moq;

namespace BridgeCareCoreTests.Tests.PerformanceCurve
{
    public class PerformanceCurvesPagingServiceMocks
    {
        public static Mock<IPerformanceCurvesPagingService> New()
        {
            var mock = new Mock<IPerformanceCurvesPagingService>();
            return mock;
        }
    }
}
