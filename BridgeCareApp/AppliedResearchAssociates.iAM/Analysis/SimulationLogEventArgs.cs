using System;
using System.Collections.Generic;
using System.Text;
using AppliedResearchAssociates.CalculateEvaluate;

namespace AppliedResearchAssociates.iAM.Analysis
{
    public class SimulationLogEventArgs: EventArgs
    {
        public SimulationLogEventArgs(SimulationLogMessageBuilder messageBuilder)
        {
            MessageBuilder = messageBuilder;
        }
        public SimulationLogMessageBuilder MessageBuilder { get; set; }
    }
}
