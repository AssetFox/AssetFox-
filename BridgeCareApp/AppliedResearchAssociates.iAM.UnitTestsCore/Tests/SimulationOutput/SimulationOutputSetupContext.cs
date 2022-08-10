using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public class SimulationOutputSetupContext
    {
        public string NumericAttributeName { get; set; }
        public string TextAttributeName { get; set; }
        public Guid ManagedAssetId { get; set; }
        public string ManagedAssetName { get; set; }
        public List<int> Years { get; set; }
        public string TreatmentName { get; set; }
        public string BudgetName { get; set; }
        public Guid SimulationId { get; set; }
    }
}
