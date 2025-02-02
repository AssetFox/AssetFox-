﻿using System;
using AppliedResearchAssociates.iAM.DTOs.Enums;

namespace BridgeCareCore.Models
{
    public class WorkQueueMetadata
    {
        public WorkType WorkType { get; set; }
        public DomainType DomainType { get; set; }
        public string PreviousRunTime { get; set; }
        public Guid DomainId { get; set; }
    }
}
