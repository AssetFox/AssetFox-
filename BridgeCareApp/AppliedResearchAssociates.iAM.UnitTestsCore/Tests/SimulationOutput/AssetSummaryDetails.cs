using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public static class AssetSummaryDetails
    {
        public static AssetSummaryDetail Detail(string assetName, Guid assetId, List<string> numericAttributeNames, List<string> textAttributeNames)
        {
            var detail = new AssetSummaryDetail(assetName, assetId);
            foreach (var textAttributeName in textAttributeNames)
            {
                detail.ValuePerTextAttribute[textAttributeName] = TestAttributeValues.GenericStringValue;
            }
            foreach (var numericAttributeName in numericAttributeNames)
            {
                detail.ValuePerNumericAttribute[numericAttributeName] = 6;
            }
            return detail;
        }

        public static AssetSummaryDetail Detail(SimulationOutputSetupContext setupContext, AssetNameIdPair assetNameIdPair)
            => Detail(assetNameIdPair.Name, assetNameIdPair.Id, setupContext.NumericAttributeNames, setupContext.TextAttributeNames);
    }
}
