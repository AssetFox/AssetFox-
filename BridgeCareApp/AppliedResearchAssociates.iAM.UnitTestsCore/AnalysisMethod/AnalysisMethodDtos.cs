using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DTOs.Enums;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Benefit;

namespace AppliedResearchAssociates.iAM.UnitTestsCore
{
    public static class AnalysisMethodDtos
    {
        public static AnalysisMethodDTO Default(Guid id, string attributeName = "attribute")
        {
            var benefit = BenefitDtos.Dto(attributeName);
            var criterionLibrary = new CriterionLibraryDTO();
            var dto = new AnalysisMethodDTO
            {
                Benefit = benefit,
                CriterionLibrary = criterionLibrary,
                Id = id,
                OptimizationStrategy = OptimizationStrategy.Benefit,
                SpendingStrategy = SpendingStrategy.NoSpending,
            };
            return dto;
        }
    }
}
