using System;
using System.Linq;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using BridgeCareCore.Controllers;
using BridgeCareCore.Interfaces.DefaultData;
using BridgeCareCore.Models.DefaultData;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.APITestClasses
{
    public class AnalysisMethodTests
    {
        private TestHelper _testHelper => TestHelper.Instance;

        private static readonly Guid BenefitId = Guid.Parse("be2497dd-3acd-4cdd-88a8-adeb9893f1df");
        private readonly Mock<IAnalysisDefaultDataService> _mockAnalysisDefaultDataService = new Mock<IAnalysisDefaultDataService>();

        private AnalysisMethodController SetupController()
        {
            _testHelper.CreateSingletons();
            _testHelper.CreateSimulation();
            _mockAnalysisDefaultDataService.Setup(m => m.GetAnalysisDefaultData()).ReturnsAsync(new AnalysisDefaultData());
            var controller = new AnalysisMethodController(_testHelper.MockEsecSecurityAdmin.Object, _testHelper.UnitOfWork,
                _testHelper.MockHubService.Object, _testHelper.MockHttpContextAccessor.Object, _mockAnalysisDefaultDataService.Object);
            return controller;
        }

        public AnalysisMethodEntity TestAnalysis(Guid simulationId, Guid? id = null)
        {
            var resolveId = id ?? Guid.NewGuid();
            var returnValue = new AnalysisMethodEntity
            {
                Id = resolveId,
                SimulationId = simulationId,
                OptimizationStrategy = OptimizationStrategy.Benefit,
                SpendingStrategy = SpendingStrategy.NoSpending,
                ShouldApplyMultipleFeasibleCosts = false,
                ShouldDeteriorateDuringCashFlow = false,
                ShouldUseExtraFundsAcrossBudgets = false
            };
            return returnValue;
        }

        public BenefitEntity TestBenefit(Guid analysisMethodId, Guid? benefitId = null)
        {
            var resolveId = benefitId ?? Guid.NewGuid();
            var returnValue = new BenefitEntity
            {
                Id = resolveId,
                AnalysisMethodId = analysisMethodId,
                Limit = 1
            };
            return returnValue;
        }

        private AnalysisMethodEntity SetupForGet(Guid simulationId)
        {
            var entity = TestAnalysis(simulationId);

            _testHelper.UnitOfWork.Context.AnalysisMethod.Add(entity);
            _testHelper.UnitOfWork.Context.SaveChanges();
            return entity;
        }

        private CriterionLibraryEntity SetupForUpsert(Guid simulationId)
        {
            SetupForGet(simulationId);
            var criterionLibrary = _testHelper.TestCriterionLibrary();
            _testHelper.UnitOfWork.Context.CriterionLibrary.Add(criterionLibrary);
            _testHelper.UnitOfWork.Context.SaveChanges();
            return criterionLibrary;
        }

        [Fact]
        public async Task ShouldReturnOkResultOnGet()
        {
            var controller = SetupController();
            // Act
            var simulation = _testHelper.CreateSimulation();
            var result = await controller.AnalysisMethod(simulation.Id);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task ShouldReturnOkResultOnPost()
        {
            // Arrange
            var controller = SetupController();
            var simulation = _testHelper.CreateSimulation();
            var analysisEntity = TestAnalysis(simulation.Id);
            var attributeEntity = _testHelper.UnitOfWork.Context.Attribute.First();
            var dto = analysisEntity.ToDto();
            var benefit = TestBenefit(analysisEntity.Id);
            benefit.Attribute = attributeEntity;
            dto.Benefit = benefit.ToDto();
            dto.Benefit.Attribute = attributeEntity.Name;

            // Act
            var result =
                await controller.UpsertAnalysisMethod(simulation.Id, dto);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task ShouldGetAnalysisMethod()
        {
            // Arrange
            var controller = SetupController();
            var simulation = _testHelper.CreateSimulation();
            var analysisMethodEntity = SetupForGet(simulation.Id);

            // Act
            var result = await controller.AnalysisMethod(simulation.Id);

            // Assert
            var okObjResult = result as OkObjectResult;
            Assert.NotNull(okObjResult.Value);

            var dto = (AnalysisMethodDTO)Convert.ChangeType(okObjResult.Value, typeof(AnalysisMethodDTO));

            Assert.Equal(analysisMethodEntity.Id, dto.Id);
        }

        [Fact]
        public async Task ShouldCreateAnalysisMethod()
        {
            // Arrange
            var controller = SetupController();
            var simulation = _testHelper.CreateSimulation();
            var getResult = await controller.AnalysisMethod(simulation.Id);
            var analysisMethodDto = (AnalysisMethodDTO)Convert.ChangeType((getResult as OkObjectResult).Value,
                typeof(AnalysisMethodDTO));

            analysisMethodDto.Benefit = new BenefitDTO
            {
                Id = Guid.NewGuid(),
                Limit = 0.0,
                Attribute = _testHelper.UnitOfWork.Context.Attribute.First().Name
            };

            // Act
            await controller.UpsertAnalysisMethod(simulation.Id, analysisMethodDto);

            // Assert
            getResult = await controller.AnalysisMethod(simulation.Id);
            var upsertedAnalysisMethodDto = (AnalysisMethodDTO)Convert.ChangeType((getResult as OkObjectResult).Value,
                typeof(AnalysisMethodDTO));

            Assert.Equal(analysisMethodDto.Id, upsertedAnalysisMethodDto.Id);
            Assert.Equal(analysisMethodDto.Benefit.Id, upsertedAnalysisMethodDto.Benefit.Id);
        }

        [Fact]
        public async Task ShouldUpdateAnalysisMethod()
        {
            // Arrange
            var controller = SetupController();
            var simulation = _testHelper.CreateSimulation();
            var criterionLibrary = SetupForUpsert(simulation.Id);
            var getResult = await controller.AnalysisMethod(simulation.Id);
            var dto = (AnalysisMethodDTO)Convert.ChangeType((getResult as OkObjectResult).Value,
                typeof(AnalysisMethodDTO));
            var attributeEntity = _testHelper.UnitOfWork.Context.Attribute.First();
            dto.Attribute = attributeEntity.Name;
            dto.CriterionLibrary = criterionLibrary.ToDto();
            var analysisMethod = TestAnalysis(simulation.Id);
            var benefit = TestBenefit(analysisMethod.Id);
            benefit.Attribute = attributeEntity;
            dto.Benefit = benefit.ToDto();
            dto.Benefit.Attribute = attributeEntity.Name;

            // Act
            await controller.UpsertAnalysisMethod(simulation.Id, dto);

            // Assert
            var analysisMethodDto =
                _testHelper.UnitOfWork.AnalysisMethodRepo.GetAnalysisMethod(simulation.Id);

            Assert.Equal(dto.Id, analysisMethodDto.Id);
            Assert.Equal(dto.Attribute, analysisMethodDto.Attribute);
            Assert.Equal(dto.CriterionLibrary.Id, analysisMethodDto.CriterionLibrary.Id);
            Assert.Equal(dto.Benefit.Id, analysisMethodDto.Benefit.Id);
            Assert.Equal(dto.Benefit.Attribute, analysisMethodDto.Benefit.Attribute);
        }
    }
}
