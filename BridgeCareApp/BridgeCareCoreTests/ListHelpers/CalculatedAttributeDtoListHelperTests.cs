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
    public class CalculatedAttributeDtoListHelperTests
    {
        [Fact]
        public void AddModifiedToCalculatedAttribute_Does()
        {
            var curve = new CalculatedAttributeDTO();
            var curves = new List<CalculatedAttributeDTO> { curve };
            Assert.False(curve.IsModified);
            CalculatedAttributeDtoListHelper.AddModifiedToScenarioCalculatedAttributes(curves, true);
            Assert.True(curve.IsModified);
        }


        [Fact]
        public void AddLibraryIdToCalculatedAttribute_Does()
        {
            var curve = new CalculatedAttributeDTO();
            var curves = new List<CalculatedAttributeDTO> { curve };
            var libraryId = Guid.NewGuid();

            CalculatedAttributeDtoListHelper.AddLibraryIdToScenarioCalculatedAttributes(curves, libraryId);

            Assert.Equal(libraryId, curve.LibraryId);
        }

        [Fact]
        public void AddLibraryIdToCalculatedAttribute_LibraryIdIsNull_NoChange()
        {
            var curve = new CalculatedAttributeDTO();
            var curves = new List<CalculatedAttributeDTO> { curve };
            var libraryId = Guid.NewGuid();
            curve.LibraryId = libraryId;

            CalculatedAttributeDtoListHelper.AddLibraryIdToScenarioCalculatedAttributes(curves, null);

            Assert.Equal(libraryId, curve.LibraryId);
        }
    }
}
