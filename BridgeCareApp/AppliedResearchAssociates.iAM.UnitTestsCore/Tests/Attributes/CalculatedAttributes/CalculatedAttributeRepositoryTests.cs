using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Moq;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.CalculatedAttribute;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.CalculatedAttribute;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using System.Data;
using AppliedResearchAssociates.iAM.DTOs.Enums;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.User;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Attributes.CalculatedAttributes;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.CalculatedAttributes
{
    public class CalculatedAttributeRepositoryTests
    {
        // NOTE:  Because of the extension system, actions on the context itself cannot be mocked (pending a discussion with Paul)
        // As the definitions on these tests are still useful, they are commented out here in the hope that we can find a way
        // to make them work later.

        private UnitOfDataPersistenceWork _testRepo;
        private UnitOfDataPersistenceWork _emptyTestRepo;
        private UnitOfDataPersistenceWork _isCalcualtedTestRepo;
        private Mock<IAMContext> _mockedContext;
        private Mock<IAMContext> _emptyMockedContext;
        private Mock<DbSet<CalculatedAttributeLibraryEntity>> _mockLibrary;
        private Mock<DbSet<CalculatedAttributeEntity>> _mockLibrarycalcAttr;
        private Mock<DbSet<ScenarioCalculatedAttributeEntity>> _mockScenarioCalculations;
        private Mock<DbSet<AttributeEntity>> _mockAttributes;
        private Guid _badId;

        public CalculatedAttributeRepositoryTests()
        {
            // Create main test context
            _mockedContext = new Mock<IAMContext>();

            var libraryRepo = TestDataForCalculatedAttributesRepository.GetLibraryRepo();
            _mockLibrary = MockedContextBuilder.AddDataSet(_mockedContext, _ => _.CalculatedAttributeLibrary, libraryRepo);
            _mockLibrary.Setup(_ => _.Add(It.IsAny<CalculatedAttributeLibraryEntity>())).Returns<CalculatedAttributeDTO>(null);

            var libraryCalcAttrRepo = libraryRepo.SelectMany(_ => _.CalculatedAttributes);
            _mockLibrarycalcAttr = MockedContextBuilder.AddDataSet(_mockedContext, _ => _.CalculatedAttribute, libraryCalcAttrRepo);

            var scenarioRepo = TestDataForCalculatedAttributesRepository.GetSimulationCalculatedAttributesRepo();
            _mockScenarioCalculations = MockedContextBuilder.AddDataSet(_mockedContext, _ => _.ScenarioCalculatedAttribute, scenarioRepo);

            var attributeRepo = TestDataForCalculatedAttributesRepository.GetAttributeRepo();
            _mockAttributes = MockedContextBuilder.AddDataSet(_mockedContext, _ => _.Attribute, attributeRepo);

            var simulationRepo = TestDataForCalculatedAttributesRepository.GetSimulations();
            var simulationLibrary = MockedContextBuilder.AddDataSet(_mockedContext, _ => _.Simulation, simulationRepo);

            var mockedRepo = new UnitOfDataPersistenceWork((new Mock<IConfiguration>()).Object, _mockedContext.Object);
            _testRepo = mockedRepo;

            // Create empty test context
            _emptyMockedContext = new Mock<IAMContext>();

            libraryRepo = new List<CalculatedAttributeLibraryEntity>().AsQueryable();
            _mockLibrary = MockedContextBuilder.AddDataSet(_emptyMockedContext, _ => _.CalculatedAttributeLibrary, libraryRepo);

            scenarioRepo = new List<ScenarioCalculatedAttributeEntity>().AsQueryable();
            _mockScenarioCalculations = MockedContextBuilder.AddDataSet(_emptyMockedContext, _ => _.ScenarioCalculatedAttribute, scenarioRepo);

            var emptyMockedRepo = new UnitOfDataPersistenceWork((new Mock<IConfiguration>()).Object, _emptyMockedContext.Object);
            _emptyTestRepo = emptyMockedRepo;

            // Create calculated test context (using objects from the prior creation
            var isCalcultedContext = new Mock<IAMContext>();

            scenarioRepo = TestDataForCalculatedAttributesRepository.GetSimulationCalculatedAttributesRepo(false);
            var _mockScenarioLimitedCalculations = MockedContextBuilder.AddDataSet(isCalcultedContext, _ => _.ScenarioCalculatedAttribute, scenarioRepo);

            isCalcultedContext.Setup(_ => _.CalculatedAttributeLibrary).Returns(_mockLibrary.Object);
            isCalcultedContext.Setup(_ => _.Set<CalculatedAttributeLibraryEntity>()).Returns(_mockLibrary.Object);
            isCalcultedContext.Setup(_ => _.Attribute).Returns(_mockAttributes.Object);
            isCalcultedContext.Setup(_ => _.Set<AttributeEntity>()).Returns(_mockAttributes.Object);
            isCalcultedContext.Setup(_ => _.Simulation).Returns(simulationLibrary.Object);
            isCalcultedContext.Setup(_ => _.Set<SimulationEntity>()).Returns(simulationLibrary.Object);

            var mockedIsCalculatedRepo = new UnitOfDataPersistenceWork((new Mock<IConfiguration>()).Object, isCalcultedContext.Object);
            _isCalcualtedTestRepo = mockedIsCalculatedRepo;

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
            Assert.Empty(result);
        }

        //[Fact]
        //public void AddsLibraryToRepository()
        //{
        //    // Arrange
        //    var repo = new CalculatedAttributeRepository(_testRepo);

        //    var attributes = TestDataForCalculatedAttributesRepository.GetAttributeRepo();

        //    var library = new CalculatedAttributeLibraryDTO
        //    {
        //        Name = "Third"
        //    };

        //    PopulateCalculatedAttributeLibraryDTO(library);

        //    // Act
        //    repo.UpsertCalculatedAttributeLibrary(library);

        //    // Assert
        //    _mockLibrary.Verify(_ => _.Add(It.IsAny<CalculatedAttributeLibraryEntity>()), Times.Once());
        //    _mockedContext.Verify(_ => _.SaveChanges(), Times.Once());
        //}

        //[Fact]
        //public void UpdatesRepositoryWithExistingLibrary()
        //{
        //    // TODO:  Ensure that calculated attribute changes are being reflected
        //    // Arrange
        //    var repo = new CalculatedAttributeRepository(_testRepo);

        //    var attributes = TestDataForCalculatedAttributesRepository.GetAttributeRepo();

        //    var library = new CalculatedAttributeLibraryDTO
        //    {
        //        Name = "First"
        //    };

        //    PopulateCalculatedAttributeLibraryDTO(library);

        //    // Act
        //    repo.UpsertCalculatedAttributeLibrary(library);

        //    // Assert
        //    _mockLibrary.Verify(_ => _.Update(It.IsAny<CalculatedAttributeLibraryEntity>()), Times.Once());
        //    _mockedContext.Verify(_ => _.SaveChanges(), Times.Once());
        //}

        //[Fact]
        //public void SwitchesDefaultLibrary()
        //{
        //    // Arrange
        //    var repo = new CalculatedAttributeRepository(_testRepo);

        //    var attributes = TestDataForCalculatedAttributesRepository.GetAttributeRepo();

        //    var library = new CalculatedAttributeLibraryDTO
        //    {
        //        Name = "Third",
        //        IsDefault = true
        //    };

        //    PopulateCalculatedAttributeLibraryDTO(library);

        //    // Act
        //    repo.UpsertCalculatedAttributeLibrary(library);

        //    // Assert
        //    _mockLibrary.Verify(_ => _.Update(It.IsAny<CalculatedAttributeLibraryEntity>()), Times.Exactly(2));
        //    _mockedContext.Verify(_ => _.SaveChanges(), Times.Once());
        //}

        //[Fact]
        //public void AddsAttributeListToExistingLibrary()
        //{
        //    // TODO:  Ensure existing attributes are preserved
        //    // Arrange
        //    var repo = new CalculatedAttributeRepository(_testRepo);
        //    const string newAttributeName = "IsNew";

        //    var attributeList = TestDataForCalculatedAttributesRepository.GetAttributeRepo().ToList();
        //    attributeList.Add(new AttributeEntity()
        //    {
        //        Id = Guid.NewGuid(),
        //        Name = newAttributeName
        //    });
        //    var attributes = attributeList.AsQueryable();

        //    var changingLibraryDTO = _testRepo.Context.CalculatedAttributeLibrary.First(_ => _.Name == "Second").ToDto();
        //    var newCalculationList = new List<CalculatedAttributeDTO>() { GetDefaultNewCalculation(attributes.First(_ => _.Name == newAttributeName)) };

        //    // Act
        //    repo.UpsertCalculatedAttributes(newCalculationList, changingLibraryDTO.Id);

        //    // Assert
        //    _mockLibrary.Verify(_ => _.Update(It.IsAny<CalculatedAttributeLibraryEntity>()), Times.Once());
        //    _mockedContext.Verify(_ => _.CalculatedAttribute.Add(It.IsAny<CalculatedAttributeEntity>()));
        //    _mockedContext.Verify(_ => _.SaveChanges(), Times.Once());
        //}

        //[Fact]
        //public void UpdatesExistingCalculatedAttributeInLibrary()
        //{
        //    // TODO:  Ensure existing, non-modified attributes are preserved
        //    // Arrange
        //    var repo = new CalculatedAttributeRepository(_testRepo);

        //    var attributes = TestDataForCalculatedAttributesRepository.GetAttributeRepo();

        //    var changingLibraryDTO = _testRepo.Context.CalculatedAttributeLibrary.First(_ => _.Name == "Second").ToDto();
        //    var revisedCalculation = changingLibraryDTO.CalculatedAttributes.First(_ => _.Attribute == "Description");
        //    revisedCalculation.CalculationTiming = 2;

        //    // Act
        //    repo.UpsertCalculatedAttributes(new List<CalculatedAttributeDTO>() { revisedCalculation }, changingLibraryDTO.Id);

        //    // Assert
        //    _mockLibrary.Verify(_ => _.Update(It.IsAny<CalculatedAttributeLibraryEntity>()), Times.Once());
        //    _mockedContext.Verify(_ => _.CalculatedAttribute.Update(It.IsAny<CalculatedAttributeEntity>()), Times.Once());
        //    _mockedContext.Verify(_ => _.SaveChanges(), Times.Once());
        //}

        [Fact]
        public void UpsertHandlesNoLibraryFound()
        {
            // Arrange
            var repo = new CalculatedAttributeRepository(_testRepo);

            var attributes = TestDataForCalculatedAttributesRepository.GetAttributeRepo();

            var changingLibraryDTO = _testRepo.Context.CalculatedAttributeLibrary.First(_ => _.Name == "Second").ToDto();
            var revisedCalculation = changingLibraryDTO.CalculatedAttributes.FirstOrDefault(_ => _.Attribute == "DESCRIPTION");
            revisedCalculation.CalculationTiming = 2;

            // Act & Assert
            Assert.Throws<RowNotInTableException>(() => repo.UpsertCalculatedAttributes(new List<CalculatedAttributeDTO>() { revisedCalculation }, _badId));
        }

        //[Fact]
        //public void SuccessfullyDeletesLibrary()
        //{
        //    // Arrange
        //    var repo = new CalculatedAttributeRepository(_testRepo);
        //    var deletedLibraryEntityId = _testRepo.Context.CalculatedAttributeLibrary.First(_ => _.Name == "Second").Id;

        //    // Act
        //    repo.DeleteCalculatedAttributeLibrary(deletedLibraryEntityId);

        //    // Assert
        //    _mockLibrary.Verify(_ => _.Remove(It.IsAny<CalculatedAttributeLibraryEntity>()), Times.Once());
        //    _mockedContext.Verify(_ => _.SaveChanges(), Times.Once());
        //}

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
            var simulationId = TestDataForCalculatedAttributesRepository.FirstSimulationId;

            // Act
            var result = repo.GetScenarioCalculatedAttributes(simulationId);

            // Assert
            Assert.Equal(3, result.Count());
            Assert.NotNull(result.FirstOrDefault(_ => _.Attribute == "AGE"));
        }

        [Fact]
        public void HandlesNoScenarioAttributes()
        {
            // Arrange
            var repo = new CalculatedAttributeRepository(_emptyTestRepo);
            var simulationId = TestDataForCalculatedAttributesRepository.FirstSimulationId;

            // Act
            var result = repo.GetScenarioCalculatedAttributes(simulationId);

            // Assert
            Assert.Empty(result);
        }//

        //[Fact]
        //public void AddsNewScenarioAttributes()
        //{
        //    // TODO:  Ensure existing attributes are preserved
        //    // Arrange
        //    var repo = new CalculatedAttributeRepository(_testRepo);
        //    const string newAttributeName = "IsNew";
        //    var simulationId = TestDataForCalculatedAttributesRepository.GetSimulations().First(_ => _.Name == "First").Id;

        //    var attributeList = TestDataForCalculatedAttributesRepository.GetAttributeRepo().ToList();
        //    attributeList.Add(new AttributeEntity()
        //    {
        //        Id = Guid.NewGuid(),
        //        Name = newAttributeName
        //    });
        //    var attributes = attributeList.AsQueryable();

        //    var newCalculationList = new List<CalculatedAttributeDTO>() { GetDefaultNewCalculation(attributes.First(_ => _.Name == newAttributeName)) };

        //    // Act
        //    repo.UpsertScenarioCalculatedAttributes(newCalculationList, simulationId);

        //    // Assert
        //    _mockScenarioCalculations.Verify(_ => _.Add(It.IsAny<ScenarioCalculatedAttributeEntity>()), Times.Once());
        //    _mockedContext.Verify(_ => _.SaveChanges(), Times.Once());
        //}

        //[Fact]
        //public void UpdateExistingScenarioAttributes()
        //{
        //    // TODO:  Ensure existing, non-modified attributes are preserved
        //    // Arrange
        //    var repo = new CalculatedAttributeRepository(_testRepo);
        //    var simulationId = TestDataForCalculatedAttributesRepository.GetSimulations().First(_ => _.Name == "First").Id;
        //    var attributeToModify = _testRepo.Context.ScenarioCalculatedAttribute.First(_ => _.Attribute.Name == "Condition" && _.SimulationId == simulationId).ToDto();
        //    attributeToModify.CalculationTiming = 2;

        //    // Act
        //    repo.UpsertScenarioCalculatedAttributes(new List<CalculatedAttributeDTO>() { attributeToModify }, simulationId);

        //    // Assert
        //    _mockScenarioCalculations.Verify(_ => _.Update(It.IsAny<ScenarioCalculatedAttributeEntity>()), Times.Once());
        //    _mockedContext.Verify(_ => _.SaveChanges(), Times.Once());
        //}

        [Fact]
        public void UpsertScenarioCalculatedAttributesHandlesNoScenarioFound()
        {
            // Arrange
            var repo = new CalculatedAttributeRepository(_testRepo);
            var attributeToModify = _testRepo.Context.ScenarioCalculatedAttribute.First(_ => _.Attribute.Name == "CONDITION").ToDto();
            attributeToModify.CalculationTiming = 2;

            // Act & Assert
            Assert.Throws<RowNotInTableException>(() => repo.UpsertScenarioCalculatedAttributes(new List<CalculatedAttributeDTO>() { attributeToModify }, _badId));
        }

        [Fact]
        public void AttributeObjectPopulatedCorrectly()
        {
            // Arrange
            var attributeRepo = new AttributeRepository(_testRepo);
            var testExplorer = attributeRepo.GetExplorer();
            var simulation = testExplorer.AddNetwork().AddSimulation();
            SetupSimulation("First", simulation, _isCalcualtedTestRepo);

            var repo = new CalculatedAttributeRepository(_isCalcualtedTestRepo);

            // Act
            repo.PopulateScenarioCalculatedFields(simulation);

            // Assert
            Assert.Equal(1, testExplorer.CalculatedFields.Count);
            Assert.Equal(2, testExplorer.CalculatedFields.First().ValueSources.Count);
        }

        [Fact]
        public void CalculatedFieldPopulationHandlesCalculationsWithoutAttributes()
        {
            // Arrange
            var testAttributeRepo = TestDataForCalculatedAttributesRepository.GetAttributeRepo().ToList();
            var attributeToRemove = testAttributeRepo.First(_ => _.Name == "CONDITION");
            testAttributeRepo.Remove(attributeToRemove);
            var limitedAttributes = MockedContextBuilder.AddDataSet(_mockedContext, _ => _.Attribute, testAttributeRepo.AsQueryable());

            var attributeRepo = new AttributeRepository(_testRepo);
            var testExplorer = attributeRepo.GetExplorer();
            var simulation = testExplorer.AddNetwork().AddSimulation();
            SetupSimulation("First", simulation, _isCalcualtedTestRepo);

            var repo = new CalculatedAttributeRepository(_isCalcualtedTestRepo);

            // Act
            repo.PopulateScenarioCalculatedFields(simulation);

            // Assert
            Assert.Equal(0, testExplorer.CalculatedFields.Count);
        }

        [Fact]
        public void CalculatedFieldPopulationHandlesAttributesWithoutCalculation()
        {
            // Arrange
            var scenarioRepo = TestDataForCalculatedAttributesRepository.GetSimulationCalculatedAttributesRepo().ToList();
            var calculationsToRemove = scenarioRepo.Where(_ => _.Attribute.Name == "CONDITION").ToList();
            calculationsToRemove.ForEach(calc => scenarioRepo.Remove(calc));
            var limitedCalculations = MockedContextBuilder.AddDataSet(_mockedContext, _ => _.ScenarioCalculatedAttribute, scenarioRepo.AsQueryable());

            var attributeRepo = new AttributeRepository(_testRepo);
            var testExplorer = attributeRepo.GetExplorer();
            var simulation = testExplorer.AddNetwork().AddSimulation();
            SetupSimulation("First", simulation, _testRepo);

            var repo = new CalculatedAttributeRepository(_testRepo);

            // Act
            repo.PopulateScenarioCalculatedFields(simulation);

            // Assert
            Assert.Equal(1, testExplorer.CalculatedFields.Count);
            Assert.Equal(CalculatedFieldTiming.OnDemand, testExplorer.CalculatedFields.First().Timing);
        }

        [Fact]
        public void GetLibraryCalulatedAttributesByLibraryAndAttributeIdTest()
        {
            // Arrange
            var repo = new CalculatedAttributeRepository(_testRepo);
            var libraryId = _testRepo.Context.CalculatedAttributeLibrary.First(_ => _.Name == "First").Id;
            var attr = _mockAttributes.Object.First(_ => _.Name == "AGE");

            // Act
            var result = repo.GetLibraryCalulatedAttributesByLibraryAndAttributeId(libraryId, attr.Id);

            // Assert
            Assert.True(result.Attribute == "AGE");
        }

        [Fact]
        public void GetScenarioCalulatedAttributesByScenarioAndAttributeIdTest()
        {
            // Arrange
            var repo = new CalculatedAttributeRepository(_testRepo);
            var simulationId = TestDataForCalculatedAttributesRepository.FirstSimulationId;
            var attr = _testRepo.Context.Attribute.First(_ => _.Name == "AGE");
            
            // Act
            var result = repo.GetScenarioCalulatedAttributesByScenarioAndAttributeId(simulationId, attr.Id);

            // Assert
            Assert.True(result.Attribute == "AGE");
        }
        [Fact]
        public async Task UpdateCalculatedAttributeLibraryWithUserAccessChange_Does()
        {
            var user = await UserTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            var library = CalculatedAttributeLibraryTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            CalculatedAttributeLibraryUserTestSetup.SetUsersOfCalculatedAttributeLibrary(TestHelper.UnitOfWork, library.Id, LibraryAccessLevel.Modify, user.Id);
            var libraryUsersBefore = TestHelper.UnitOfWork.CalculatedAttributeRepo.GetLibraryUsers(library.Id);
            var libraryUserBefore = libraryUsersBefore.Single();
            Assert.Equal(LibraryAccessLevel.Modify, libraryUserBefore.AccessLevel);
            libraryUserBefore.AccessLevel = LibraryAccessLevel.Read;

            TestHelper.UnitOfWork.CalculatedAttributeRepo.UpsertOrDeleteUsers(library.Id, libraryUsersBefore);

            var libraryUsersAfter = TestHelper.UnitOfWork.CalculatedAttributeRepo.GetLibraryUsers(library.Id);
            var libraryUserAfter = libraryUsersAfter.Single();
            Assert.Equal(LibraryAccessLevel.Read, libraryUserAfter.AccessLevel);
        }
        [Fact]
        public async Task UpdateCalculatedAttributeLibraryUsers_RequestAccessRemoval_Does()
        {
            var user = await UserTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            var library = CalculatedAttributeLibraryTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            CalculatedAttributeLibraryUserTestSetup.SetUsersOfCalculatedAttributeLibrary(TestHelper.UnitOfWork, library.Id, LibraryAccessLevel.Modify, user.Id);
            var libraryUsersBefore = TestHelper.UnitOfWork.CalculatedAttributeRepo.GetLibraryUsers(library.Id);
            var libraryUserBefore = libraryUsersBefore.Single();
            libraryUsersBefore.Remove(libraryUserBefore);

            TestHelper.UnitOfWork.CalculatedAttributeRepo.UpsertOrDeleteUsers(library.Id, libraryUsersBefore);
            TestHelper.UnitOfWork.Context.SaveChanges();

            var libraryUsersAfter = TestHelper.UnitOfWork.CalculatedAttributeRepo.GetLibraryUsers(library.Id);
            Assert.Empty(libraryUsersAfter);
        }
        [Fact]
        public async Task UpdateLibraryUsers_AddAccessForUser_Does()
        {
            var user1 = await UserTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            var user2 = await UserTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            var library = CalculatedAttributeLibraryTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            CalculatedAttributeLibraryUserTestSetup.SetUsersOfCalculatedAttributeLibrary(TestHelper.UnitOfWork, library.Id, LibraryAccessLevel.Modify, user1.Id);
            var usersBefore = TestHelper.UnitOfWork.CalculatedAttributeRepo.GetLibraryUsers(library.Id);
            var newUser = new LibraryUserDTO
            {
                AccessLevel = LibraryAccessLevel.Read,
                UserId = user2.Id,
            };
            usersBefore.Add(newUser);

            TestHelper.UnitOfWork.CalculatedAttributeRepo.UpsertOrDeleteUsers(library.Id, usersBefore);

            var libraryUsersAfter = TestHelper.UnitOfWork.CalculatedAttributeRepo.GetLibraryUsers(library.Id);
            var user1After = libraryUsersAfter.Single(u => u.UserId == user1.Id);
            var user2After = libraryUsersAfter.Single(u => u.UserId == user2.Id);
            Assert.Equal(LibraryAccessLevel.Modify, user1After.AccessLevel);
            Assert.Equal(LibraryAccessLevel.Read, user2After.AccessLevel);
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

        private void SetupSimulation(string name, Simulation simulation, UnitOfDataPersistenceWork uow)
        {
            var populatedSimulation = uow.Context.Simulation.First(_ => _.Name == name);
            simulation.Id = populatedSimulation.Id;
            simulation.Name = populatedSimulation.Name;
        }
    }
}
