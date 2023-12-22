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
    public class TreatmentDtoListHelperTests
    {
        [Fact]
        public void AddModifiedToTreatment_Does()
        {
            var curve = new TreatmentDTO();
            var curves = new List<TreatmentDTO> { curve };
            Assert.False(curve.IsModified);
            TreatmentDtoListHelper.AddModifiedToScenarioSelectableTreatments(curves, true);
            Assert.True(curve.IsModified);
        }


        [Fact]
        public void AddLibraryIdToTreatment_Does()
        {
            var curve = new TreatmentDTO();
            var curves = new List<TreatmentDTO> { curve };
            var libraryId = Guid.NewGuid();

            TreatmentDtoListHelper.AddLibraryIdToScenarioSelectableTreatments(curves, libraryId);

            Assert.Equal(libraryId, curve.LibraryId);
        }

        [Fact]
        public void AddLibraryIdToTreatment_LibraryIdIsNull_NoChange()
        {
            var curve = new TreatmentDTO();
            var curves = new List<TreatmentDTO> { curve };
            var libraryId = Guid.NewGuid();
            curve.LibraryId = libraryId;

            TreatmentDtoListHelper.AddLibraryIdToScenarioSelectableTreatments(curves, null);

            Assert.Equal(libraryId, curve.LibraryId);
        }
    }
}
