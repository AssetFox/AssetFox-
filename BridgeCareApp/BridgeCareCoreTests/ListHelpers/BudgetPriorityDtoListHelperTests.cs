using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using BridgeCareCore.Services;
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
            BudgetPriorityDtoListHelper.AddModifiedToScenarioBudgetPriority(curves, true);
            Assert.True(curve.IsModified);
        }


        [Fact]
        public void AddLibraryIdToBudgetPriority_Does()
        {
            var curve = new BudgetPriorityDTO();
            var curves = new List<BudgetPriorityDTO> { curve };
            var libraryId = Guid.NewGuid();

            BudgetPriorityDtoListHelper.AddLibraryIdToScenarioBudgetPriority(curves, libraryId);

            Assert.Equal(libraryId, curve.libraryId);
        }

        [Fact]
        public void AddLibraryIdToBudgetPriority_LibraryIdIsNull_NoChange()
        {
            var curve = new BudgetPriorityDTO();
            var curves = new List<BudgetPriorityDTO> { curve };
            var libraryId = Guid.NewGuid();
            curve.libraryId = libraryId;

            BudgetPriorityDtoListHelper.AddLibraryIdToScenarioBudgetPriority(curves, null);

            Assert.Equal(libraryId, curve.libraryId);
        }
    }
}
