using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public static class AssetNameIdPairLists
    {
        public static List<AssetNameIdPair> Random(int count)
        {
            var list = new List<AssetNameIdPair>();
            for (int i=0; i<count; i++)
            {
                var pair = AssetNameIdPairs.Random();
                list.Add(pair);
            }
            return list;
        }
    }
}
