using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public static class TreatmentPerformanceFactorDtos
    {
        public static TreatmentPerformanceFactorDTO Dto(string attribute, Guid? id = null)
        {
            var resolveId = id ?? Guid.NewGuid();
            var dto = new TreatmentPerformanceFactorDTO
            {
                Attribute = attribute,
                Id = resolveId,
                PerformanceFactor = 2f,
            };
            return dto;
        }
    }
}
