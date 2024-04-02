namespace BridgeCareCore.Models.Validation
{
    public class EquationValidationParameters : ValidationParameter
    {
        public bool IsPiecewise { get; set; }
        public bool IsAscendingAttribute { get; set; } = false;
    }
}
