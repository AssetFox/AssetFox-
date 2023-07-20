using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace AppliedResearchAssociates.iAM.Analysis.Engine;

public class AssetSummaryDetail
{
    public AssetSummaryDetail(AnalysisMaintainableAsset asset)
    {
        if (asset is null)
        {
            throw new ArgumentNullException(nameof(asset));
        }

        AssetName = asset.AssetName;
        AssetId = asset.Id;
    }

    [JsonConstructor]
    public AssetSummaryDetail(string assetName, Guid assetId)
    {
        AssetName = assetName ?? "";
        AssetId = assetId;
    }

    public Guid AssetId { get; }

    public string AssetName { get; }

    public Dictionary<string, double> ValuePerNumericAttribute { get; } = new();

    public Dictionary<string, string> ValuePerTextAttribute { get; } = new();

    internal AssetSummaryDetail(AssetSummaryDetail original)
    {
        AssetName = original.AssetName;
        AssetId = original.AssetId;

        ValuePerNumericAttribute.CopyFrom(original.ValuePerNumericAttribute);
        ValuePerTextAttribute.CopyFrom(original.ValuePerTextAttribute);
    }
}
