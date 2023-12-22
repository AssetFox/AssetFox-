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
    public class DeficientConditionGoalDtoListHelperTests
    {
        [Fact]
        public void AddModifiedToDeficientConditionGoal_Does()
        {
            var curve = new DeficientConditionGoalDTO();
            var curves = new List<DeficientConditionGoalDTO> { curve };
            Assert.False(curve.IsModified);
            TestHelper.UnitOfWork.DeficientConditionGoalRepo.AddModifiedToScenarioDeficientConditionGoal(curves, true);
            Assert.True(curve.IsModified);
        }


        [Fact]
        public void AddLibraryIdToDeficientConditionGoal_Does()
        {
            var curve = new DeficientConditionGoalDTO();
            var curves = new List<DeficientConditionGoalDTO> { curve };
            var libraryId = Guid.NewGuid();

            TestHelper.UnitOfWork.DeficientConditionGoalRepo.AddLibraryIdToScenarioDeficientConditionGoal(curves, libraryId);

            Assert.Equal(libraryId, curve.LibraryId);
        }

        [Fact]
        public void AddLibraryIdToDeficientConditionGoal_LibraryIdIsNull_NoChange()
        {
            var curve = new DeficientConditionGoalDTO();
            var curves = new List<DeficientConditionGoalDTO> { curve };
            var libraryId = Guid.NewGuid();
            curve.LibraryId = libraryId;

            TestHelper.UnitOfWork.DeficientConditionGoalRepo.AddLibraryIdToScenarioDeficientConditionGoal(curves, null);

            Assert.Equal(libraryId, curve.LibraryId);
        }

    }
}
