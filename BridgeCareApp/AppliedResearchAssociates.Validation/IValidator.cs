using System.Collections.Generic;

namespace AppliedResearchAssociates.Validation
{
    public interface IValidator
    {
        ValidatorBag Subvalidators { get; }

        ValidationResultBag GetDirectValidationResults(List<string> validationPath);
    }
}
