namespace AppliedResearchAssociates.iAM.Analysis.Engine;

/// <summary>
///     General information for all condition goals.
/// </summary>
public abstract class ConditionGoalDetail
{
    /// <summary>
    ///     Name of the attribute the goal is associated with.
    /// </summary>
    public string AttributeName { get; set; }

    /// <summary>
    ///     Is the goal met in this specific case?
    /// </summary>
    public bool GoalIsMet { get; set; }

    /// <summary>
    ///     Name of the condition goal.
    /// </summary>
    public string GoalName { get; set; }
}
