using System;
using System.Collections.Generic;
using System.Text;
using AppliedResearchAssociates.iAM.Domains;

namespace AppliedResearchAssociates.iAM
{
    public static class SimulationLogMessages
    {
        public static string SectionCalculationReturnedNaN(Section section, string key)
            => $"Calculation for {key} on Section {section.Name} {section.Id} returned 'not-a-number'";
    }
}
