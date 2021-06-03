using System;
using System.Collections.Generic;
using System.Text;
using AppliedResearchAssociates.iAM.Domains;

namespace AppliedResearchAssociates.iAM
{
    public static class SimulationLogMessages
    {
        public static string SectionCalculationReturnedNaN(Section section, PerformanceCurve performanceCurve, string key)
            => $"Calculation for {key} on (Section {section.Name} {section.Id}) using performance curve ({performanceCurve.Name} {performanceCurve.Id}) returned 'not-a-number'";
    }
}
