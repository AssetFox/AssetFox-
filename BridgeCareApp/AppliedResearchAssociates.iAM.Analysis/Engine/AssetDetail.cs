using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DTOs.Enums;
using Newtonsoft.Json;

namespace AppliedResearchAssociates.iAM.Analysis.Engine
{
    /// <summary>
    /// .
    /// </summary>
    public sealed class AssetDetail : AssetSummaryDetail
    {
        public AssetDetail(AnalysisMaintainableAsset asset) : base(asset)
        {
        }

        [JsonConstructor]
        public AssetDetail(string assetName, Guid assetId) : base(assetName, assetId)
        {
        }

        /// <summary>
        /// .
        /// </summary>
        public string AppliedTreatment { get; set; }

        /// <summary>
        /// .
        /// </summary>
        public TreatmentCause TreatmentCause { get; set; }

        /// <summary>
        /// .
        /// </summary>
        public List<TreatmentConsiderationDetail> TreatmentConsiderations { get; } = new List<TreatmentConsiderationDetail>();

        /// <summary>
        /// .
        /// </summary>
        public bool TreatmentFundingIgnoresSpendingLimit { get; set; }

        /// <summary>
        /// .
        /// </summary>
        public List<TreatmentOptionDetail> TreatmentOptions { get; } = new List<TreatmentOptionDetail>();

        /// <summary>
        /// .
        /// </summary>
        public List<TreatmentRejectionDetail> TreatmentRejections { get; } = new List<TreatmentRejectionDetail>();

        /// <summary>
        /// .
        /// </summary>
        public List<TreatmentSchedulingCollisionDetail> TreatmentSchedulingCollisions { get; } = new List<TreatmentSchedulingCollisionDetail>();

        /// <summary>
        /// .
        /// </summary>
        public TreatmentStatus TreatmentStatus { get; set; }

        internal AssetDetail(AssetDetail original) : base(original)
        {
            AppliedTreatment = original.AppliedTreatment;
            TreatmentCause = original.TreatmentCause;
            TreatmentStatus = original.TreatmentStatus;
            TreatmentFundingIgnoresSpendingLimit = original.TreatmentFundingIgnoresSpendingLimit;

            TreatmentRejections.AddRange(original.TreatmentRejections.Select(_ => new TreatmentRejectionDetail(_)));
            TreatmentOptions.AddRange(original.TreatmentOptions.Select(_ => new TreatmentOptionDetail(_)));
            TreatmentConsiderations.AddRange(original.TreatmentConsiderations.Select(_ => new TreatmentConsiderationDetail(_)));
            TreatmentSchedulingCollisions.AddRange(original.TreatmentSchedulingCollisions.Select(_ => new TreatmentSchedulingCollisionDetail(_)));
        }
    }
}
