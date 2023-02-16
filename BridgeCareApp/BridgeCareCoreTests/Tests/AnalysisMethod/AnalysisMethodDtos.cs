using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DTOs.Enums;
using AppliedResearchAssociates.iAM.DTOs;

namespace BridgeCareCoreTests.Tests.AnalysisMethod
{
    public static class AnalysisMethodDtos
    {
        internal static AnalysisMethodDTO Default(Guid id)
        {
            var benefit = new BenefitDTO();
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
