using System;
using AppliedResearchAssociates.iAM.DTOs.Enums;

namespace BridgeCareCore.Models.General_Work_Queue
{
    public class WorkQueueRequestModel
    {
        public Guid DomainId { get; set; }
        public WorkType WorkType { get; set; }
    }
}
