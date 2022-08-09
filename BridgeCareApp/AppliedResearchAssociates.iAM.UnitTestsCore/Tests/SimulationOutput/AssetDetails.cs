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
        public static AssetDetail AssetDetail(Guid assetId, string textAttributeName, string numericAttributeName)
        {
            var assetName = RandomStrings.Length11();
            var detail = new AssetDetail(assetName, assetId);
            detail.ValuePerTextAttribute[textAttributeName] = "String";
            detail.ValuePerNumericAttribute[numericAttributeName] = 7;
            return detail;
        }
    }
}
