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
using AppliedResearchAssociates.iAM.UnitTestsCore.Extensions;
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
        public void GetLibraryPage_DefaultEquationIdHasExpectedValue()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var calculatedAttributeRepo = CalculatedAttributeRepositoryMocks.New(unitOfWork);
            var calculatedAttributeId = Guid.NewGuid();
            var calculatedAttributeEquationCriterionId = Guid.NewGuid();
            var attribute = AttributeDtos.Age;
            var attributeRepo = AttributeRepositoryMocks.New(unitOfWork);
            attributeRepo.Setup(a => a.GetSingleById(attribute.Id)).Returns(attribute);
            var simulationId = Guid.NewGuid();
            var calculatedAttribute = CalculatedAttributeDtos.ForAttribute(attribute, calculatedAttributeId, calculatedAttributeEquationCriterionId);
            calculatedAttributeRepo.Setup(c => c.GetScenarioCalulatedAttributesByScenarioAndAttributeId(simulationId, attribute.Id)).Returns(calculatedAttribute);
            var service = new CalculatedAttributePagingService(unitOfWork.Object);
            var request = new CalculatedAttributePagingRequestModel()
            {
                AttributeId = attribute.Id,
                SyncModel = new CalculatedAttributePagingSyncModel { AddedCalculatedAttributes = new List<CalculatedAttributeDTO>() }
            };

            var result = service.GetLibraryPage(simulationId, request) as CalculcatedAttributePagingPageModel;

            Assert.True(calculatedAttributeEquationCriterionId == result.DefaultEquation.Id);
        }

        [Fact]
        public void GetSyncedScenarioDataSet_GrabsFromRepo()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var attribute = AttributeDtos.Age;
            var calculatedAttribute = CalculatedAttributeDtos.ForAttribute(attribute);
            var calculatedAttributeRepo = CalculatedAttributeRepositoryMocks.New(unitOfWork);
            var simulationId = Guid.NewGuid();
            var calculatedAttributes = new List<CalculatedAttributeDTO> { calculatedAttribute };
            calculatedAttributeRepo.Setup(c => c.GetScenarioCalculatedAttributes(simulationId)).Returns(calculatedAttributes);
            var service = new CalculatedAttributePagingService(unitOfWork.Object);

            var SyncModel = new CalculatedAttributePagingSyncModel { AddedCalculatedAttributes = new List<CalculatedAttributeDTO>() };

            var result = service.GetSyncedScenarioDataSet(simulationId, SyncModel);
            Assert.Equal(calculatedAttributes, result);
        }

        [Fact]
        public void GetSyncedLibraryDatasetTest()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var calculatedAttributeRepo = CalculatedAttributeRepositoryMocks.New(unitOfWork);
            var libraryId = Guid.NewGuid();
            var library = CalculatedAttributeLibraryDtos.Empty(libraryId);
            var attribute = AttributeDtos.Age;
            var calculatedAttribute = CalculatedAttributeDtos.ForAttribute(attribute);
            library.CalculatedAttributes.Add(calculatedAttribute);
            calculatedAttributeRepo.Setup(c => c.GetCalculatedAttributeLibraryByID(libraryId)).Returns(library);
            var service = new CalculatedAttributePagingService(unitOfWork.Object);
            var SyncModel = new CalculatedAttributePagingSyncModel { AddedCalculatedAttributes = new List<CalculatedAttributeDTO>() };

            var result = service.GetSyncedLibraryDataset(libraryId, SyncModel);

            var retreivedAttribute = result.Single();
            Assert.Equal(calculatedAttribute, retreivedAttribute);
        }
    }
}
