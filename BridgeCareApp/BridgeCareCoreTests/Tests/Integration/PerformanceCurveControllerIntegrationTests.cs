using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Repositories;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using BridgeCareCore.Controllers;
using BridgeCareCore.Models;
using BridgeCareCore.Services;
using BridgeCareCoreTests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace BridgeCareCoreTests.Tests.Integration
{
    public class PerformanceCurveControllerIntegrationTests
    {
        public PerformanceCurveController CreateController()
        {
            var security = EsecSecurityMocks.Admin;
            var hubService = HubServiceMocks.Default();
            var contextAccessor = HttpContextAccessorMocks.Default();
            var claimHelper = ClaimHelperMocks.New();
            var expressionValidationService = ExpressionValidationServiceMocks.EverythingIsValid();
            var service = new PerformanceCurvesService(TestHelper.UnitOfWork, hubService, expressionValidationService.Object);
            var pagingService = new PerformanceCurvesPagingService(TestHelper.UnitOfWork);
            var controller = new PerformanceCurveController(
                security,
                TestHelper.UnitOfWork,
                hubService,
                contextAccessor,
                service,
                pagingService,
                claimHelper.Object);
            return controller;
        }

        [Fact]
        public async void UpsertPerformanceCurveLibrary_CurveUpsertThrows_LibraryIsNotChanged()
        {
            NetworkTestSetup.CreateNetwork(TestHelper.UnitOfWork);
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            var attributeName = TestAttributeNames.CulvDurationN;
            var controller = CreateController();
            var libraryId = Guid.NewGuid();
            var library = PerformanceCurveLibraryDtos.Empty(libraryId);
            TestHelper.UnitOfWork.PerformanceCurveRepo.UpsertPerformanceCurveLibrary(library);
            var curveId = Guid.NewGuid();
            var criterionLibraryId = Guid.NewGuid();
            var curve = PerformanceCurveDtos.Dto(curveId, criterionLibraryId, attributeName);
            var curves = new List<PerformanceCurveDTO> { curve };
            TestHelper.UnitOfWork.PerformanceCurveRepo.UpsertOrDeletePerformanceCurves(curves, libraryId);
            var updateLibrary = PerformanceCurveLibraryDtos.Empty(libraryId);
            updateLibrary.Description = "Updated description";
            var updateCurve = PerformanceCurveDtos.Dto(curveId, criterionLibraryId, "AttributeDoesNotExist");
            var syncModel = new PagingSyncModel<PerformanceCurveDTO>
            {
                UpdateRows = new List<PerformanceCurveDTO> { updateCurve },
            };
            var upsertRequest = new LibraryUpsertPagingRequestModel<PerformanceCurveLibraryDTO, PerformanceCurveDTO>
            {
                Library = library,
                SyncModel = syncModel,
            };
            var libraryBefore = TestHelper.UnitOfWork.PerformanceCurveRepo.GetPerformanceCurveLibrary(libraryId);

            var exception = await Assert.ThrowsAsync<RowNotInTableException>(async () => await controller.UpsertPerformanceCurveLibrary(upsertRequest));

            var libraryAfter = TestHelper.UnitOfWork.PerformanceCurveRepo.GetPerformanceCurveLibrary(libraryId);
            ObjectAssertions.Equivalent(libraryBefore, libraryAfter);
        }
    }
}
