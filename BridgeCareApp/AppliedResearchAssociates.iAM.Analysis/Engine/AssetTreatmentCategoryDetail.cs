using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DTOs.Enums;

namespace AppliedResearchAssociates.iAM.Analysis.Engine
{
    /// <summary>
    /// Treatment category for a specific committed project.
    /// Only used for committed projects
    /// </summary>
    public class AssetTreatmentCategoryDetail
    {
        public AssetTreatmentCategoryDetail(Guid assetId, string assetName, TreatmentCategory treatmentCategory) {
            AssetId = assetId;
            AssetName = assetName;
            TreatmentCategory = treatmentCategory;
        }

        /// <summary>
        /// ID of the asset
        /// </summary>
        public Guid AssetId;

        /// <summary>
        /// Name of the asset (not always provided)
        /// </summary>
        public string AssetName;

        /// <summary>
        /// Category of the treatment provided
        /// </summary>
        public TreatmentCategory TreatmentCategory;
    }
}
