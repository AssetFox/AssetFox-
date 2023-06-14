namespace AppliedResearchAssociates.iAM.DTOs.Enums
{
    /// <summary>
    /// Defines spending stratigies used by simulation options
    /// </summary>
    public enum SpendingStrategy
    {
        /// <summary>
        /// Spend no funding (apply no treatments)
        /// </summary>
        NoSpending,

        /// <summary>
        /// Ignore budget levels and spend as required
        /// </summary>
        UnlimitedSpending,

        /// <summary>
        /// Spend funding until both deficient and condition
        /// goals are met.  Provided budget levels are ignored
        /// </summary>
        UntilTargetAndDeficientConditionGoalsMet,

        /// <summary>
        /// Spend funding until condition goals are met,
        /// provided budget levels are ignored.
        /// </summary>
        UntilTargetConditionGoalsMet,

        /// <summary>
        /// Spend funding until deficiency goals are met,
        /// provided budget levels are ignored.
        /// </summary>
        UntilDeficientConditionGoalsMet,

        /// <summary>
        /// Spend funding until either no eligible treatments
        /// remain or no budget remains.
        /// </summary>
        AsBudgetPermits,
    }
}
