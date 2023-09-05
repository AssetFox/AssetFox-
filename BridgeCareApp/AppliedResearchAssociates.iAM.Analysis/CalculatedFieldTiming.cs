namespace AppliedResearchAssociates.iAM.Analysis;

public enum CalculatedFieldTiming
{
    /// <summary>
    ///     Indicates that a calculated field will always be computed using the most recently
    ///     updated values of its parameters.
    /// </summary>
    OnDemand = 1,

    /// <summary>
    ///     Indicates that a calculated field will have its value "fixed" <em>before</em>
    ///     performance curves are applied. The field will be "unfixed" at the end of the
    ///     analysis year.
    /// </summary>
    PreDeterioration,

    /// <summary>
    ///     Indicates that a calculated field will have its value "fixed" <em>after</em>
    ///     performance curves are applied and <em>before</em> treatments are considered. The
    ///     field will be "unfixed" at the end of the analysis year.
    /// </summary>
    PostDeterioration,
}
