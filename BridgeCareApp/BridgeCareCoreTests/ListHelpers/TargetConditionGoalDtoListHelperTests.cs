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
    public class TargetConditionGoalDtoListHelperTests
    {
        [Fact]
        public void AddModifiedToTargetConditionGoal_Does()
        {
            var curve = new TargetConditionGoalDTO();
            var curves = new List<TargetConditionGoalDTO> { curve };
            Assert.False(curve.IsModified);

            TestHelper.UnitOfWork.TargetConditionGoalRepo.AddModifiedToScenarioTargetConditionGoal(curves, true);

            Assert.True(curve.IsModified);
        }

        [Fact]
        public void AddLibraryIdToTargetConditionGoal_Does()
        {
            var curve = new TargetConditionGoalDTO();
            var curves = new List<TargetConditionGoalDTO> { curve };
            var libraryId = Guid.NewGuid();

            TestHelper.UnitOfWork.TargetConditionGoalRepo.AddLibraryIdToScenarioTargetConditionGoal(curves, libraryId);

            Assert.Equal(libraryId, curve.LibraryId);
        }

        [Fact]
        public void AddLibraryIdToTargetConditionGoal_LibraryIdIsNull_NoChange()
        {
            var curve = new TargetConditionGoalDTO();
            var curves = new List<TargetConditionGoalDTO> { curve };
            var libraryId = Guid.NewGuid();
            curve.LibraryId = libraryId;

            TestHelper.UnitOfWork.TargetConditionGoalRepo.AddLibraryIdToScenarioTargetConditionGoal(curves, null);

            Assert.Equal(libraryId, curve.LibraryId);
        }
    }
}
