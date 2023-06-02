using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DTOs.Enums;

namespace AppliedResearchAssociates.iAM.Analysis.Engine
{
    /// <summary>
    /// The results of a simulation
    /// </summary>
    public sealed class SimulationOutput
    {
        /// <summary>
        /// Average value of the simulation's benefit attribute
        /// </summary>
        public double InitialConditionOfNetwork { get; set; }

        /// <summary>
        /// List of assets and their conditions at the start of a scenario
        /// </summary>
        public List<AssetSummaryDetail> InitialAssetSummaries { get; } = new List<AssetSummaryDetail>();

        /// <summary>
        /// List of results for a given year in a scenario
        /// </summary>
        public List<SimulationYearDetail> Years { get; } = new List<SimulationYearDetail>();

        /// <summary>
        /// Time when the scenario was last updated (typically when it was run)
        /// </summary>
        public DateTime LastModifiedDate { get; set; }

        /// <summary>
        /// List of assets and the categories of thier committed projects.
        /// Only used for committed projects.
        /// </summary>
        public List<AssetTreatmentCategoryDetail> AssetTreatmentCategories;
    }
}
