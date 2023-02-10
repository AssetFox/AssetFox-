using System.Data;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.CalculatedAttribute;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.CalculatedAttribute;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Attributes;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using BridgeCareCore.Models;
using BridgeCareCore.Services;
using BridgeCareCoreTests.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace BridgeCareCoreTests.Tests
{
    public class CalculatedAttributeServiceTests
    {
        private UnitOfDataPersistenceWork _testRepo;
        private Mock<IAMContext> _mockedContext;
        private Mock<DbSet<CalculatedAttributeLibraryEntity>> _mockLibrary;

        private List<CalculatedAttributeDTO> _testScenarionCalcAttributes;
        private List<CalculatedAttributeDTO> _testLibraryCalcAttributes;

        public CalculatedAttributeServiceTests()
        {
            // Create main test context
            _mockedContext = new Mock<IAMContext>();

            var libraryRepo = TestDataForCalculatedAttributesRepository.GetLibraryRepo();
            _mockLibrary = MockedContextBuilder.AddDataSet(_mockedContext, _ => _.CalculatedAttributeLibrary, libraryRepo);
            _mockLibrary.Setup(_ => _.Add(It.IsAny<CalculatedAttributeLibraryEntity>())).Returns<CalculatedAttributeDTO>(null);

            var libraryCalcAttrRepo = libraryRepo.SelectMany(_ => _.CalculatedAttributes);
            MockedContextBuilder.AddDataSet(_mockedContext, _ => _.CalculatedAttribute, libraryCalcAttrRepo);
            _testLibraryCalcAttributes = libraryCalcAttrRepo.Where(_ => _.CalculatedAttributeLibraryId ==
            new Guid("86bf65df-5ac9-44cc-b26d-9a1182c258d4")).Select(_ => _.ToDto()).ToList();

            var scenarioRepo = TestDataForCalculatedAttributesRepository.GetSimulationCalculatedAttributesRepo();
            MockedContextBuilder.AddDataSet(_mockedContext, _ => _.ScenarioCalculatedAttribute, scenarioRepo);
            _testScenarionCalcAttributes = scenarioRepo.Where(_ => _.SimulationId ==
             new Guid("440ab4bb-da87-4aee-868b-4272910fae9b")).Select(_ => _.ToDto()).ToList();

            var attributeRepo = TestDataForCalculatedAttributesRepository.GetAttributeRepo();
            MockedContextBuilder.AddDataSet(_mockedContext, _ => _.Attribute, attributeRepo);

            var simulationRepo = TestDataForCalculatedAttributesRepository.GetSimulations();
            var simulationLibrary = MockedContextBuilder.AddDataSet(_mockedContext, _ => _.Simulation, simulationRepo);

            var mockedRepo = new UnitOfDataPersistenceWork((new Mock<IConfiguration>()).Object, _mockedContext.Object);
            _testRepo = mockedRepo;
        }

        public const string Reason = "Delaying these until I test paging services";

        [Fact]
        public void GetScenarioPage_DefaultEquationIdHasExpectedValue()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var attribute = AttributeDtos.Age;
            var attributeRepo = AttributeRepositoryMocks.New(unitOfWork);
            attributeRepo.Setup(a => a.GetSingleById(attribute.Id)).Returns(attribute);
            var calculatedAttributeRepo = CalculatedAttributeRepositoryMocks.New(unitOfWork);
            var libraryId = Guid.NewGuid();
            var calculatedAttributeId = Guid.NewGuid();
            var calculatedAttributeEquationCriterionId = Guid.NewGuid();
            var calculatedAttribute = CalculatedAttributeDtos.ForAttribute(attribute, calculatedAttributeId, calculatedAttributeEquationCriterionId);
            var equation = calculatedAttribute.Equations.FirstOrDefault();
            calculatedAttributeRepo.Setup(c => c.GetLibraryCalulatedAttributesByLibraryAndAttributeId(libraryId, attribute.Id)).Returns(calculatedAttribute);
            var service = new CalculatedAttributePagingService(unitOfWork.Object);
            var request = new CalculatedAttributePagingRequestModel()
            {
                AttributeId = attribute.Id,
                SyncModel = new CalculatedAttributePagingSyncModel { AddedCalculatedAttributes = new List<CalculatedAttributeDTO>() }
            };

            var result = service.GetScenarioPage(libraryId, request) as CalculcatedAttributePagingPageModel;

            Assert.Equal(calculatedAttributeEquationCriterionId, result.DefaultEquation.Id);
        }

        [Fact]
        public void GetScenarioCalculatedAttributePageTest()
        {
            var service = new CalculatedAttributePagingService(_testRepo);

            var simulationId = TestDataForCalculatedAttributesRepository.FirstSimulationId;

            var request = new CalculatedAttributePagingRequestModel()
            {
                AttributeId = TestDataForCalculatedAttributesRepository.GetAttributeRepo().First().Id,
                SyncModel = new CalculatedAttributePagingSyncModel { AddedCalculatedAttributes = new List<CalculatedAttributeDTO>() }
            };

            var result = service.GetLibraryPage(simulationId, request) as CalculcatedAttributePagingPageModel;
            var pairIds = _testScenarionCalcAttributes.First(_ => _.Attribute ==
            TestDataForCalculatedAttributesRepository.GetAttributeRepo().First().Name).Equations.Select(_ => _.Id).ToList();

            Assert.True(pairIds.First() == result.DefaultEquation.Id);
        }

        [Fact]
        public void GetSyncedScenarioDatasetTest()
        {
            var service = new CalculatedAttributePagingService(_testRepo);

            var simulationId = TestDataForCalculatedAttributesRepository.FirstSimulationId;

            var SyncModel = new CalculatedAttributePagingSyncModel { AddedCalculatedAttributes = new List<CalculatedAttributeDTO>() };

            var result = service.GetSyncedScenarioDataSet(simulationId, SyncModel);
            var calcAttrIds = _testScenarionCalcAttributes.Select(_ => _.Id);
            Assert.True(result.Where(_ => calcAttrIds.Contains(_.Id)).Count() == calcAttrIds.Count());
        }

        [Fact]
        public void GetSyncedLibraryDatasetTest()
        {
            var service = new CalculatedAttributePagingService(_testRepo);

            var libraryId = TestDataForCalculatedAttributesRepository.GetLibraryRepo().First(_ => _.Name == "First").Id;

            var SyncModel = new CalculatedAttributePagingSyncModel { AddedCalculatedAttributes = new List<CalculatedAttributeDTO>() };

            var result = service.GetSyncedLibraryDataset(libraryId, SyncModel);
            var calcAttrIds = _testLibraryCalcAttributes.Select(_ => _.Id);
            Assert.True(result.Where(_ => calcAttrIds.Contains(_.Id)).Count() == calcAttrIds.Count());
        }
    }
}
