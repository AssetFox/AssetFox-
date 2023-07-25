using System;
using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.Analysis.Engine;

/// <summary>
///     The results of a simulation.
/// </summary>
/// <remarks>Serialization-friendly aggregate of values for capturing simulation output data.</remarks>
public sealed class SimulationOutput
{
    /// <summary>
    ///     List of assets and their conditions at the start of a scenario.
    /// </summary>
    public List<AssetSummaryDetail> InitialAssetSummaries { get; } = new();

    /// <summary>
    ///     Average value of the simulation's benefit attribute.
    /// </summary>
    public double InitialConditionOfNetwork { get; set; }

    /// <summary>
    ///     Time when the scenario was last updated (typically when it was run).
    /// </summary>
    public DateTime LastModifiedDate { get; set; }

    /// <summary>
    ///     Any events that may have occurred before the analysis period began. For example,
    ///     committed projects, which can cause treatment shadaows that affect treatment
    ///     considerations during the analysis period.
    /// </summary>
    public List<RollForwardEventDetail> RollForwardEvents { get; } = new();

    /// <summary>
    ///     Results for each year in a scenario.
    /// </summary>
    public List<SimulationYearDetail> Years { get; } = new();
}
