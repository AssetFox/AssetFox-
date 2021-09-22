using System;
using System.Collections.Generic;
using System.Text;
using AppliedResearchAssociates.CalculateEvaluate;

namespace AppliedResearchAssociates.iAM.Analysis.Engine
{
    public class SimulationLogMessageBuilder
    {
        public Guid SimulationId { get; set; }
        public SimulationLogStatus Status { get; set; }
        public SimulationLogSubject Subject { get; set; }
        public string Message { get; set; }
    }
}
