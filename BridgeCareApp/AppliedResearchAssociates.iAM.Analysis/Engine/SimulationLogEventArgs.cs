using System;

namespace AppliedResearchAssociates.iAM.Analysis.Engine;

public class SimulationLogEventArgs: EventArgs
{
    public SimulationLogEventArgs(SimulationLogMessageBuilder messageBuilder)
    {
        MessageBuilder = messageBuilder;
    }
    public SimulationLogMessageBuilder MessageBuilder { get; set; }
}
