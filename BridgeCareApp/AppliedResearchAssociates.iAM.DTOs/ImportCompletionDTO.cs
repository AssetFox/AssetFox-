using System;
using System.Collections.Generic;
using System.Text;
using AppliedResearchAssociates.iAM.DTOs.Enums;

namespace AppliedResearchAssociates.iAM.DTOs
{
    public class ImportCompletionDTO
    {
        public Guid Id { get; set; }
        public WorkType DomainType { get; set; }
    }
}
