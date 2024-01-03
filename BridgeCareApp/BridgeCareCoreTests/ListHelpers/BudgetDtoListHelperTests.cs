using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Services;
using Xunit;

namespace BridgeCareCoreTests.ListHelpers
{
    public class BudgetDtoListHelperTests
    {
        [Fact]
        public void AddModifiedToBudget_Does()
        {
            var curve = new BudgetDTO();
            var curves = new List<BudgetDTO> { curve };
            Assert.False(curve.IsModified);
            BudgetDtoListHelper.AddModifiedToScenarioBudget(curves, true);
            Assert.True(curve.IsModified);
        }


        [Fact]
        public void AddLibraryIdToBudget_Does()
        {
            var curve = new BudgetDTO();
            var curves = new List<BudgetDTO> { curve };
            var libraryId = Guid.NewGuid();

            BudgetDtoListHelper.AddLibraryIdToScenarioBudget(curves, libraryId);

            Assert.Equal(libraryId, curve.LibraryId);
        }

        [Fact]
        public void AddLibraryIdToBudget_LibraryIdIsNull_NoChange()
        {
            var curve = new BudgetDTO();
            var curves = new List<BudgetDTO> { curve };
            var libraryId = Guid.NewGuid();
            curve.LibraryId = libraryId;

            BudgetDtoListHelper.AddLibraryIdToScenarioBudget(curves, null);

            Assert.Equal(libraryId, curve.LibraryId);
        }
    }
}
