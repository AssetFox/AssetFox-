namespace AppliedResearchAssociates.iAM.Analysis.Engine
{
    /// <summary>
    /// .
    /// </summary>
    public sealed class DeficientConditionGoalDetail : ConditionGoalDetail
    {
        /// <summary>
        /// .
        /// </summary>
        public double ActualDeficientPercentage { get; set; }

        /// <summary>
        /// .
        /// </summary>
        public double AllowedDeficientPercentage { get; set; }

        /// <summary>
        /// .
        /// </summary>
        public double DeficientLimit { get; set; }
    }
}
