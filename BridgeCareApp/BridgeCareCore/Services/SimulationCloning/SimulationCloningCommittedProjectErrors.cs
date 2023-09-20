using System;
using System.Collections.Generic;
using System.Text;

namespace BridgeCareCore.Services.SimulationCloning
{
    public class SimulationCloningCommittedProjectErrors
    {
        public List<string> BudgetsPreventingCloning { get; set; } = new List<string>();
        public int NumberOfCommittedProjectsAffected { get; set; } = 0;


    }
}
