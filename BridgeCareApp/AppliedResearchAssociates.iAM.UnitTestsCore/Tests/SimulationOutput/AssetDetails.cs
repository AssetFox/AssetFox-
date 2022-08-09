using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.TestHelpers;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public static class AssetDetails
    { 
        public static AssetDetail AssetDetail(SimulationOutputSetupContext context)
        {
            var assetName = RandomStrings.Length11();
            var assetId = context.ManagedAssetId;
            var textAttributeName = context.TextAttributeName;
            var numericAttributeName = context.NumericAttributeName;
            var detail = new AssetDetail(assetName, assetId)
            {
                AppliedTreatment = "Treatment",
                TreatmentCause = TreatmentCause.Undefined,
                TreatmentFundingIgnoresSpendingLimit = true,
                TreatmentStatus = TreatmentStatus.Applied,
            };
            detail.ValuePerTextAttribute[textAttributeName] = "String";
            detail.ValuePerNumericAttribute[numericAttributeName] = 7;
            var treatmentConsiderationDetail = TreatmentConsiderationDetails.Detail(context);
            detail.TreatmentConsiderations.Add(treatmentConsiderationDetail);
            var treatmentOptionDetail = TreatmentOptionDetails.Detail();
            detail.TreatmentOptions.Add(treatmentOptionDetail);
            var treatmentRejectionDetail = TreatmentRejectionDetails.Detail();
            detail.TreatmentRejections.Add(treatmentRejectionDetail);
            foreach (var year in context.Years)
            {
                var treatmentSchedulingConsiderationDetail = TreatmentSchedulingCollisionDetails.Detail(year);
                detail.TreatmentSchedulingCollisions.Add(treatmentSchedulingConsiderationDetail);
            }
            return detail;
        }
    }
}
