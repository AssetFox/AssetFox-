using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BridgeCareCore.Interfaces;
using Moq;

namespace BridgeCareCoreTests.Tests.PerformanceCurve
{
    public class PerformanceCurveServiceMocks
    {
        public static Mock<IPerformanceCurvesService> New()
        {
            var mock = new Mock<IPerformanceCurvesService>();
            return mock;
        }
    }
}
