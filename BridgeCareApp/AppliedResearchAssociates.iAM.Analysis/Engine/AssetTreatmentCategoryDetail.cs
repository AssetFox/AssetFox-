using System;
using AppliedResearchAssociates.iAM.DTOs.Enums;
using Newtonsoft.Json;

namespace AppliedResearchAssociates.iAM.Analysis.Engine;

public sealed class AssetTreatmentCategoryDetail
{
    [JsonConstructor]
    public AssetTreatmentCategoryDetail(Guid assetId, string assetName, TreatmentCategory treatmentCategory)
    {
        AssetId = assetId;
        AssetName = assetName;
        TreatmentCategory = treatmentCategory;
    }

    public Guid AssetId { get; }

    public string AssetName { get; }

    public TreatmentCategory TreatmentCategory { get; }
}
