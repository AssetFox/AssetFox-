using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace AppliedResearchAssociates.iAM.TestHelpers.Extensions
{
    public static class StringExtensions
    {
        public static List<string> ToLines(string splitMe)
        {
            using (StringReader sr = new StringReader(splitMe))
            {
                var lines = new List<string>();
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    lines.Add(line);
                }
                return lines;
            }
        }
    }
}
