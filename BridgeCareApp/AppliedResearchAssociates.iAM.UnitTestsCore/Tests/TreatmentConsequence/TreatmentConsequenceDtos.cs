using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public static class TreatmentConsequenceDtos
    {
        public static TreatmentConsequenceDTO Dto(
            Guid? id = null,
            string attribute = "attribute",
            string equation = null,
            string criterion = null)
        {
            var resolveId = id ?? Guid.NewGuid();
            var consequenceEquation = equation == null ? null :
                EquationDtos.WithExpression(Guid.NewGuid(), equation);
            var consequenceCriterion = criterion == null ? null : CriterionLibraryDtos.Dto(
                null, criterion);
            var dto = new TreatmentConsequenceDTO
            {
                Id = resolveId,
                Attribute = attribute,
                Equation = consequenceEquation,
                ChangeValue = "1",
                CriterionLibrary = consequenceCriterion,
            };
            return dto;
        }

        public static TreatmentConsequenceDTO WithEquationAndCriterionLibrary(
            Guid? id = null,
            string attribute = "attribute",
            Guid? equationId = null,
            Guid? criterionLibraryId = null)
        {
            var dto = Dto(id, attribute);
            var equation = EquationDtos.AgePlus1(equationId);
            var criterionLibrary = CriterionLibraryDtos.Dto(criterionLibraryId);
            dto.Equation = equation;
            dto.CriterionLibrary = criterionLibrary;
            return dto;
        }
    }
}
