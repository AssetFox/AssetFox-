﻿using System;
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
        public static AssetDetail AssetDetail(SimulationOutputSetupContext context, AssetNameIdPair assetNameIdPair, int year)
        {
            var assetId = assetNameIdPair.Id;
            var assetName = assetNameIdPair.Name;
            var textAttributeNames = context.TextAttributeNames;
            var numericAttributeNames = context.NumericAttributeNames;
            var detail = new AssetDetail(assetName, assetId)
            {
                AppliedTreatment = "Treatment",
                TreatmentCause = TreatmentCause.Undefined,
                TreatmentFundingIgnoresSpendingLimit = true,
                TreatmentStatus = TreatmentStatus.Applied,
            };
            foreach (var textAttributeName in textAttributeNames)
            {
                detail.ValuePerTextAttribute[textAttributeName] = "String";
            }
            foreach (var numericAttributeName in numericAttributeNames)
            {
                detail.ValuePerNumericAttribute[numericAttributeName] = 7;
            }
            var treatmentConsiderationDetail = TreatmentConsiderationDetails.Detail(context);
            detail.TreatmentConsiderations.Add(treatmentConsiderationDetail);
            var treatmentOptionDetail = TreatmentOptionDetails.Detail();
            detail.TreatmentOptions.Add(treatmentOptionDetail);
            var treatmentRejectionDetail = TreatmentRejectionDetails.Detail();
            detail.TreatmentRejections.Add(treatmentRejectionDetail);
            var treatmentSchedulingConsiderationDetail = TreatmentSchedulingCollisionDetails.Detail(year);
            detail.TreatmentSchedulingCollisions.Add(treatmentSchedulingConsiderationDetail);
            return detail;
        }
    }
}
