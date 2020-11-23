using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BridgeCareCore.Models.SummaryReport
{
    public class WorkSummaryModel
    {
        public List<double> PreviousYearInitialMinC { get; set; }
        public Dictionary<int, (int on, int off)> PoorOnOffCount { get; set; }
        //public List<SimulationDataModel> SimulationDataModels { get; set; }

        //public List<BridgeDataModel> BridgeDataModels { get; set; }

        //public List<string> Treatments { get; set; }

        //public List<BudgetsPerBRKey> BudgetsPerBRKeys { get; set; }

        //public List<UnfundedRecommendationModel> UnfundedRecommendations { get; set; }

        //public ParametersModel ParametersModel { get; set; }
    }
}
