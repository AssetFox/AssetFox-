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
        /// <summary>
        /// .
        /// </summary>
        public double InitialConditionOfNetwork { get; set; }

        /// <summary>
        /// .
        /// </summary>
        public List<AssetSummaryDetail> InitialAssetSummaries { get; } = new List<AssetSummaryDetail>();

        /// <summary>
        /// .
        /// </summary>
        public List<SimulationYearDetail> Years { get; } = new List<SimulationYearDetail>();

        /// <summary>
        /// .
        /// </summary>
        public DateTime LastModifiedDate { get; set; }

        /// <summary>
        /// .
        /// </summary>
        public List<AssetTreatmentCategoryDetail> AssetTreatmentCategories;
    }
}
