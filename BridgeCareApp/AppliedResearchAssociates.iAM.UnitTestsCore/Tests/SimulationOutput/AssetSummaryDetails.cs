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
        public static AssetSummaryDetail Detail(Guid assetId, string numericAttributeName, string textAttributeName)
        {
            var name = RandomStrings.Length11();
            var detail = new AssetSummaryDetail(name, assetId);
            detail.ValuePerTextAttribute[textAttributeName] = "Hello";
            detail.ValuePerNumericAttribute[numericAttributeName] = 6;
            return detail;
        }

        public static AssetSummaryDetail Detail(SimulationOutputSetupContext setupContext)
            => Detail(setupContext.ManagedAssetId, setupContext.NumericAttributeName, setupContext.TextAttributeName);
    }
}
