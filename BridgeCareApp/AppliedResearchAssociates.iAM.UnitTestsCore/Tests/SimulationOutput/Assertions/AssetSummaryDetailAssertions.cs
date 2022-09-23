using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.TestHelpers;
using Xunit;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public static class AssetSummaryDetailAssertions
    {
        public static void Same(AssetSummaryDetail expected, AssetSummaryDetail actual)
        {
            Assert.Equal(expected.AssetName, actual.AssetName);
            DoubleDictionaryAssertions.ApproximatelySame(expected.ValuePerNumericAttribute, actual.ValuePerNumericAttribute);
            DictionaryAssertions.Same(expected.ValuePerTextAttribute, actual.ValuePerTextAttribute);
        }
    }
}
