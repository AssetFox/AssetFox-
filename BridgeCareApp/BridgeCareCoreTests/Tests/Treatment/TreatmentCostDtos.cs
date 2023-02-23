using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.Validation;

namespace BridgeCareCoreTests.Tests
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
            Guid? criterionLibraryId = null)
        {
            var resolveId = id ?? Guid.NewGuid();
            var equation = EquationDtos.AgePlus1(equationId);
            var criterionLibrary = CriterionLibraryDtos.Dto(criterionLibraryId);
            var cost = new TreatmentCostDTO
            {
                Id = resolveId,
                Equation = equation,
                CriterionLibrary = criterionLibrary,
            };
            return cost;
        }
    }
}
