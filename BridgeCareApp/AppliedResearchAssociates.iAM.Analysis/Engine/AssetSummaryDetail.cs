using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace AppliedResearchAssociates.iAM.Analysis.Engine
{
    public class AssetSummaryDetail
    {
        public AssetSummaryDetail(AnalysisMaintainableAsset asset)
        {
            if (asset is null)
            {
                throw new ArgumentNullException(nameof(asset));
            }

            AssetName = asset.AssetName;
        }

        [JsonConstructor]
        public AssetSummaryDetail(string assetName)
        {
            AssetName = assetName ?? throw new ArgumentNullException(nameof(assetName)); 
        }

        public string AssetName { get; }

        public Dictionary<string, double> ValuePerNumericAttribute { get; } = new Dictionary<string, double>();

        public Dictionary<string, string> ValuePerTextAttribute { get; } = new Dictionary<string, string>();

        internal AssetSummaryDetail(AssetSummaryDetail original)
        {
            AssetName = original.AssetName;

            ValuePerNumericAttribute.CopyFrom(original.ValuePerNumericAttribute);
            ValuePerTextAttribute.CopyFrom(original.ValuePerTextAttribute);
        }
    }
}
