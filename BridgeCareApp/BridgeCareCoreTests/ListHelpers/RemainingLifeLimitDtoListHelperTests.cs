using System;
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
    public class RemainingLifeLimitDtoListHelperTests
    {
        [Fact]
        public void AddModifiedToRemainingLifeLimit_Does()
        {
            var curve = new RemainingLifeLimitDTO();
            var curves = new List<RemainingLifeLimitDTO> { curve };
            Assert.False(curve.IsModified);
            TestHelper.UnitOfWork.RemainingLifeLimitRepo.AddModifiedToScenarioRemainingLifeLimit(curves, true);
            Assert.True(curve.IsModified);
        }


        [Fact]
        public void AddLibraryIdToRemainingLifeLimit_Does()
        {
            var curve = new RemainingLifeLimitDTO();
            var curves = new List<RemainingLifeLimitDTO> { curve };
            var libraryId = Guid.NewGuid();

            TestHelper.UnitOfWork.RemainingLifeLimitRepo.AddLibraryIdToScenarioRemainingLifeLimit(curves, libraryId);

            Assert.Equal(libraryId, curve.LibraryId);
        }

        [Fact]
        public void AddLibraryIdToRemainingLifeLimit_LibraryIdIsNull_NoChange()
        {
            var curve = new RemainingLifeLimitDTO();
            var curves = new List<RemainingLifeLimitDTO> { curve };
            var libraryId = Guid.NewGuid();
            curve.LibraryId = libraryId;

            TestHelper.UnitOfWork.RemainingLifeLimitRepo.AddLibraryIdToScenarioRemainingLifeLimit(curves, null);

            Assert.Equal(libraryId, curve.LibraryId);
        }
    }
}
