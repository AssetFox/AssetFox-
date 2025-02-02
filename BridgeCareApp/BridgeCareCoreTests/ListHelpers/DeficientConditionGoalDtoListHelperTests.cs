﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using BridgeCareCore.Services;
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
            DeficientConditionGoalDtoListService.AddModifiedToScenarioDeficientConditionGoal(curves, true);
            Assert.True(curve.IsModified);
        }


        [Fact]
        public void AddLibraryIdToDeficientConditionGoal_Does()
        {
            var curve = new DeficientConditionGoalDTO();
            var curves = new List<DeficientConditionGoalDTO> { curve };
            var libraryId = Guid.NewGuid();

            DeficientConditionGoalDtoListService.AddLibraryIdToScenarioDeficientConditionGoal(curves, libraryId);

            Assert.Equal(libraryId, curve.LibraryId);
        }

        [Fact]
        public void AddLibraryIdToDeficientConditionGoal_LibraryIdIsNull_NoChange()
        {
            var curve = new DeficientConditionGoalDTO();
            var curves = new List<DeficientConditionGoalDTO> { curve };
            var libraryId = Guid.NewGuid();
            curve.LibraryId = libraryId;

            DeficientConditionGoalDtoListService.AddLibraryIdToScenarioDeficientConditionGoal(curves, null);

            Assert.Equal(libraryId, curve.LibraryId);
        }

    }
}
