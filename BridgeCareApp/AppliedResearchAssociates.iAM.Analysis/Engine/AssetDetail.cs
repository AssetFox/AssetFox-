using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace AppliedResearchAssociates.iAM.Analysis.Engine
{
    /// <summary>
    /// Represents the state of a specific asset in the simulation, usually as part of an array representing the state of
    /// the selected network at the end of a specific cycle.
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
        /// A specific treatment that has been applied to this asset.  Can be the default treatment (e.g., "No Treatment")
        /// </summary>
        public string AppliedTreatment { get; set; }

        /// <summary>
        /// Reason the treatment was applied
        /// </summary>
        public TreatmentCause TreatmentCause { get; set; }

        /// <summary>
        /// List of treatments considered as part of the current analysis cycle
        /// </summary>
        public List<TreatmentConsiderationDetail> TreatmentConsiderations { get; } = new List<TreatmentConsiderationDetail>();

        /// <summary>
        /// Indicates that the funding for the applied treatment ignores any spending limits
        /// </summary>
        public bool TreatmentFundingIgnoresSpendingLimit { get; set; }

        /// <summary>
        /// List of available treatments for this asset
        /// </summary>
        public List<TreatmentOptionDetail> TreatmentOptions { get; } = new List<TreatmentOptionDetail>();

        /// <summary>
        /// List of rejection reasosns for treatments that could be applied to this asset.
        /// </summary>
        public List<TreatmentRejectionDetail> TreatmentRejections { get; } = new List<TreatmentRejectionDetail>();

        /// <summary>
        /// Lists treatments that could be applied but not at the same time
        /// </summary>
        public List<TreatmentSchedulingCollisionDetail> TreatmentSchedulingCollisions { get; } = new List<TreatmentSchedulingCollisionDetail>();

        /// <summary>
        /// Indicates the status of the applied treatment
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
