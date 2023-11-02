using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.SelectableTreatment
{
    public static class TreatmentLibraryDtos
    {
        public static TreatmentLibraryDTO Empty(Guid? id = null)
        {
            var resolveId = id ?? Guid.NewGuid();
            var dto = new TreatmentLibraryDTO
            {
                Id = resolveId,
                Name = "Treatment library",
                Treatments = new List<TreatmentDTO>(),
                Description = "Treatment library description",
            };
            return dto;
        }

        public static TreatmentLibraryDTO WithSingleTreatment(Guid? id = null, Guid? treatmentId = null)
        {
            var dto = Empty(id);
            var treatment = TreatmentDtos.Dto(treatmentId);
            dto.Treatments.Add(treatment);
            return dto;
        }
    }
}
