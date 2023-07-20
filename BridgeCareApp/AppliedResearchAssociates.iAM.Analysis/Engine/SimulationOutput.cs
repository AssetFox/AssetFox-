using System;
using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.Analysis.Engine;

/// <summary>
///     Serialization-friendly aggregate of values for capturing simulation output data.
/// </summary>
public sealed class SimulationOutput
{
    public List<AssetSummaryDetail> InitialAssetSummaries { get; } = new();

    public double InitialConditionOfNetwork { get; set; }

    public DateTime LastModifiedDate { get; set; }

    public List<RollForwardEventDetail> RollForwardEvents { get; } = new();

    public List<SimulationYearDetail> Years { get; } = new();
}
