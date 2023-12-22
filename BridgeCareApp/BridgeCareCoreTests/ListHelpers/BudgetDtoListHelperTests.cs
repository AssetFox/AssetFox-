using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
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
            TestHelper.UnitOfWork.BudgetRepo.AddModifiedToScenarioBudget(curves, true);
            Assert.True(curve.IsModified);
        }


        [Fact]
        public void AddLibraryIdToBudget_Does()
        {
            var curve = new BudgetDTO();
            var curves = new List<BudgetDTO> { curve };
            var libraryId = Guid.NewGuid();

            TestHelper.UnitOfWork.BudgetRepo.AddLibraryIdToScenarioBudget(curves, libraryId);

            Assert.Equal(libraryId, curve.LibraryId);
        }

        [Fact]
        public void AddLibraryIdToBudget_LibraryIdIsNull_NoChange()
        {
            var curve = new BudgetDTO();
            var curves = new List<BudgetDTO> { curve };
            var libraryId = Guid.NewGuid();
            curve.LibraryId = libraryId;

            TestHelper.UnitOfWork.BudgetRepo.AddLibraryIdToScenarioBudget(curves, null);

            Assert.Equal(libraryId, curve.LibraryId);
        }
    }
}
