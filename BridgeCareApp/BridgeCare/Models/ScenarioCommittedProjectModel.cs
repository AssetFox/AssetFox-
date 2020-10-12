using System.Collections.Generic;

namespace BridgeCare.Models
{
    public class ScenarioCommittedProjectModel
    {
        public string BrKey { get; set; }
        public string BmsId { get; set; }
        public int Year { get; set; }
        public CommittedProjectTreatmentModel Treatment { get; set; }
        public CommittedProjectBudgetModel Budget { get; set; }
        public double Cost { get; set; }
    }

    public class CommittedProjectTreatmentModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class CommittedProjectBudgetModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
