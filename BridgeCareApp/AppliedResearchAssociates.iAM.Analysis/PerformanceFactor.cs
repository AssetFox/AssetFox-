using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAM.Analysis
{
    public class PerformanceFactor : WeakEntity, IValidator
    {
        internal PerformanceFactor(Explorer explorer)
        {
        }

        public string Attribute { get; set; }

        public float Factor { get; set; }

        public ValidatorBag Subvalidators => new ValidatorBag();

        public ValidationResultBag GetDirectValidationResults() => new ValidationResultBag();

        public string ShortDescription => nameof(PerformanceFactor);
    }
}
