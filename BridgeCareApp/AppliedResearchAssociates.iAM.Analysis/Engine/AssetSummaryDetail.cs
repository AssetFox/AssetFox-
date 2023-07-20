using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace AppliedResearchAssociates.iAM.Analysis.Engine;

/// <summary>
/// Represents the current condition of an asset
/// </summary>
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

    /// <summary>
    /// Unique asset ID
    /// </summary>
    public Guid AssetId { get; }

    /// <summary>
    /// Name of the asset (not always provided)
    /// </summary>
    public string AssetName { get; }

    /// <summary>
    /// List the current values of each numeric attribute for the asset
    /// </summary>
    public Dictionary<string, double> ValuePerNumericAttribute { get; } = new();

    /// <summary>
    /// List the current values of each text attribute for the asset
    /// </summary>
    public Dictionary<string, string> ValuePerTextAttribute { get; } = new();

    internal AssetSummaryDetail(AssetSummaryDetail original)
    {
        AssetName = original.AssetName;
        AssetId = original.AssetId;

        ValuePerNumericAttribute.CopyFrom(original.ValuePerNumericAttribute);
        ValuePerTextAttribute.CopyFrom(original.ValuePerTextAttribute);
    }
}
