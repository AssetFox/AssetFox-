using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.Analysis
{
    /// <summary>
    ///     Serialization-friendly aggregate of values for capturing simulation output data.
    /// </summary>
    public sealed class SimulationYearDetail
    {
        public SimulationYearDetail(int year) => Year = year;

        public List<DeficientConditionGoalDetail> DeficientConditionGoals { get; } = new List<DeficientConditionGoalDetail>();

        public double InitialConditionOfNetwork { get; set; }

        public List<SectionDetail> Sections { get; } = new List<SectionDetail>();

        public List<TargetConditionGoalDetail> TargetConditionGoals { get; } = new List<TargetConditionGoalDetail>();

        public int Year { get; }
    }
}
