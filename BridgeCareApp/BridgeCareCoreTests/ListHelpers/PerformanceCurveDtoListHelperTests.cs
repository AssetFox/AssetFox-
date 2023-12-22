using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Services;
using Xunit;

namespace BridgeCareCoreTests
{
    public class PerformanceCurveDtoListHelperTests
    {
        [Fact]
        public void AddModifiedToPerformanceCurve_Does()
        {
            var curve = new PerformanceCurveDTO();
            var curves = new List<PerformanceCurveDTO> { curve };
            Assert.False(curve.IsModified);
            PerformanceCurveDtoListHelper.AddModifiedToScenarioPerformanceCurve(curves, true);
            Assert.True(curve.IsModified);
        }


        [Fact]
        public void AddLibraryIdToPerformanceCurve_Does()
        {
            var curve = new PerformanceCurveDTO();
            var curves = new List<PerformanceCurveDTO> { curve };
            var libraryId = Guid.NewGuid();

            PerformanceCurveDtoListHelper.AddLibraryIdToScenarioPerformanceCurves(curves, libraryId);

            Assert.Equal(libraryId, curve.LibraryId);
        }

        [Fact]
        public void AddLibraryIdToPerformanceCurve_LibraryIdIsNull_NoChange()
        {
            var curve = new PerformanceCurveDTO();
            var curves = new List<PerformanceCurveDTO> { curve };
            var libraryId = Guid.NewGuid();
            curve.LibraryId = libraryId;

            PerformanceCurveDtoListHelper.AddLibraryIdToScenarioPerformanceCurves(curves, null);

            Assert.Equal(libraryId, curve.LibraryId);
        }
    }
}
