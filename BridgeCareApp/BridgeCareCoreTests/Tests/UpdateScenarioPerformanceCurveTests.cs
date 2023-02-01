using AppliedResearchAssociates.iAM.Common;
using AppliedResearchAssociates.iAM.DataPersistenceCore;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.UnitTestsCore.Extensions;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Repositories;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using BridgeCareCore.Controllers;
using BridgeCareCore.Models;
using BridgeCareCore.Services;
using BridgeCareCoreTests.Helpers;
using Moq;
using MoreLinq;
using Xunit;
using Assert = Xunit.Assert;

namespace BridgeCareCoreTests.Tests
{
    public class UpdateScenarioPerformanceCurveTests
    {
        private PerformanceCurveController CreateController(Mock<IUnitOfWork> unitOfWork)
        {
            var security = EsecSecurityMocks.AdminMock;
            var hubService = HubServiceMocks.DefaultMock();
            var accessor = HttpContextAccessorMocks.DefaultMock();
            var claimHelper = ClaimHelperMocks.New();
            var expressionValidationService = ExpressionValidationServiceMocks.EverythingIsValid();
            var performanceCurvesService = new PerformanceCurvesService(unitOfWork.Object, hubService.Object, expressionValidationService.Object);
            var pagingService = new PerformanceCurvesPagingService(unitOfWork.Object);
            var controller = new PerformanceCurveController(
                security.Object,
                unitOfWork.Object,
                hubService.Object,
                accessor.Object,
                performanceCurvesService,
                pagingService,
                claimHelper.Object
                );
            return controller;
        }
        private void Setup()
        {
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            NetworkTestSetup.CreateNetwork(TestHelper.UnitOfWork);
        }

        [Fact]
        public async Task UpsertScenarioPerformanceCurves_CurveUpdateInUpsert_PassesToRepo()
        {
            var unitOfWork = UnitOfWorkMocks.EveryoneExists();
            var performanceCurveRepo = PerformanceCurveRepositoryMocks.New(unitOfWork);
            var controller = CreateController(unitOfWork);
            var simulationId = Guid.NewGuid();
            var curveId = Guid.NewGuid();
            var libraryId = Guid.NewGuid();
            var performanceCurveDto1 = PerformanceCurveDtos.Dto(curveId, libraryId);
            var performanceCurveDto2 = PerformanceCurveDtos.Dto(curveId, libraryId);
            performanceCurveRepo.Setup(p => p.GetScenarioPerformanceCurvesOrderedById(simulationId)).ReturnsList(performanceCurveDto1);
            performanceCurveDto2.Shift = true;

            var request = new PagingSyncModel<PerformanceCurveDTO>()
            {
                UpdateRows = new List<PerformanceCurveDTO> { performanceCurveDto2 },
                AddedRows = new List<PerformanceCurveDTO>(),
                RowsForDeletion = new List<Guid>()
            };

            // Act
            await controller.UpsertScenarioPerformanceCurves(simulationId, request);

            // assert
            var upsertCall = performanceCurveRepo.SingleInvocationWithName(nameof(IPerformanceCurveRepository.UpsertOrDeleteScenarioPerformanceCurves));
            var expectedArgument = Lists.New(performanceCurveDto2);
            ObjectAssertions.Equivalent(expectedArgument, upsertCall.Arguments[0]);
            Assert.Equal(simulationId, upsertCall.Arguments[1]);
        }

        [Fact]
        public async Task UpsertScenarioPerformanceCurves_CurveDeletionInUpsert_PassesToRepo()
        {
            var unitOfWork = UnitOfWorkMocks.EveryoneExists();
            var performanceCurveRepo = PerformanceCurveRepositoryMocks.New(unitOfWork);
            var controller = CreateController(unitOfWork);
            var simulationId = Guid.NewGuid();
            var curveId = Guid.NewGuid();
            var libraryId = Guid.NewGuid();
            var performanceCurveDto1 = PerformanceCurveDtos.Dto(curveId, libraryId);
            performanceCurveRepo.Setup(p => p.GetScenarioPerformanceCurvesOrderedById(simulationId)).ReturnsList(performanceCurveDto1);

            var request = new PagingSyncModel<PerformanceCurveDTO>()
            {
                UpdateRows = new List<PerformanceCurveDTO>(),
                AddedRows = new List<PerformanceCurveDTO>(),
                RowsForDeletion = new List<Guid> { curveId },
            };

            // Act
            await controller.UpsertScenarioPerformanceCurves(simulationId, request);

            // assert
            var upsertCall = performanceCurveRepo.SingleInvocationWithName(nameof(IPerformanceCurveRepository.UpsertOrDeleteScenarioPerformanceCurves));
            var expectedArgument = new List<PerformanceCurveDTO>();
            ObjectAssertions.Equivalent(expectedArgument, upsertCall.Arguments[0]);
            Assert.Equal(simulationId, upsertCall.Arguments[1]);
        }

        [Fact]
        public async Task UpsertSimulationPerformanceCurve_AddInRequest_PassesToRepo()
        {
            var unitOfWork = UnitOfWorkMocks.EveryoneExists();
            var performanceCurveRepo = PerformanceCurveRepositoryMocks.New(unitOfWork);
            var controller = CreateController(unitOfWork);
            var simulationId = Guid.NewGuid();
            var curveId = Guid.NewGuid();
            var libraryId = Guid.NewGuid();
            var performanceCurveDto = PerformanceCurveDtos.Dto(curveId, libraryId);
            performanceCurveRepo.Setup(p => p.GetScenarioPerformanceCurvesOrderedById(simulationId)).ReturnsEmptyList();

            var request = new PagingSyncModel<PerformanceCurveDTO>()
            {
                AddedRows = new List<PerformanceCurveDTO> { performanceCurveDto },
                UpdateRows = new List<PerformanceCurveDTO>(),
                RowsForDeletion = new List<Guid>()
            };

            // Act
            await controller.UpsertScenarioPerformanceCurves(simulationId, request);

            // assert
            var upsertCall = performanceCurveRepo.SingleInvocationWithName(nameof(IPerformanceCurveRepository.UpsertOrDeleteScenarioPerformanceCurves));
            var expectedArgument = Lists.New(performanceCurveDto);
            ObjectAssertions.Equivalent(expectedArgument, upsertCall.Arguments[0]);
            Assert.Equal(simulationId, upsertCall.Arguments[1]);
        }
    }
}
