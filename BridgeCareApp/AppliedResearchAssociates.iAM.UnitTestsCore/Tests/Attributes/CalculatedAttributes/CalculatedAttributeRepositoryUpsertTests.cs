using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Repositories;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using Xunit;
using Xunit.Sdk;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Attributes.CalculatedAttributes
{
    public  class CalculatedAttributeRepositoryUpsertTests
    {

        private void Setup()
        {
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            NetworkTestSetup.CreateNetwork(TestHelper.UnitOfWork);
        }
        [Fact]
        public void UpdatesRepositoryWithExistingLibrary()
        {
            // Arrange
            Setup();

            Guid libraryId = Guid.NewGuid();

            var library = CalculatedAttributeTestSetup.TestCalculatedAttributeLibraryInDb(TestHelper.UnitOfWork, libraryId);

            Guid calcAttrId = Guid.NewGuid();
            var calcAttr = CalculatedAttributeTestSetup.TestCalculatedAttributeInLibraryInDb(TestHelper.UnitOfWork, library, calcAttrId, TestAttributeNames.ActionType);
            calcAttr.CalculationTiming = 1;
            library.CalculatedAttributes[0] = calcAttr;

            //Act
            TestHelper.UnitOfWork.CalculatedAttributeRepo.UpsertCalculatedAttributeLibrary(library);

            // Assert
            var actualLibrary = TestHelper.UnitOfWork.CalculatedAttributeRepo.GetCalculatedAttributeLibraryByID(libraryId);
            var actual = actualLibrary.CalculatedAttributes;
            Assert.Single(actual);
            Assert.Equal(1, actual.First().CalculationTiming);
        }

        [Fact]
        public void AddsAttributeListToExistingLibrary()
        {
            // Arrange
            Setup();

            Guid libraryId = Guid.NewGuid();

            var library = CalculatedAttributeTestSetup.TestCalculatedAttributeLibraryInDb(TestHelper.UnitOfWork, libraryId);

            Guid calcAttrId = Guid.NewGuid();
            var calcAttr = CalculatedAttributeTestSetup.TestCalculatedAttributeInLibraryInDb(TestHelper.UnitOfWork, library, calcAttrId, TestAttributeNames.ActionType);

            Guid newCalcAttrId = Guid.NewGuid();
            var newCalcAttr = CalculatedAttributeTestSetup.TestCalculatedAttributeDto(newCalcAttrId, TestAttributeNames.Age);

            library.CalculatedAttributes.Add(newCalcAttr);

            //Act
            TestHelper.UnitOfWork.CalculatedAttributeRepo.UpsertCalculatedAttributeLibrary(library);

            // Assert
            var actual = TestHelper.UnitOfWork.CalculatedAttributeRepo.GetCalculatedAttributeLibraryByID(libraryId);

            var actualIds = actual.CalculatedAttributes.Select(_ => _.Id).ToList();
            Assert.Equal(actual.Id, library.Id);
            Assert.Equal(2, actual.CalculatedAttributes.Count);
            Assert.Contains(calcAttrId, actualIds);
            Assert.Contains(newCalcAttrId, actualIds);
        }

        [Fact]
        public void AddsNewScenarioAttributes()
        {
            // TODO:  Ensure existing attributes are preserved
            // Arrange
            Setup();

            Guid scenarioId = Guid.NewGuid();
            var scenario = SimulationTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork, scenarioId);

            Guid calcAttrId = Guid.NewGuid();
            var calcAttr = CalculatedAttributeTestSetup.TestCalculatedAttributeInScenarioInDb(TestHelper.UnitOfWork, scenarioId, calcAttrId, TestAttributeNames.ActionType);

            Guid newCalcAttrId = Guid.NewGuid();
            var newCalcAttr = CalculatedAttributeTestSetup.TestCalculatedAttributeDto(newCalcAttrId, TestAttributeNames.Age);

            var scenarioCalcAttrs = new List<CalculatedAttributeDTO>() { calcAttr, newCalcAttr };

            //Act
            TestHelper.UnitOfWork.CalculatedAttributeRepo.UpsertScenarioCalculatedAttributesNonAtomic(scenarioCalcAttrs, scenarioId);

            // Assert
            var actual = TestHelper.UnitOfWork.CalculatedAttributeRepo.GetScenarioCalculatedAttributes(scenarioId);

            var actualIds = actual.Select(_ => _.Id).ToList();
            Assert.Equal(2, actual.Count);
            Assert.Contains(calcAttrId, actualIds);
            Assert.Contains(newCalcAttrId, actualIds);
        }

        [Fact]
        public void UpdateExistingScenarioAttributes()
        {
            // TODO:  Ensure existing, non-modified attributes are preserved
            // Arrange
            Setup();

            Guid scenarioId = Guid.NewGuid();

            var scenario = SimulationTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork, scenarioId);

            Guid calcAttrId = Guid.NewGuid();
            var calcAttr = CalculatedAttributeTestSetup.TestCalculatedAttributeInScenarioInDb(TestHelper.UnitOfWork, scenarioId, calcAttrId, TestAttributeNames.ActionType);
            calcAttr.CalculationTiming = 1;

            Guid calcAttr2Id = Guid.NewGuid();
            var calcAttr2 = CalculatedAttributeTestSetup.TestCalculatedAttributeInScenarioInDb(TestHelper.UnitOfWork, scenarioId, calcAttr2Id, TestAttributeNames.AdtTotal);

            var scenarioCalcAttrs = new List<CalculatedAttributeDTO>() { calcAttr, calcAttr2 };

            //Act
            TestHelper.UnitOfWork.CalculatedAttributeRepo.UpsertScenarioCalculatedAttributesNonAtomic(scenarioCalcAttrs, scenarioId);

            // Assert
            var actual = TestHelper.UnitOfWork.CalculatedAttributeRepo.GetScenarioCalculatedAttributes(scenarioId);

            Assert.Equal(2, actual.Count);
            Assert.Equal(1, actual.First(_ => _.Id == calcAttrId).CalculationTiming);
            Assert.Equal(0, actual.First(_ => _.Id == calcAttr2Id).CalculationTiming);
        }

        [Fact]
        public void UpdatesExistingCalculatedAttributeInLibrary()
        {
            //// TODO:  Ensure existing, non-modified attributes are preserved
            //// Arrange
            Setup();

            Guid libraryId = Guid.NewGuid();

            var library = CalculatedAttributeTestSetup.TestCalculatedAttributeLibraryInDb(TestHelper.UnitOfWork, libraryId);

            Guid calcAttrId = Guid.NewGuid();
            var calcAttr = CalculatedAttributeTestSetup.TestCalculatedAttributeInLibraryInDb(TestHelper.UnitOfWork, library, calcAttrId, TestAttributeNames.ActionType);
            calcAttr.CalculationTiming = 1;
            library.CalculatedAttributes[0] = calcAttr;

            Guid calcAttr2Id = Guid.NewGuid();
            var calcAttr2 = CalculatedAttributeTestSetup.TestCalculatedAttributeInLibraryInDb(TestHelper.UnitOfWork, library, calcAttr2Id, TestAttributeNames.AdtTotal);

            //Act
            TestHelper.UnitOfWork.CalculatedAttributeRepo.UpsertCalculatedAttributeLibrary(library);

            // Assert
            var actual = TestHelper.UnitOfWork.CalculatedAttributeRepo.GetCalculatedAttributeLibraryByID(libraryId);

            Assert.Equal(actual.Id, library.Id);
            Assert.Equal(2, actual.CalculatedAttributes.Count);
            Assert.Equal(1, actual.CalculatedAttributes.First(_ => _.Id == calcAttrId).CalculationTiming);
            Assert.Equal(0, actual.CalculatedAttributes.First(_ => _.Id == calcAttr2Id).CalculationTiming);
        }
    }
}
