namespace BridgeCare.Models
{
    public class CriterionValidationResult : ValidationResult
    {
        public int ResultsCount { get; set; }

        public CriterionValidationResult() {}

        public CriterionValidationResult(bool isValid, int resultsCount, string validationMessage) : base(isValid, validationMessage) =>
            ResultsCount = resultsCount;
    }
}
