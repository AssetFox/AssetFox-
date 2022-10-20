using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public class CriterionLibraryRepositoryTests
    {
        [Fact]
        public void UpsertCriterionLibrary_NotInDb_Adds()
        {
            // Arrange
            var criterionLibrary = CriterionLibraryTestSetup.TestCriterionLibraryInDb(TestHelper.UnitOfWork);
            var newName = RandomStrings.WithPrefix("New Name");
            var newDescription = RandomStrings.WithPrefix("Updated description");
            var newCriterionLibraryDTO = new CriterionLibraryDTO
            {
                Id = criterionLibrary.Id,
                Name = newName,
                MergedCriteriaExpression = "New Expression",
                Description = newDescription,
            };

            // Act
            var addResult = TestHelper.UnitOfWork.CriterionLibraryRepo.UpsertCriterionLibrary(newCriterionLibraryDTO);

            var newCriterionLibraryEntity =
                TestHelper.UnitOfWork.Context.CriterionLibrary.Single(_ =>
                    _.Id == newCriterionLibraryDTO.Id);
        }

        [Fact]
        public void UpsertCriterionLibrary_AlreadyInDb_Modifies()
        {
            var criterionLibrary = CriterionLibraryTestSetup.TestCriterionLibraryInDb(TestHelper.UnitOfWork);
            criterionLibrary.Description = "Updated Description";
            var criterionLibraryEntityBefore = TestHelper.UnitOfWork.Context.CriterionLibrary
                .Single(_ => _.Id == criterionLibrary.Id);
            Assert.NotNull(criterionLibraryEntityBefore);

            var updateResult = TestHelper.UnitOfWork.CriterionLibraryRepo.UpsertCriterionLibrary(criterionLibrary);

            var updatedCriterionLibraryEntity = TestHelper.UnitOfWork.Context.CriterionLibrary
                .Single(_ => _.Id == criterionLibrary.Id);
            Assert.Equal("Updated Description", updatedCriterionLibraryEntity.Description);
        }
    }
}
