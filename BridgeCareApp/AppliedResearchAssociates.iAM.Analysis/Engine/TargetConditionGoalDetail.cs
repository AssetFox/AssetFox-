namespace AppliedResearchAssociates.iAM.Analysis.Engine
{
    /// <summary>
    /// The average (weighted by the spatial weighting attribute) value
    /// the simulation should achieve for a specific attribute in a
    /// given time period
    /// </summary>
    public sealed class TargetConditionGoalDetail : ConditionGoalDetail
    {
        /// <summary>
        /// The actual average value (weighted by the spatial weighting
        /// attribute) for this attribute at the END of the time period
        /// </summary>
        public double ActualValue { get; set; }

        /// <summary>
        /// The target average value (weighted by the spatial weighting
        /// attribute) for this attribute
        /// </summary>
        public double TargetValue { get; set; }
    }
}
