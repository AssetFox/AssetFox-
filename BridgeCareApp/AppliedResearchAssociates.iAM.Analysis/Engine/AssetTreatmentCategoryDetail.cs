using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DTOs.Enums;

namespace AppliedResearchAssociates.iAM.Analysis.Engine
{
    public class AssetTreatmentCategoryDetail
    {
        public AssetTreatmentCategoryDetail(Guid assetId, string assetName, TreatmentCategory treatmentCategory) {
            AssetId = assetId;
            AssetName = assetName;
            TreatmentCategory = treatmentCategory;
        }

        public Guid AssetId;
        public string AssetName;
        public TreatmentCategory TreatmentCategory;
    }
}
