using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.TestHelpers;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public static class AssetSummaryDetails
    {
        public static AssetSummaryDetail Detail(string assetName, Guid assetId, string numericAttributeName, string textAttributeName)
        {
            var detail = new AssetSummaryDetail(assetName, assetId);
            detail.ValuePerTextAttribute[textAttributeName] = "Hello";
            detail.ValuePerNumericAttribute[numericAttributeName] = 6;
            return detail;
        }

        public static AssetSummaryDetail Detail(SimulationOutputSetupContext setupContext, AssetNameIdPair assetNameIdPair)
            => Detail(assetNameIdPair.Name, assetNameIdPair.Id, setupContext.NumericAttributeName, setupContext.TextAttributeName);
    }
}
