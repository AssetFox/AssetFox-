using System;
using System.Collections.Generic;
using System.Text;

namespace AppliedResearchAssociates.iAM.DTOs.Static
{
    public static class CriterionLibraryValidityChecker
    {
        public static bool IsValid(this CriterionLibraryDTO criterionLibraryDTO)
        {
            bool isValid =
            criterionLibraryDTO?.Id != null && criterionLibraryDTO?.Id != Guid.Empty &&
                                     !string.IsNullOrEmpty(criterionLibraryDTO.MergedCriteriaExpression);
            return isValid;
        }
    }
}
