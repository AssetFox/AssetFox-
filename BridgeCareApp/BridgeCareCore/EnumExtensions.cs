using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BridgeCareCore
{
    public static class EnumExtensions
    {
        public static List<T> GetValues<T>()
            where T: Enum
        {
            var values = Enum.GetValues(typeof(T));
            return values.Cast<T>().ToList();
        }
    }
}
