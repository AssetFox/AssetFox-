using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BridgeCareCore.Interfaces.DefaultData;
using BridgeCareCoreTests.Tests.AnalysisMethod;
using Moq;

namespace BridgeCareCoreTests.Tests
{
    public static class AnalysisDefaultDataServiceMocks
    {
        public static Mock<IAnalysisDefaultDataService> DefaultMock()
        {
            var mock = new Mock<IAnalysisDefaultDataService>();
            var defaultData = AnalysisDefaultDataObjects.Default;
            mock.Setup(m => m.GetAnalysisDefaultData()).ReturnsAsync(defaultData);
            return mock;
        }
    }
}
