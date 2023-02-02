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
        public static TreatmentDTO Dto(Guid? id = null)
        {
            var resolveId = id ?? Guid.NewGuid();
            var dto = new TreatmentDTO
            {
                Id = resolveId,
                Name = "Treatment name",
                Description = "Treatment description",
            };
            return dto;
        }
    }
}
