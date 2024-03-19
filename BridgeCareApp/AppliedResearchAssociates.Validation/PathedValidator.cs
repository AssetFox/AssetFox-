using System.Collections.Generic;

namespace AppliedResearchAssociates.Validation
{
    public class PathedValidator
    {
        public PathedValidator(IValidator validator, IEnumerable<string> validationPath)
        {
            Validator = validator;
            ValidationPath = validationPath;
        }

        public IEnumerable<string> ValidationPath { get; set; }

        public IValidator Validator { get; set; }
    }
}
