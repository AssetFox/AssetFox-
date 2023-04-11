using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.Hubs.Interfaces;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using BridgeCareCore.Controllers;
using BridgeCareCore.Interfaces;
using BridgeCareCoreTests.Tests.General_Work_Queue;
using BridgeCareCoreTests.Tests.Treatment;
using Microsoft.AspNetCore.Http;
using Moq;

namespace BridgeCareCoreTests.Tests
{
    public static class TestTreatmentControllerSetup
    {
        public static TreatmentController Create(
            Mock<IUnitOfWork> unitOfWork,
            Mock<ITreatmentService> treatmentService = null,
            Mock<ITreatmentPagingService> pagingService = null
            )
        {
            var contextAccessor = HttpContextAccessorMocks.DefaultMock();
            var hubService = HubServiceMocks.DefaultMock();
            var esecSecurity = EsecSecurityMocks.AdminMock;
            var claimHelper = ClaimHelperMocks.New();
            treatmentService ??= TreatmentServiceMocks.EmptyMock;
            pagingService ??= TreatmentPagingServiceMocks.EmptyMock;
            var generalWorkQueue = GeneralWorkQueueServiceMocks.New();
            var controller = new TreatmentController(
                treatmentService.Object,
                pagingService.Object,
                esecSecurity.Object,
                unitOfWork.Object,
                hubService.Object,
                contextAccessor.Object,
                claimHelper.Object,
                generalWorkQueue.Object
                );
            return controller;
        }
    }
}
