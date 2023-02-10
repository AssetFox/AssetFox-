using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public static class PerformanceCurveLibraryDtos
    {

        public static PerformanceCurveLibraryDTO Empty(Guid? id = null)
        {
            var resolveId = id ?? Guid.NewGuid();
            var dto = new PerformanceCurveLibraryDTO
            {
                Id = resolveId,
                Name = "Test Name"
            };
            return dto;
        }
    }
}
