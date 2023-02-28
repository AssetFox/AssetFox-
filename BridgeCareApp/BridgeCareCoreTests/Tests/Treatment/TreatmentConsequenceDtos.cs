using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests;

namespace BridgeCareCoreTests.Tests.Treatment
{
    public static class TreatmentConsequenceDtos
    {
        public static TreatmentConsequenceDTO Dto(Guid? id = null)
        {
            var resolveId = id ?? Guid.NewGuid();
            var dto = new TreatmentConsequenceDTO
            {
                Id = resolveId,
                Attribute = "attribute",
                ChangeValue = "1",
            };
            return dto;
        }

        public static TreatmentConsequenceDTO WithEquationAndCriterionLibrary(
            Guid? id = null,
            Guid? equationId = null,
            Guid? criterionLibraryId = null)
        {
            var dto = Dto(id);
            var equation = EquationDtos.AgePlus1(equationId);
            var criterionLibrary = CriterionLibraryDtos.Dto(criterionLibraryId);
            dto.Equation = equation;
            dto.CriterionLibrary = criterionLibrary;
            return dto;
        }
    }
}
