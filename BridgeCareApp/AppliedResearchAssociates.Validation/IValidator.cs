using System.Collections.Generic;

namespace AppliedResearchAssociates.Validation
{
    public interface IValidator
    {
        ValidatorBag Subvalidators { get; }

        ValidationResultBag GetDirectValidationResults();
        /// <summary>This finds its way into the validation log. It should
        /// be something that helps the user find what it is they need to fix.</summary>
        string ShortDescription { get; }
    }
}
