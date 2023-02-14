﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.RemainingLifeLimit;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.RemainingLifeLimit;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.RemainingLifeLimit;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Repositories;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public class RemainingLifeLimitRepositoryTests
    {

        private void Setup()
        {
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            NetworkTestSetup.CreateNetwork(TestHelper.UnitOfWork);
        }

        public RemainingLifeLimitLibraryEntity TestRemainingLifeLimitLibrary(Guid? id = null)
        {
            var resolveId = id ?? Guid.NewGuid();
            var returnValue = new RemainingLifeLimitLibraryEntity
            {
                Id = resolveId,
                Name = "Test Name"
            };
            return returnValue;
        }

        public RemainingLifeLimitEntity TestRemainingLifeLimit(Guid libraryId, Guid attributeId)
        {
            return new RemainingLifeLimitEntity
            {
                Id = Guid.NewGuid(),
                RemainingLifeLimitLibraryId = libraryId,
                Value = 1.0,
                AttributeId = attributeId,
            };
        }

        public ScenarioRemainingLifeLimitEntity ScenarioTestRemainingLifeLimit(Guid scenarioId, Guid attributeId)
        {
            return new ScenarioRemainingLifeLimitEntity
            {
                Id = Guid.NewGuid(),
                SimulationId = scenarioId,
                Value = 1.0,
                AttributeId = attributeId,
            };
        }

        private RemainingLifeLimitLibraryEntity SetupForGet()
        {
            var library = TestRemainingLifeLimitLibrary();
            var attribute = TestHelper.UnitOfWork.Context.Attribute.First();
            var lifeLimit = TestRemainingLifeLimit(library.Id, attribute.Id);
            TestHelper.UnitOfWork.Context.RemainingLifeLimitLibrary.Add(library);
            TestHelper.UnitOfWork.Context.RemainingLifeLimit.Add(lifeLimit);
            TestHelper.UnitOfWork.Context.SaveChanges();
            return library;
        }

        private ScenarioRemainingLifeLimitEntity SetupForScenarioGet(Guid scenarioId)
        {
            var attribute = TestHelper.UnitOfWork.Context.Attribute.First();
            var lifeLimit = ScenarioTestRemainingLifeLimit(scenarioId, attribute.Id);
            TestHelper.UnitOfWork.Context.ScenarioRemainingLifeLimit.Add(lifeLimit);
            TestHelper.UnitOfWork.Context.SaveChanges();
            return lifeLimit;
        }

        [Fact]
        public void GetRemainingLifeLimitLibrariesNoChildren_DoesNotThrow()
        {
            Setup();
            var library = SetupForGet();

            // Act
            var result = TestHelper.UnitOfWork.RemainingLifeLimitRepo.GetAllRemainingLifeLimitLibrariesNoChildren();
            var libraryAfter = result.Single(x => x.Id == library.Id);
            Assert.Empty(libraryAfter.RemainingLifeLimits);
        }

        [Fact]
        public void UpsertRemainingLifeLimitLibrary_Does()
        {
            Setup();
            var dto = RemainingLifeLimitLibraryDtos.Empty();
            // Act
            TestHelper.UnitOfWork.RemainingLifeLimitRepo.UpsertRemainingLifeLimitLibrary(dto);

            // Assert
            var dtoAfter = TestHelper.UnitOfWork.RemainingLifeLimitRepo.GetRemainingLifeLimitsByLibraryId(dto.Id);
        }

        [Fact]
        public void GetRemainingLifeLimitsByLibraryId_LibraryInDbWithRemainingLifeLimit_Gets()
        {
            Setup();
            // Arrange
            var library = SetupForGet().ToDto();
            var limit = library.RemainingLifeLimits[0];

            // Act
            var dtos = TestHelper.UnitOfWork.RemainingLifeLimitRepo.GetRemainingLifeLimitsByLibraryId(library.Id);

            // Assert
            var dto = dtos.Single();
            Assert.Equal(limit.Id, dto.Id);
        }

        [Fact]
        public void GetRemainingLifeLimitsByLibraryId_LibraryNotInDb_Throws()
        {
            Setup();
            var libraryId = Guid.NewGuid();

            var exception = Assert.Throws<RowNotInTableException>(() => TestHelper.UnitOfWork.RemainingLifeLimitRepo.GetRemainingLifeLimitsByLibraryId(libraryId));
        }

        [Fact]
        public void Delete_Deletes()
        {
            Setup();

            var library = SetupForGet();

            // Act
            TestHelper.UnitOfWork.RemainingLifeLimitRepo.DeleteRemainingLifeLimitLibrary(library.Id);

            var libraryAfter = TestHelper.UnitOfWork.Context.RemainingLifeLimitLibrary.SingleOrDefault(l => l.Id == library.Id);
            Assert.Null(libraryAfter);
        }

        [Fact]
        public void UpsertOrDeleteRemainingLifeLimits_LimitInDb_Modifies()
        {
            // Arrange
            Setup();
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork);
            var lifeLimitLibrary = SetupForGet();
            var criterionLibrary = CriterionLibraryTestSetup.TestCriterionLibrary();
            var dtos = TestHelper.UnitOfWork.RemainingLifeLimitRepo.GetAllRemainingLifeLimitLibrariesWithRemainingLifeLimits();

            var dto = dtos.Single(x => x.Id == lifeLimitLibrary.Id);
            dto.Description = "Updated Description";
            dto.RemainingLifeLimits[0].Value = 2.0;
            dto.RemainingLifeLimits[0].CriterionLibrary =
                criterionLibrary;
            // Act
            TestHelper.UnitOfWork.RemainingLifeLimitRepo.UpsertOrDeleteRemainingLifeLimits(dto.RemainingLifeLimits, lifeLimitLibrary.Id);

            // Assert
            var modifiedDto = TestHelper.UnitOfWork.RemainingLifeLimitRepo
                .GetAllRemainingLifeLimitLibrariesWithRemainingLifeLimits().Single(rll => rll.Id == lifeLimitLibrary.Id);

            Assert.Equal(dto.RemainingLifeLimits[0].Attribute, modifiedDto.RemainingLifeLimits[0].Attribute);
        }

        [Fact]
        public void ShouldModifyScenarioRemainingLifeLimitData()
        {
            // Arrange
            Setup();
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork);
            var limitEntity = SetupForScenarioGet(simulation.Id);
            var criterionLibrary = CriterionLibraryTestSetup.TestCriterionLibrary();

            var dto = limitEntity.ToDto();
            dto.Value = 2.0;
            dto.CriterionLibrary =
                criterionLibrary;
            var dtos = new List<RemainingLifeLimitDTO> { dto };

            // Act
            TestHelper.UnitOfWork.RemainingLifeLimitRepo.UpsertOrDeleteScenarioRemainingLifeLimits(dtos, simulation.Id);

            // Assert
            var modifiedDto = TestHelper.UnitOfWork.RemainingLifeLimitRepo
                .GetScenarioRemainingLifeLimits(simulation.Id).Single(rll => rll.Id == dto.Id);

            Assert.Equal(2.0, modifiedDto.Value);
        }


        [Fact]
        public void GetScenarioRemainingLifeLimits_ScenarioInDbWithRemainingLifeLimit_Gets()
        {
            Setup();
            // Arrange
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork);
            var limit = SetupForScenarioGet(simulation.Id).ToDto();

            // Act
            var dtos = TestHelper.UnitOfWork.RemainingLifeLimitRepo.GetScenarioRemainingLifeLimits(simulation.Id);

            // Assert
            var dto = dtos.Single();
            Assert.Equal(limit.Id, dto.Id);
        }

        [Fact]
        public void DeleteRemainingLifeLimitLibrary_LibraryInDbWithRemainingLifeLimitAndCriterionLibraryLink_DeletesAll()
        {
            // Arrange
            Setup();
            var library = SetupForGet();
            var criterionLibrary = CriterionLibraryTestSetup.TestCriterionLibrary();
            var dtos = TestHelper.UnitOfWork.RemainingLifeLimitRepo.GetAllRemainingLifeLimitLibrariesWithRemainingLifeLimits();

            var remainingLifeLimitLibraryDTO = dtos.Single(lib => lib.Id == library.Id);
            var remainingLifeLimitDto = remainingLifeLimitLibraryDTO.RemainingLifeLimits[0];
            remainingLifeLimitDto.CriterionLibrary =
                criterionLibrary;
            var dtosForUpsert = new List<RemainingLifeLimitDTO> { remainingLifeLimitDto };

            TestHelper.UnitOfWork.RemainingLifeLimitRepo.UpsertRemainingLifeLimitLibrary(remainingLifeLimitLibraryDTO);
            TestHelper.UnitOfWork.RemainingLifeLimitRepo.UpsertOrDeleteRemainingLifeLimits(dtosForUpsert, library.Id);
            Assert.True(TestHelper.UnitOfWork.Context.RemainingLifeLimitLibrary.Any(_ => _.Id == library.Id));
            Assert.True(TestHelper.UnitOfWork.Context.RemainingLifeLimit.Any(_ => _.RemainingLifeLimitLibraryId == library.Id));
            Assert.True(TestHelper.UnitOfWork.Context.CriterionLibraryRemainingLifeLimit.Any(_ => _.RemainingLifeLimitId == remainingLifeLimitDto.Id));

            // Act
            TestHelper.UnitOfWork.RemainingLifeLimitRepo.DeleteRemainingLifeLimitLibrary(library.Id);

            // Assert
            Assert.False(TestHelper.UnitOfWork.Context.RemainingLifeLimitLibrary.Any(_ => _.Id == library.Id));
            Assert.False(TestHelper.UnitOfWork.Context.RemainingLifeLimit.Any(_ => _.RemainingLifeLimitLibraryId == library.Id));
            Assert.False(TestHelper.UnitOfWork.Context.CriterionLibraryRemainingLifeLimit.Any(_ => _.RemainingLifeLimitId == remainingLifeLimitDto.Id));
        }
    }
}