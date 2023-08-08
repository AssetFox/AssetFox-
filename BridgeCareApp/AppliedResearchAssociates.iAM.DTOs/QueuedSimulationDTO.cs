using System;
using AppliedResearchAssociates.iAM.DTOs.Enums;

namespace AppliedResearchAssociates.iAM.DTOs
{
    public class QueuedWorkDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public DateTime QueueEntryTimestamp { get; set; }
        public DateTime? WorkStartedTimestamp { get; set; }
        public string QueueingUser { get; set; }
        public string CurrentRunTime { get; set; }
        public string PreviousRunTime { get; set; }
        public int QueuePosition { get; set; }
        public string WorkDescription { get; set; }
        public WorkType WorkType { get; set; }
        public DomainType DomainType { get; set; }
        public Guid DomainId { get; set; }
    }
}
