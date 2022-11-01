using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.Hubs.Interfaces;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using BridgeCareCore.Controllers;
using Microsoft.AspNetCore.Http;
using Moq;

namespace BridgeCareCoreTests.Tests
{
    public static class TestTreatmentControllerSetup
    {
        public static TreatmentController Create(
            Mock<IUnitOfWork> unitOfWork,
            IHttpContextAccessor contextAccessor,
            Mock<IHubService> hubServiceMock = null
            )
        {
            var esecSecurity = EsecSecurityMocks.AdminMock;
            var claimHelper = ClaimHelperMocks.New();
            var treatmentService = TreatmentServiceMocks.EmptyMock;
            var controller = new TreatmentController(
                treatmentService.Object,
                esecSecurity.Object,
                unitOfWork.Object,
                hubServiceMock.Object,
                contextAccessor,
                claimHelper.Object
                );
            return controller;
        }
    }
}
