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
using Microsoft.EntityFrameworkCore.Infrastructure;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using System.Data;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.CalculatedAttributes
{
    public class CalculatedAttributeRepositoryTests
    {
        private UnitOfDataPersistenceWork _testRepo;
        private UnitOfDataPersistenceWork _emptyTestRepo;
        private Mock<IAMContext> _mockedContext;
        private Mock<IAMContext> _emptyMockedContext;
        private Mock<DbSet<CalculatedAttributeLibraryEntity>> _mockLibrary;
        private Mock<DbSet<ScenarioCalculatedAttributeEntity>> _mockScenarioCalculations;
        private Guid _badId;

        public CalculatedAttributeRepositoryTests()
        {
            // From https://docs.microsoft.com/en-us/ef/ef6/fundamentals/testing/mocking?redirectedfrom=MSDN

            _mockedContext = new Mock<IAMContext>();

            var libraryRepo = TestDataForCalculatedAttributesRepository.GetLibraryRepo();
            _mockLibrary = new Mock<DbSet<CalculatedAttributeLibraryEntity>>();
            _mockLibrary.As<IQueryable<CalculatedAttributeLibraryEntity>>().Setup(_ => _.Provider).Returns(libraryRepo.Provider);
            _mockLibrary.As<IQueryable<CalculatedAttributeLibraryEntity>>().Setup(_ => _.Expression).Returns(libraryRepo.Expression);
            _mockLibrary.As<IQueryable<CalculatedAttributeLibraryEntity>>().Setup(_ => _.ElementType).Returns(libraryRepo.ElementType);
            _mockLibrary.As<IQueryable<CalculatedAttributeLibraryEntity>>().Setup(_ => _.GetEnumerator()).Returns(libraryRepo.GetEnumerator());

            var mockEntry = new Mock<Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<CalculatedAttributeLibraryEntity>>(It.IsAny<string>());
            mockEntry.Setup(_ => _.Entity).Returns(libraryRepo.First);
            //var test = mockEntry.Object;
            _mockLibrary.Setup(_ => _.Add(It.IsAny<CalculatedAttributeLibraryEntity>())).Returns <CalculatedAttributeDTO>(null);

            _mockedContext.Setup(_ => _.CalculatedAttributeLibrary).Returns(_mockLibrary.Object);
            _mockedContext.Setup(_ => _.Set<CalculatedAttributeLibraryEntity>()).Returns(_mockLibrary.Object);

            var scenarioRepo = TestDataForCalculatedAttributesRepository.GetSimulationCalculatedAttributesRepo();
            _mockScenarioCalculations = new Mock<DbSet<ScenarioCalculatedAttributeEntity>>();
            _mockScenarioCalculations.As<IQueryable<ScenarioCalculatedAttributeEntity>>().Setup(_ => _.Provider).Returns(scenarioRepo.Provider);
            _mockScenarioCalculations.As<IQueryable<ScenarioCalculatedAttributeEntity>>().Setup(_ => _.Expression).Returns(scenarioRepo.Expression);
            _mockScenarioCalculations.As<IQueryable<ScenarioCalculatedAttributeEntity>>().Setup(_ => _.ElementType).Returns(scenarioRepo.ElementType);
            _mockScenarioCalculations.As<IQueryable<ScenarioCalculatedAttributeEntity>>().Setup(_ => _.GetEnumerator()).Returns(scenarioRepo.GetEnumerator());
            _mockedContext.Setup(_ => _.ScenarioCalculatedAttribute).Returns(_mockScenarioCalculations.Object);
            _mockedContext.Setup(_ => _.Set<ScenarioCalculatedAttributeEntity>()).Returns(_mockScenarioCalculations.Object);

            var attributeRepo = TestDataForCalculatedAttributesRepository.GetAttributeRepo();
            var attributeLibrary = new Mock<DbSet<AttributeEntity>>();
            attributeLibrary.As<IQueryable<AttributeEntity>>().Setup(_ => _.Provider).Returns(attributeRepo.Provider);
            attributeLibrary.As<IQueryable<AttributeEntity>>().Setup(_ => _.Expression).Returns(attributeRepo.Expression);
            attributeLibrary.As<IQueryable<AttributeEntity>>().Setup(_ => _.ElementType).Returns(attributeRepo.ElementType);
            attributeLibrary.As<IQueryable<AttributeEntity>>().Setup(_ => _.GetEnumerator()).Returns(attributeRepo.GetEnumerator());
            _mockedContext.Setup(_ => _.Attribute).Returns(attributeLibrary.Object);
            _mockedContext.Setup(_ => _.Set<AttributeEntity>()).Returns(attributeLibrary.Object);

            var simulationRepo = TestDataForCalculatedAttributesRepository.GetSimulations();
            var simulationLibrary = new Mock<DbSet<SimulationEntity>>();
            simulationLibrary.As<IQueryable<SimulationEntity>>().Setup(_ => _.Provider).Returns(simulationRepo.Provider);
            simulationLibrary.As<IQueryable<SimulationEntity>>().Setup(_ => _.Expression).Returns(simulationRepo.Expression);
            simulationLibrary.As<IQueryable<SimulationEntity>>().Setup(_ => _.ElementType).Returns(simulationRepo.ElementType);
            simulationLibrary.As<IQueryable<SimulationEntity>>().Setup(_ => _.GetEnumerator()).Returns(simulationRepo.GetEnumerator());
            _mockedContext.Setup(_ => _.Simulation).Returns(simulationLibrary.Object);
            _mockedContext.Setup(_ => _.Set<SimulationEntity>()).Returns(simulationLibrary.Object);

            var mockedRepo = new UnitOfDataPersistenceWork((new Mock<IConfiguration>()).Object, _mockedContext.Object);
            _testRepo = mockedRepo;

            _emptyMockedContext = new Mock<IAMContext>();

            libraryRepo = new List<CalculatedAttributeLibraryEntity>().AsQueryable();
            _mockLibrary = new Mock<DbSet<CalculatedAttributeLibraryEntity>>();
            _mockLibrary.As<IQueryable<CalculatedAttributeLibraryEntity>>().Setup(_ => _.Provider).Returns(libraryRepo.Provider);
            _mockLibrary.As<IQueryable<CalculatedAttributeLibraryEntity>>().Setup(_ => _.Expression).Returns(libraryRepo.Expression);
            _mockLibrary.As<IQueryable<CalculatedAttributeLibraryEntity>>().Setup(_ => _.ElementType).Returns(libraryRepo.ElementType);
            _mockLibrary.As<IQueryable<CalculatedAttributeLibraryEntity>>().Setup(_ => _.GetEnumerator()).Returns(libraryRepo.GetEnumerator());
            _emptyMockedContext.Setup(_ => _.CalculatedAttributeLibrary).Returns(_mockLibrary.Object);
            _emptyMockedContext.Setup(_ => _.Set<CalculatedAttributeLibraryEntity>()).Returns(_mockLibrary.Object);

            scenarioRepo = new List<ScenarioCalculatedAttributeEntity>().AsQueryable();
            _mockScenarioCalculations = new Mock<DbSet<ScenarioCalculatedAttributeEntity>>();
            _mockScenarioCalculations.As<IQueryable<ScenarioCalculatedAttributeEntity>>().Setup(_ => _.Provider).Returns(scenarioRepo.Provider);
            _mockScenarioCalculations.As<IQueryable<ScenarioCalculatedAttributeEntity>>().Setup(_ => _.Expression).Returns(scenarioRepo.Expression);
            _mockScenarioCalculations.As<IQueryable<ScenarioCalculatedAttributeEntity>>().Setup(_ => _.ElementType).Returns(scenarioRepo.ElementType);
            _mockScenarioCalculations.As<IQueryable<ScenarioCalculatedAttributeEntity>>().Setup(_ => _.GetEnumerator()).Returns(scenarioRepo.GetEnumerator());
            _emptyMockedContext.Setup(_ => _.ScenarioCalculatedAttribute).Returns(_mockScenarioCalculations.Object);
            _emptyMockedContext.Setup(_ => _.Set<ScenarioCalculatedAttributeEntity>()).Returns(_mockScenarioCalculations.Object);

            var emptyMockedRepo = new UnitOfDataPersistenceWork((new Mock<IConfiguration>()).Object, _emptyMockedContext.Object);
            _emptyTestRepo = emptyMockedRepo;

            _badId = new Guid("ddb82ba3-174f-43b0-97b2-4456b6b9edb2");
        }

        [Fact]
        public void SuccessfullyPullsDataFromLibraryRepository()
        {
            // Arrange
            var repo = new CalculatedAttributeRepository(_testRepo);

            // Act
            var result = repo.GetCalculatedAttributeLibraries();

            // Assert
            Assert.Equal(2, result.Count());
            Assert.NotNull(result.FirstOrDefault(_ => _.Name == "Second"));
        }

        [Fact]
        public void HandlesEmptyRepository()
        {
            // Arrange
            var repo = new CalculatedAttributeRepository(_emptyTestRepo);

            // Act
            var result = repo.GetCalculatedAttributeLibraries();

            // Assert
            Assert.Equal(0, result.Count());
        }

        [Fact]
        public void AddsLibraryToRepository()
        {
            // Arrange
            var repo = new CalculatedAttributeRepository(_testRepo);

            var attributes = TestDataForCalculatedAttributesRepository.GetAttributeRepo();

            var library = new CalculatedAttributeLibraryDTO
            {
                Name = "Third"
            };

            PopulateCalculatedAttributeLibraryDTO(library);

            // Act
            repo.UpsertCalculatedAttributeLibrary(library);

            // Assert
            _mockLibrary.Verify(_ => _.Add(It.IsAny<CalculatedAttributeLibraryEntity>()), Times.Once());
            _mockedContext.Verify(_ => _.SaveChanges(), Times.Once());
        }

        [Fact]
        public void UpdatesRepositoryWithExistingLibrary()
        {
            // TODO:  Ensure that calculated attribute changes are being reflected
            // Arrange
            var repo = new CalculatedAttributeRepository(_testRepo);

            var attributes = TestDataForCalculatedAttributesRepository.GetAttributeRepo();

            var library = new CalculatedAttributeLibraryDTO
            {
                Name = "First"
            };

            PopulateCalculatedAttributeLibraryDTO(library);

            // Act
            repo.UpsertCalculatedAttributeLibrary(library);

            // Assert
            _mockLibrary.Verify(_ => _.Update(It.IsAny<CalculatedAttributeLibraryEntity>()), Times.Once());
            _mockedContext.Verify(_ => _.SaveChanges(), Times.Once());
        }

        [Fact]
        public void SwitchesDefaultLibrary()
        {
            // Arrange
            var repo = new CalculatedAttributeRepository(_testRepo);

            var attributes = TestDataForCalculatedAttributesRepository.GetAttributeRepo();

            var library = new CalculatedAttributeLibraryDTO
            {
                Name = "Third",
                IsDefault = true
            };

            PopulateCalculatedAttributeLibraryDTO(library);

            // Act
            repo.UpsertCalculatedAttributeLibrary(library);

            // Assert
            _mockLibrary.Verify(_ => _.Update(It.IsAny<CalculatedAttributeLibraryEntity>()), Times.Exactly(2));
            _mockedContext.Verify(_ => _.SaveChanges(), Times.Once());
        }

        [Fact]
        public void AddsAttributeListToExistingLibrary()
        {
            // TODO:  Ensure existing attributes are preserved
            // Arrange
            var repo = new CalculatedAttributeRepository(_testRepo);
            const string newAttributeName = "IsNew";

            var attributeList = TestDataForCalculatedAttributesRepository.GetAttributeRepo().ToList();
            attributeList.Add(new AttributeEntity()
            {
                Id = Guid.NewGuid(),
                Name = newAttributeName
            });
            var attributes = attributeList.AsQueryable();

            var changingLibraryDTO = _testRepo.Context.CalculatedAttributeLibrary.First(_ => _.Name == "Second").ToDto();
            var newCalculationList = new List<CalculatedAttributeDTO>() { GetDefaultNewCalculation(attributes.First(_ => _.Name == newAttributeName)) };

            // Act
            repo.UpsertCalculatedAttributes(newCalculationList, changingLibraryDTO.Id);

            // Assert
            _mockLibrary.Verify(_ => _.Update(It.IsAny<CalculatedAttributeLibraryEntity>()), Times.Once());
            _mockedContext.Verify(_ => _.CalculatedAttribute.Add(It.IsAny<CalculatedAttributeEntity>()));
            _mockedContext.Verify(_ => _.SaveChanges(), Times.Once());
        }

        [Fact]
        public void UpdatesExistingCalculatedAttributeInLibrary()
        {
            // TODO:  Ensure existing, non-modified attributes are preserved
            // Arrange
            var repo = new CalculatedAttributeRepository(_testRepo);

            var attributes = TestDataForCalculatedAttributesRepository.GetAttributeRepo();

            var changingLibraryDTO = _testRepo.Context.CalculatedAttributeLibrary.First(_ => _.Name == "Second").ToDto();
            var revisedCalculation = changingLibraryDTO.CalculatedAttributes.First(_ => _.Attribute == "Description");
            revisedCalculation.CalculationTiming = 2;

            // Act
            repo.UpsertCalculatedAttributes(new List<CalculatedAttributeDTO>() { revisedCalculation }, changingLibraryDTO.Id);

            // Assert
            _mockLibrary.Verify(_ => _.Update(It.IsAny<CalculatedAttributeLibraryEntity>()), Times.Once());
            _mockedContext.Verify(_ => _.CalculatedAttribute.Update(It.IsAny<CalculatedAttributeEntity>()), Times.Once());
            _mockedContext.Verify(_ => _.SaveChanges(), Times.Once());
        }

        [Fact]
        public void UpsertHandlesNoLibraryFound()
        {
            // Arrange
            var repo = new CalculatedAttributeRepository(_testRepo);

            var attributes = TestDataForCalculatedAttributesRepository.GetAttributeRepo();

            var changingLibraryDTO = _testRepo.Context.CalculatedAttributeLibrary.First(_ => _.Name == "Second").ToDto();
            var revisedCalculation = changingLibraryDTO.CalculatedAttributes.First(_ => _.Attribute == "Description");
            revisedCalculation.CalculationTiming = 2;

            // Act & Assert
            Assert.Throws<RowNotInTableException>(() => repo.UpsertCalculatedAttributes(new List<CalculatedAttributeDTO>() { revisedCalculation }, _badId));
        }

        [Fact]
        public void SuccessfullyDeletesLibrary()
        {
            // Arrange
            var repo = new CalculatedAttributeRepository(_testRepo);
            var deletedLibraryEntityId = _testRepo.Context.CalculatedAttributeLibrary.First(_ => _.Name == "Second").Id;

            // Act
            repo.DeleteCalculatedAttributeLibrary(deletedLibraryEntityId);

            // Assert
            _mockLibrary.Verify(_ => _.Remove(It.IsAny<CalculatedAttributeLibraryEntity>()), Times.Once());
            _mockedContext.Verify(_ => _.SaveChanges(), Times.Once());
        }

        [Fact]
        public void DeleteLibraryHandlesNoLibraryFound()
        {
            // Arrange
            var repo = new CalculatedAttributeRepository(_testRepo);

            // Act
            repo.DeleteCalculatedAttributeLibrary(_badId);

            // Assert
            _mockLibrary.Verify(_ => _.Remove(It.IsAny<CalculatedAttributeLibraryEntity>()), Times.Never());
            _mockedContext.Verify(_ => _.SaveChanges(), Times.Never());
        }

        [Fact]
        public void SuccessfulyGetScenarioAttributes()
        {
            // Arrange
            var repo = new CalculatedAttributeRepository(_testRepo);
            var simulationId = TestDataForCalculatedAttributesRepository.GetSimulations().First(_ => _.Name == "First").Id;

            // Act
            var result = repo.GetScenarioCalculatedAttributes(simulationId);

            // Assert
            Assert.Equal(3, result.Count());
            Assert.NotNull(result.FirstOrDefault(_ => _.Attribute == "Age"));
        }

        [Fact]
        public void HandlesNoScenarioAttributes()
        {
            // Arrange
            var repo = new CalculatedAttributeRepository(_emptyTestRepo);
            var simulationId = TestDataForCalculatedAttributesRepository.GetSimulations().First(_ => _.Name == "First").Id;

            // Act
            var result = repo.GetScenarioCalculatedAttributes(simulationId);

            // Assert
            Assert.Equal(0, result.Count());
        }

        [Fact]
        public void AddsNewScenarioAttributes()
        {
            // TODO:  Ensure existing attributes are preserved
            // Arrange
            var repo = new CalculatedAttributeRepository(_testRepo);
            const string newAttributeName = "IsNew";
            var simulationId = TestDataForCalculatedAttributesRepository.GetSimulations().First(_ => _.Name == "First").Id;

            var attributeList = TestDataForCalculatedAttributesRepository.GetAttributeRepo().ToList();
            attributeList.Add(new AttributeEntity()
            {
                Id = Guid.NewGuid(),
                Name = newAttributeName
            });
            var attributes = attributeList.AsQueryable();

            var newCalculationList = new List<CalculatedAttributeDTO>() { GetDefaultNewCalculation(attributes.First(_ => _.Name == newAttributeName)) };

            // Act
            repo.UpsertScenarioCalculatedAttributes(newCalculationList, simulationId);

            // Assert
            _mockScenarioCalculations.Verify(_ => _.Add(It.IsAny<ScenarioCalculatedAttributeEntity>()), Times.Once());
            _mockedContext.Verify(_ => _.SaveChanges(), Times.Once());
        }

        [Fact]
        public void UpdateExistingScenarioAttributes()
        {
            // TODO:  Ensure existing, non-modified attributes are preserved
            // Arrange
            var repo = new CalculatedAttributeRepository(_testRepo);
            var simulationId = TestDataForCalculatedAttributesRepository.GetSimulations().First(_ => _.Name == "First").Id;
            var attributeToModify = _testRepo.Context.ScenarioCalculatedAttribute.First(_ => _.Attribute.Name == "Condition" && _.SimulationId == simulationId).ToDto();
            attributeToModify.CalculationTiming = 2;

            // Act
            repo.UpsertScenarioCalculatedAttributes(new List<CalculatedAttributeDTO>() { attributeToModify }, simulationId);

            // Assert
            _mockScenarioCalculations.Verify(_ => _.Update(It.IsAny<ScenarioCalculatedAttributeEntity>()), Times.Once());
            _mockedContext.Verify(_ => _.SaveChanges(), Times.Once());
        }

        [Fact]
        public void UpsertScenarioCalculatedAttributesHandlesNoScenarioFound()
        {
            // Arrange
            var repo = new CalculatedAttributeRepository(_testRepo);
            var attributeToModify = _testRepo.Context.ScenarioCalculatedAttribute.First(_ => _.Attribute.Name == "Condition").ToDto();
            attributeToModify.CalculationTiming = 2;

            // Act & Assert
            Assert.Throws<RowNotInTableException>(() => repo.UpsertScenarioCalculatedAttributes(new List<CalculatedAttributeDTO>() { attributeToModify }, _badId));
        }

        // Helpers
        private void PopulateCalculatedAttributeLibraryDTO(CalculatedAttributeLibraryDTO library)
        {
            var attributes = TestDataForCalculatedAttributesRepository.GetAttributeRepo();

            foreach (var attribute in attributes)
            {
                library.CalculatedAttributes.Add(GetDefaultNewCalculation(attribute));
            }
        }

        private CalculatedAttributeDTO GetDefaultNewCalculation(AttributeEntity attribute)
        {
            var newCalculation = new CalculatedAttributeDTO
            {
                Id = Guid.NewGuid(),
                Attribute = attribute.Name,
                CalculationTiming = 1
            };

            var newPair = new CalculatedAttributeEquationCriteriaPairDTO()
            {
                Id = Guid.NewGuid(),
                Equation = new EquationDTO()
                {
                    Expression = $"[{attribute.Name}] + 15"
                }
            };
            newCalculation.Equations.Add(newPair);

            newPair = new CalculatedAttributeEquationCriteriaPairDTO()
            {
                Id = Guid.NewGuid(),
                Equation = new EquationDTO()
                {
                    Expression = $"[{attribute.Name}] + 25"
                },
                CriteriaLibrary = new CriterionLibraryDTO()
                {
                    Name = "Test",
                    IsSingleUse = false,
                    MergedCriteriaExpression = "[Status] = 'Fair'"
                }
            };
            newCalculation.Equations.Add(newPair);

            return newCalculation;
        }
    }
}
