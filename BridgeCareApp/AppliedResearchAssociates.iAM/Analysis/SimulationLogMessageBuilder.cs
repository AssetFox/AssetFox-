﻿using System;
using System.Collections.Generic;
using System.Text;
using AppliedResearchAssociates.iAM.DTOs.Static;

namespace AppliedResearchAssociates.CalculateEvaluate
{
    public class SimulationLogMessageBuilder
    {
        public Guid SimulationId { get; set; }
        public SimulationLogStatus Status { get; set; }
        public SimulationLogSubject Subject { get; set; }
        public string Message { get; set; }
    }
}
