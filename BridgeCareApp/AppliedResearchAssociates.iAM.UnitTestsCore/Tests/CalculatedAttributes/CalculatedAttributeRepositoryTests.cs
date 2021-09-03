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

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.CalculatedAttributes
{
    public class CalculatedAttributeRepositoryTests
    {
        private UnitOfDataPersistenceWork _testRepo;
        private Mock<IAMContext> _mockedContext;
        private Mock<DbSet<CalculatedAttributeLibraryEntity>> _mockLibrary;
        private Mock<DbSet<ScenarioCalculatedAttributeEntity>> _mockScenarioCalculations;

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
            
            var mockedRepo = new UnitOfDataPersistenceWork((new Mock<IConfiguration>()).Object, _mockedContext.Object);
            _testRepo = mockedRepo;
        }

        [Fact]
        public void SuccessfullyPullsDataFromRepository()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void HandlesEmptyRepository()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void AddsLibraryToRepository()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void UpdatesRepositoryWithExistingLibrary()
        {
            // TODO:  Ensure that calculated attribute changes are being reflected
            throw new NotImplementedException();
        }

        [Fact]
        public void AddsAttributeListToExistingLibrary()
        {
            // TODO:  Ensure existing attributes are preserved
            throw new NotImplementedException();
        }

        [Fact]
        public void UpdatesExistingCalculatedAttributeInLibrary()
        {
            // TODO:  Ensure existing, non-modified attributes are preserved
            throw new NotImplementedException();
        }

        [Fact]
        public void RemovesCaclulatedAttributesInLibrary()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void UpsertHandlesNoLibraryFound()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void DeleteCalculatedAttributeHandlesNoLibraryFound()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void SuccessfullyDeletesLibrary()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void DeleteLibraryHandlesNoLibraryFound()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void SuccessfulyGetScenarioAttributes()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void HandlesNoScenarioAttributes()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void AddsNewScenarioAttributes()
        {
            // TODO:  Ensure existing attributes are preserved
            throw new NotImplementedException();
        }

        [Fact]
        public void UpdateExistingScenarioAttributes()
        {
            // TODO:  Ensure existing, non-modified attributes are preserved
            throw new NotImplementedException();
        }

        [Fact]
        public void UpsertScenarioCalculatedAttributesHandlesNoScenarioFound()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void SuccessfullyDeleteScenarioCalculatedAttribute()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void DeleteScenarioCalculatedAttributeHandlesAttributeNotFound()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void DeleteScenarioCalculatedAttributeHandlesScenarioNotFound()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void SuccessfullyClearsCalculatedAttributesFromScenario()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void ClearHandlesScenarioNotFound()
        {
            throw new NotImplementedException();
        }
    }
}
