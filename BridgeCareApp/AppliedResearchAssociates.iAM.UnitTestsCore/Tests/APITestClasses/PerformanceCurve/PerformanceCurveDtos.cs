using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests { 
    public static class PerformanceCurveDtos
    {
        public static PerformanceCurveDTO Dto(
            Guid? performanceCurveId = null,
            Guid? criterionLibraryId = null,
            string attribute = "attribute",
            string equation = "equation"
            )
        {
            var resolveCurveId = performanceCurveId ?? Guid.NewGuid();
            var resolveLibraryId = criterionLibraryId ?? Guid.NewGuid();
            var dto = new PerformanceCurveDTO
            {
                Attribute = attribute,
                Equation = new EquationDTO
                {
                    Expression = equation,
                },
                Id = resolveCurveId,
                Name = "Performance curve",
                CriterionLibrary = new CriterionLibraryDTO
                {
                    Id = resolveLibraryId,
                    IsSingleUse = true,
                }
            };
            return dto;
        }
    }
}
