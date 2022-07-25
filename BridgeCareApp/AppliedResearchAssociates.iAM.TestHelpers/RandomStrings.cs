using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppliedResearchAssociates.iAM.TestHelpers { 
    public static class RandomStrings
    {
        public static string Length11()
        {
            var path = Path.GetRandomFileName();
            return path.Replace(".", "");
        }

        public static string WithPrefix(string prefix)
        {
            var suffix = Length11();
            return $"{prefix}{suffix}";
        }
    }
}
