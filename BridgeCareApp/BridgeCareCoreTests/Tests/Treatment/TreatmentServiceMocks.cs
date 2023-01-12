using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BridgeCareCore.Interfaces;
using Moq;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils
{
    public static class TreatmentServiceMocks
    {
        public static Mock<ITreatmentService> EmptyMock => new Mock<ITreatmentService>();
        public static Mock<ITreatmentPagingService> EmptyPagingMock => new Mock<ITreatmentPagingService>();
    }
}
