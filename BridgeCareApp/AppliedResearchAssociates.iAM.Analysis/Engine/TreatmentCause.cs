namespace AppliedResearchAssociates.iAM.Analysis.Engine;

public enum TreatmentCause
{
    /// <summary>
    ///     Indicates the existence of incomplete logic in the analysis engine.
    /// </summary>
    Undefined,

    /// <summary>
    ///     Indicates a treatment was not selected by the analysis engine.
    /// </summary>
    NoSelection,

    /// <summary>
    ///     Indicates a treatment was selected by the analysis engine.
    /// </summary>
    SelectedTreatment,

    /// <summary>
    ///     Indicates a treatment was scheduled by a previous treatment selection.
    /// </summary>
    ScheduledTreatment,

    /// <summary>
    ///     Indicates a treatment explicitly pre-selected in the input to the analysis engine.
    /// </summary>
    CommittedProject,

    /// <summary>
    ///     Indicates a non-initial year of a multi-year treatment. (Initial year uses <see cref="SelectedTreatment"/>.)
    /// </summary>
    CashFlowProject,
}
