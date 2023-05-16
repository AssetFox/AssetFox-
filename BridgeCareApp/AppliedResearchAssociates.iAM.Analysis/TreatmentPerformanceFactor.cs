using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAM.Analysis
{
    public sealed class TreatmentPerformanceFactor : PerformanceFactor
    {
        internal TreatmentPerformanceFactor(Explorer explorer) : base(explorer)
        {
            if (explorer == null)
            {
                throw new ArgumentNullException(nameof(explorer));
            }
        }
    }
}
