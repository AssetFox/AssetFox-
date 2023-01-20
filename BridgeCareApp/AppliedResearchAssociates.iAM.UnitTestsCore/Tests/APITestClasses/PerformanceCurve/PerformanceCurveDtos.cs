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
            Guid criterionLibraryId,
            Guid performanceCurveId,
            string attribute,
            string equation)
        {
            var dto = new PerformanceCurveDTO
            {
                Attribute = attribute,
                Equation = new EquationDTO
                {
                    Expression = equation,
                },
                Id = performanceCurveId,
                Name = "Performance curve",
                CriterionLibrary = new CriterionLibraryDTO
                {
                    Id = criterionLibraryId,
                    IsSingleUse = true,
                }
            };
            return dto;
        }
    }
}
