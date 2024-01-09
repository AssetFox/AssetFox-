using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.TestHelpers;
using Xunit;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public class SimulationCloningCriterionLibraryDtoAssertions
    {

        public static void AssertValidLibraryClone(CriterionLibraryDTO originalLibrary, CriterionLibraryDTO clonedLibrary, Guid? expectedOwnerId)
        {
            var resolveOwnerId = expectedOwnerId ?? Guid.Empty;
            ObjectAssertions.EquivalentExcluding(originalLibrary, clonedLibrary, x => x.Id, x => x.IsSingleUse, x => x.Name, x => x.Owner);
            Assert.NotEqual(originalLibrary.Id, clonedLibrary.Id);
            Assert.True(clonedLibrary.IsSingleUse);
            Assert.Equal(resolveOwnerId, clonedLibrary.Owner);
        }
    }
}
