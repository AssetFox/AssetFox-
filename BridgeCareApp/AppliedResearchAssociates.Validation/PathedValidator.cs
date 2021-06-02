using System;
using System.Collections.Generic;
using System.Text;

namespace AppliedResearchAssociates.Validation
{
    public class PathedValidator
    {
        public PathedValidator(IValidator validator, IEnumerable<string> validationPath)
        {
            Validator = validator;
            ValidationPath = validationPath;
        }
        public IValidator Validator { get; set; }
        public IEnumerable<string> ValidationPath { get; set; }
    }
}
