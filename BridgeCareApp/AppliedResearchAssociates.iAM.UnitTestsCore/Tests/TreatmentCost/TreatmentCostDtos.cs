using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.DataSources;
using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAM.UnitTestsCore
{
    public static class TreatmentCostDtos
    {
        public static TreatmentCostDTO Dto(Guid? id = null)
        {
            var resolveId = id ?? Guid.NewGuid();
            return new TreatmentCostDTO
            {
                Id = resolveId,
            };
        }

        public static TreatmentCostDTO WithEquationAndCriterionLibrary(
            Guid? id = null,
            Guid? equationId = null,
            Guid? criterionLibraryId = null,
            string equation = null)
        {
            var resolveId = id ?? Guid.NewGuid();
            EquationDTO equationDto;
            if (equation == null)
            {
                equationDto = EquationDtos.AgePlus1(equationId);
            } else
            {
                var resolveEquationId = equationId ?? Guid.NewGuid();
                equationDto = EquationDtos.WithExpression(resolveEquationId, equation);
            }
            var criterionLibrary = CriterionLibraryDtos.Dto(criterionLibraryId, "True");
            var cost = new TreatmentCostDTO
            {
                Id = resolveId,
                Equation = equationDto,
                CriterionLibrary = criterionLibrary,
            };
            return cost;
        }
    }
}
