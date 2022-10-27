using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using Moq;

namespace BridgeCareCoreTests.Tests.PerformanceCurve
{
    public static class PerformanceCurveRepositoryMocks
    {
        public static Mock<IPerformanceCurveRepository> New()
        {
            var mock = new Mock<IPerformanceCurveRepository>();
            return mock;
        }
    }
}
