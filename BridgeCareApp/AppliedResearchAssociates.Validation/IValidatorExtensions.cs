using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AppliedResearchAssociates.Validation
{
    public static class IValidatorExtensions
    {
        public static ValidationResultBag GetDirectValidationResults(this IValidator validator, IEnumerable<string> validationPath)
        {
            var results = validator.GetDirectValidationResults();
            foreach (var result in results) {
                var path = validationPath.ToList();
                result.Target.ValidationPath = path;
            }
            return results;
        }
    }
}
