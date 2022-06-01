using System.Text.Json;

namespace AppliedResearchAssociates.iAM.Reporting.Logging
{
    public class ErrorDetails
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public override string ToString() => JsonSerializer.Serialize(this);
    }
}
