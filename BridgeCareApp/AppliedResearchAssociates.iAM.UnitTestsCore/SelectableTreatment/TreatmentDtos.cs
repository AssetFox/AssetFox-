using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.UnitTestsCore
{
    public static class TreatmentDtos
    {
        public static TreatmentDTO Dto(Guid? id = null, string name = "Treatment name")
        {
            var resolveId = id ?? Guid.NewGuid();
            var dto = new TreatmentDTO
            {
                Id = resolveId,
                Name = name,
                Description = "Treatment description",
            };
            return dto;
        }

        public static TreatmentDTO DtoWithEmptyCostsAndConsequencesLists(Guid? id = null, string name = "Treatment name")
        {

            var resolveId = id ?? Guid.NewGuid();
            var dto = new TreatmentDTO
            {
                Id = resolveId,
                Name = name,
                Description = "Treatment description",
                Costs = new List<TreatmentCostDTO>(),
                Consequences = new List<TreatmentConsequenceDTO>(),
                PerformanceFactors = new List<TreatmentPerformanceFactorDTO>(),
            };
            return dto;
        }
    }
}
