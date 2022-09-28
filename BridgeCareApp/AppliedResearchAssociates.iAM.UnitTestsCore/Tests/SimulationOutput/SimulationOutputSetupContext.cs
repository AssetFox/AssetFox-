using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public class SimulationOutputSetupContext
    {
        public List<string> NumericAttributeNames { get; set; }
        public List<string> TextAttributeNames { get; set; }
        public List<AssetNameIdPair> AssetNameIdPairs { get; set; }
        public List<int> Years { get; set; }
        public string TreatmentName { get; set; }
        public string BudgetName { get; set; }
        public Guid SimulationId { get; set; }
    }
}
