using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.Analysis
{
    public sealed class SimulationYearDetail
    {
        public SimulationYearDetail(int year) => Year = year;

        public SimulationStatusDetail InitialStatus { get; } = new SimulationStatusDetail();

        public List<SectionDetail> Sections { get; } = new List<SectionDetail>();

        public int Year { get; }
    }
}
