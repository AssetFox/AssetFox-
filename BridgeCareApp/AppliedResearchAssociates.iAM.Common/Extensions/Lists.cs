using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppliedResearchAssociates.iAM.Common
{
    public static class Lists
    {
        public static List<T> New<T>(params T[] items)
        {
            return new List<T>(items);
        }
    }
}
