using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using Xunit;

namespace BridgeCareCoreTests.ListHelpers
{
    public class BudgetPriorityDtoListHelperTests
    {
        [Fact]
        public void AddModifiedToBudgetPriority_Does()
        {
            var curve = new BudgetPriorityDTO();
            var curves = new List<BudgetPriorityDTO> { curve };
            Assert.False(curve.IsModified);
            TestHelper.UnitOfWork.BudgetPriorityRepo.AddModifiedToScenarioBudgetPriority(curves, true);
            Assert.True(curve.IsModified);
        }


        [Fact]
        public void AddLibraryIdToBudgetPriority_Does()
        {
            var curve = new BudgetPriorityDTO();
            var curves = new List<BudgetPriorityDTO> { curve };
            var libraryId = Guid.NewGuid();

            TestHelper.UnitOfWork.BudgetPriorityRepo.AddLibraryIdToScenarioBudgetPriority(curves, libraryId);

            Assert.Equal(libraryId, curve.libraryId);
        }

        [Fact]
        public void AddLibraryIdToBudgetPriority_LibraryIdIsNull_NoChange()
        {
            var curve = new BudgetPriorityDTO();
            var curves = new List<BudgetPriorityDTO> { curve };
            var libraryId = Guid.NewGuid();
            curve.libraryId = libraryId;

            TestHelper.UnitOfWork.BudgetPriorityRepo.AddLibraryIdToScenarioBudgetPriority(curves, null);

            Assert.Equal(libraryId, curve.libraryId);
        }
    }
}
