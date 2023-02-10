using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DTOs;

namespace BridgeCareCoreTests.Tests
{
    public static class SectionCommittedProjectDtos
    {
        public static SectionCommittedProjectDTO Dto(Guid? id = null)
        {
            var resolveId = id ?? Guid.NewGuid();
            var dto = new SectionCommittedProjectDTO
            {
                Id = resolveId,
            };
            return dto;
        }
    }
}
