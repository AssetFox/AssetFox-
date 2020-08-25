using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.Analysis
{
    /// <summary>
    ///     Serialization-friendly aggregate of values for capturing simulation output data.
    /// </summary>
    public sealed class SimulationOutput
    {
        public SimulationStatusDetail FinalStatus { get; } = new SimulationStatusDetail();

        public List<SectionSummaryDetail> SectionSummaries { get; } = new List<SectionSummaryDetail>();

        public List<SimulationYearDetail> Years { get; } = new List<SimulationYearDetail>();
    }
}
