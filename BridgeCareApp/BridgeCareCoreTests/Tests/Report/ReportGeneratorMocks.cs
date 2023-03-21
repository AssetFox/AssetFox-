using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Reporting;
using Moq;

namespace BridgeCareCoreTests.Tests.Report
{
    public static class ReportGeneratorMocks
    {
        public static Mock<IReportGenerator> New()
        {
            var mock = new Mock<IReportGenerator>();
            return mock;
        }
    }
}
