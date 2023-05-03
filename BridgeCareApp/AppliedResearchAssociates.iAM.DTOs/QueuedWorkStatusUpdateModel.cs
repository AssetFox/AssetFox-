using System;
using System.Collections.Generic;
using System.Text;
using AppliedResearchAssociates.iAM.DTOs.Enums;

namespace AppliedResearchAssociates.iAM.DTOs
{
    public class QueuedWorkStatusUpdateModel
    {
        public Guid Id { get; set; }
        public string Status { get; set; }
    }
}
