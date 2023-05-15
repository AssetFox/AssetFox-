using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace AppliedResearchAssociates.iAM.Analysis.Engine
{
    /// <summary>
    /// .
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
        /// .
        /// </summary>
        public string AssetName { get; }

        /// <summary>
        /// .
        /// </summary>
        public Guid AssetId { get; }

        public Dictionary<string, double> ValuePerNumericAttribute { get; } = new Dictionary<string, double>();

        public Dictionary<string, string> ValuePerTextAttribute { get; } = new Dictionary<string, string>();

        internal AssetSummaryDetail(AssetSummaryDetail original)
        {
            AssetName = original.AssetName;
            AssetId = original.AssetId;

            ValuePerNumericAttribute.CopyFrom(original.ValuePerNumericAttribute); 
            ValuePerTextAttribute.CopyFrom(original.ValuePerTextAttribute);
        }
    }
}
