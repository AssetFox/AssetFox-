using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DTOs.Enums;

namespace AppliedResearchAssociates.iAM.Analysis.Engine
{
    /// <summary>
    ///     Serialization-friendly aggregate of values for capturing simulation output data.
    /// </summary>
    public sealed class SimulationOutput
    {
        public double InitialConditionOfNetwork { get; set; }

        public List<AssetSummaryDetail> InitialAssetSummaries { get; } = new List<AssetSummaryDetail>();

        public List<SimulationYearDetail> Years { get; } = new List<SimulationYearDetail>();

        public DateTime LastModifiedDate { get; set; }

        public List<AssetTreatmentCategoryDetail> AssetTreatmentCategories;
    }
}
