namespace BridgeCare.Models
{
    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public string ValidationMessage { get; set; }

        public ValidationResult() {}

        public ValidationResult(bool isValid, string validationMessage)
        {
            IsValid = isValid;
            ValidationMessage = validationMessage;
        }
    }
}
