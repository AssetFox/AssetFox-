using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.TestHelpers;
using Xunit;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Assertions
{
    public static class CriterionLibraryDtoAssertions
    {
        public static void AssertValidUpsertResult(CriterionLibraryDTO upsertedLibrary, CriterionLibraryDTO loadedCriterionLibrary)
        {
            if (upsertedLibrary == null)
            {
                var emptyLibrary = new CriterionLibraryDTO();
                ObjectAssertions.Equivalent(emptyLibrary, loadedCriterionLibrary);
            }
            else
            {
                ObjectAssertions.EquivalentExcluding(upsertedLibrary, loadedCriterionLibrary,
                    cl => cl.Id,
                    cl => cl.Name,
                    cl => cl.IsSingleUse,
                    cl => cl.Owner);
                Assert.True(loadedCriterionLibrary.IsSingleUse);
            }
        }
    }
}
