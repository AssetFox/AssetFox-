using System.Collections.Generic;
using System.Linq;

namespace AppliedResearchAssociates.Validation
{
    public static class Validation
    {
        public static ValidationResultBag GetAllValidationResults(this IValidator validator, IEnumerable<string> validationPath)
        {
            var results = new ValidationResultBag();

            var visited = new HashSet<IValidator>();
            var queue = new Queue<PathedValidator>();
            var initialPathedValidator = new PathedValidator(validator, validationPath);
            queue.Enqueue(initialPathedValidator);
            while (queue.Count > 0)
            {
                var pathedValidator = queue.Dequeue();
                var nextValidator = pathedValidator.Validator;
                if (nextValidator != null && visited.Add(nextValidator))
                {
                    results.Add(nextValidator.GetDirectValidationResults(pathedValidator.ValidationPath));
                    foreach (var subvalidator in nextValidator.Subvalidators)
                    {
                        var subpath = pathedValidator.ValidationPath.Append(subvalidator.ShortDescription);
                        var pathedSubvalidator = new PathedValidator(subvalidator, subpath);
                        queue.Enqueue(pathedSubvalidator);
                    }
                }
            }

            return results;
        }
    }
}
