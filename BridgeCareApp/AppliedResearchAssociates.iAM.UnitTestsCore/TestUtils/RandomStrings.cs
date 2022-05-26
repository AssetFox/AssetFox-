using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils
{
    public static class RandomStrings
    {
        public static string Length11()
        {
            var path = Path.GetRandomFileName();
            return path.Replace(".", "");
        }
    }
}
