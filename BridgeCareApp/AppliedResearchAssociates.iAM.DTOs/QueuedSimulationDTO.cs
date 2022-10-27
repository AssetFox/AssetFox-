﻿using System;

namespace AppliedResearchAssociates.iAM.DTOs
{
    public class QueuedSimulationDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public DateTime QueueEntryTimestamp { get; set; }
        public DateTime? WorkStartedTimestamp { get; set; }
        public string QueueingUser { get; set; }
        public string CurrentRunTime { get; set; }
        public string PreviousRunTime { get; set; }
        public int QueuePosition { get; set; }
    }
}
