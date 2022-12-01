using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Moq;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.CalculatedAttribute;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.CalculatedAttribute;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using System.Data;
using BridgeCareCore.Services;
using BridgeCareCore.Models;

namespace BridgeCareCoreTests.Tests
{
    public class CalculatedAttributeServiceTests
    {
        private UnitOfDataPersistenceWork _testRepo;
        private UnitOfDataPersistenceWork _emptyTestRepo;
        private UnitOfDataPersistenceWork _isCalcualtedTestRepo;
        private Mock<IAMContext> _mockedContext;
        private Mock<IAMContext> _emptyMockedContext;
        private Mock<DbSet<CalculatedAttributeLibraryEntity>> _mockLibrary;
        private Mock<DbSet<CalculatedAttributeEntity>> _mockLibrarycalcAttr;
        private Mock<DbSet<ScenarioCalculatedAttributeEntity>> _mockScenarioCalculations;
        private Mock<DbSet<AttributeEntity>> _mockAttributes;

        private List<CalculatedAttributeDTO> _testScenarionCalcAttributes;
        private List<CalculatedAttributeDTO> _testLIbraryCalcAttributes;

        public CalculatedAttributeServiceTests()
        {
            // Create main test context
            _mockedContext = new Mock<IAMContext>();

            var libraryRepo = TestDataForCalculatedAttributesRepository.GetLibraryRepo();
            _mockLibrary = MockedContextBuilder.AddDataSet(_mockedContext, _ => _.CalculatedAttributeLibrary, libraryRepo);
            _mockLibrary.Setup(_ => _.Add(It.IsAny<CalculatedAttributeLibraryEntity>())).Returns<CalculatedAttributeDTO>(null);

            var libraryCalcAttrRepo = libraryRepo.SelectMany(_ => _.CalculatedAttributes);
            _mockLibrarycalcAttr = MockedContextBuilder.AddDataSet(_mockedContext, _ => _.CalculatedAttribute, libraryCalcAttrRepo);
            _testLIbraryCalcAttributes = libraryCalcAttrRepo.Where(_ => _.CalculatedAttributeLibraryId ==
            new Guid("86bf65df-5ac9-44cc-b26d-9a1182c258d4")).Select(_ => _.ToDto()).ToList();

            var scenarioRepo = TestDataForCalculatedAttributesRepository.GetSimulationCalculatedAttributesRepo();
            _mockScenarioCalculations = MockedContextBuilder.AddDataSet(_mockedContext, _ => _.ScenarioCalculatedAttribute, scenarioRepo);
            _testScenarionCalcAttributes = scenarioRepo.Where(_ => _.SimulationId ==
             new Guid("440ab4bb-da87-4aee-868b-4272910fae9b")).Select(_ => _.ToDto()).ToList();

            var attributeRepo = TestDataForCalculatedAttributesRepository.GetAttributeRepo();
            _mockAttributes = MockedContextBuilder.AddDataSet(_mockedContext, _ => _.Attribute, attributeRepo);

            var simulationRepo = TestDataForCalculatedAttributesRepository.GetSimulations();
            var simulationLibrary = MockedContextBuilder.AddDataSet(_mockedContext, _ => _.Simulation, simulationRepo);

            var mockedRepo = new UnitOfDataPersistenceWork((new Mock<IConfiguration>()).Object, _mockedContext.Object);
            _testRepo = mockedRepo;
        }

        [Fact]
        public void GetLibraryCalculatedAttributePageTest()
        {
            var service = new CalculatedAttributeService(_testRepo);

            var libraryId = TestDataForCalculatedAttributesRepository.GetLibraryRepo().First(_ => _.Name == "First").Id;

            var request = new CalculatedAttributePagingRequestModel()
            {
                AttributeId = TestDataForCalculatedAttributesRepository.GetAttributeRepo().First().Id,
                SyncModel = new CalculatedAttributePagingSyncModel { AddedCalculatedAttributes = new List<CalculatedAttributeDTO>() }
            };

            var result = service.GetLibraryCalculatedAttributePage(libraryId, request) as CalculcatedAttributePagingPageModel;
            var pairIds = _testLIbraryCalcAttributes.First(_ => _.Attribute ==
            TestDataForCalculatedAttributesRepository.GetAttributeRepo().First().Name).Equations.Select(_ => _.Id).ToList();

            Assert.True(pairIds.First() == result.DefaultEquation.Id);
        }

        [Fact]
        public void GetScenarioCalculatedAttributePageTest()
        {
            var service = new CalculatedAttributeService(_testRepo);

            var simulationId = TestDataForCalculatedAttributesRepository.GetSimulations().First(_ => _.Name == "First").Id;

            var request = new CalculatedAttributePagingRequestModel()
            {
                AttributeId = TestDataForCalculatedAttributesRepository.GetAttributeRepo().First().Id,
                SyncModel = new CalculatedAttributePagingSyncModel { AddedCalculatedAttributes = new List<CalculatedAttributeDTO>() }
            };

            var result = service.GetScenarioCalculatedAttributePage(simulationId, request) as CalculcatedAttributePagingPageModel;
            var pairIds = _testScenarionCalcAttributes.First(_ => _.Attribute ==
            TestDataForCalculatedAttributesRepository.GetAttributeRepo().First().Name).Equations.Select(_ => _.Id).ToList();

            Assert.True(pairIds.First() == result.DefaultEquation.Id);
        }

        [Fact]
        public void GetSyncedScenarioDatasetTest()
        {
            var service = new CalculatedAttributeService(_testRepo);

            var simulationId = TestDataForCalculatedAttributesRepository.GetSimulations().First(_ => _.Name == "First").Id;

            var SyncModel = new CalculatedAttributePagingSyncModel { AddedCalculatedAttributes = new List<CalculatedAttributeDTO>() };

            var result = service.GetSyncedScenarioDataset(simulationId, SyncModel);
            var calcAttrIds = _testScenarionCalcAttributes.Select(_ => _.Id);
            Assert.True(result.Where(_ => calcAttrIds.Contains(_.Id)).Count() == calcAttrIds.Count());
        }

        [Fact]
        public void GetSyncedLibraryDatasetTest()
        {
            var service = new CalculatedAttributeService(_testRepo);

            var libraryId = TestDataForCalculatedAttributesRepository.GetLibraryRepo().First(_ => _.Name == "First").Id;

            var SyncModel = new CalculatedAttributePagingSyncModel { AddedCalculatedAttributes = new List<CalculatedAttributeDTO>() };

            var result = service.GetSyncedLibraryDataset(libraryId, SyncModel);
            var calcAttrIds = _testLIbraryCalcAttributes.Select(_ => _.Id);
            Assert.True(result.Where(_ => calcAttrIds.Contains(_.Id)).Count() == calcAttrIds.Count());
        }
    }
}
