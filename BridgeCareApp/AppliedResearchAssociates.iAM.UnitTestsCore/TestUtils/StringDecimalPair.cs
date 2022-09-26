using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils
{
    public class StringDecimalPair
    {
        public string String { get; set; }
        /// <summary>Due to space constraints, we use decimal.MinValue when there is no decimal.</summary> 
        public decimal Decimal { get; set; }
    }

}
