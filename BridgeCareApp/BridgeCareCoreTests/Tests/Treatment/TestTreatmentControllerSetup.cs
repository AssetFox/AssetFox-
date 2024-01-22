using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Common;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.Hubs.Interfaces;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using BridgeCareCore.Controllers;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Services;
using BridgeCareCore.Services.Treatment;
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
            Mock<ITreatmentPagingService> pagingService = null
            )
        {
            var contextAccessor = HttpContextAccessorMocks.DefaultMock();
            var hubService = HubServiceMocks.DefaultMock();
            var esecSecurity = EsecSecurityMocks.AdminMock;
            var claimHelper = ClaimHelperMocks.New();
            var logger = new DoNotLog();
            var expressionValidationService = new ExpressionValidationService(unitOfWork.Object, logger);
            var treatmentLoader = new ExcelTreatmentLoader(expressionValidationService);
            var treatmentService = new TreatmentService(unitOfWork.Object, treatmentLoader);
            pagingService ??= TreatmentPagingServiceMocks.EmptyMock;
            var generalWorkQueue = GeneralWorkQueueServiceMocks.New();
            var controller = new TreatmentController(
                treatmentService,
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
