using System;
using System.Linq;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using Xunit;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Criterion
{
    public class CriterionLibraryRepositoryTests
    {
        [Fact]
        public async Task UpsertCriterionLibrary_LibraryNotInDb_Adds()
        {
            var repo = TestHelper.UnitOfWork.CriterionLibraryRepo;
            var dto = CriterionLibraryTestSetup.TestCriterionLibrary();

            // Act
            var id = repo.UpsertCriterionLibrary(dto);

            // Assert
            Assert.Equal(id, dto.Id);
            var dtoAfter = await repo.CriteriaLibrary(id);
            ObjectAssertions.EquivalentExcluding(dto, dtoAfter, x => x.Owner);
        }

        [Fact]
        public void DeleteCriterionLibrary_LibraryNotInDb_DoesNotThrow()
        {
            TestHelper.UnitOfWork.CriterionLibraryRepo.DeleteCriterionLibrary(Guid.NewGuid());
        }

        [Fact]
        public void DeleteCriterionLibrary_LibraryInDb_Deletes()
        {
            // Arrange
            var criterionLibrary = CriterionLibraryTestSetup.TestCriterionLibraryInDb(TestHelper.UnitOfWork);
            Assert.True(TestHelper.UnitOfWork.Context.CriterionLibrary.Any(_ => _.Id == criterionLibrary.Id));

            // Act
            TestHelper.UnitOfWork.CriterionLibraryRepo.DeleteCriterionLibrary(criterionLibrary.Id);

            // Assert
            Assert.False(TestHelper.UnitOfWork.Context.CriterionLibrary.Any(_ => _.Id == criterionLibrary.Id));
        }

        [Fact]
        public async Task GetCriterionLibraries_LibraryInDb_Gets()
        {
            // Arrange
            var criterionLibrary = CriterionLibraryTestSetup.TestCriterionLibraryInDb(TestHelper.UnitOfWork, isSingleUse: false);

            // Act
            var dtos = await TestHelper.UnitOfWork.CriterionLibraryRepo.CriterionLibraries();

            // Assert
            Assert.Contains(dtos, cl => cl.Id == criterionLibrary.Id);
        }
    }
}
