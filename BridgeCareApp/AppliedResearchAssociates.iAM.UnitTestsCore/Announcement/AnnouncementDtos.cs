using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.UnitTestsCore
{
    public static class AnnouncementDtos
    {
        public static AnnouncementDTO Dto(Guid? id = null)
        {
            var resolveId = id ?? Guid.NewGuid();
            var dto = new AnnouncementDTO
            {
                Content = "Test content",
                Id = resolveId,
                Title = "Test title",
            };
            return dto;
        }
    }
}
