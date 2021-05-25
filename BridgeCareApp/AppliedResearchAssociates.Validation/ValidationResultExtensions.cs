using System;
using System.Collections.Generic;
using System.Text;

namespace AppliedResearchAssociates.Validation
{
    public static class ValidationResultExtensions
    {
        
        public static string LogEntryPrefix(this ValidationResult result)
        {
            var maxLength = $"information: ".Length;
            var r = $"{result.Status}:";
            return r.PadRight(maxLength);
        }
        public static string ToLogEntry(this ValidationResult result)
        {
            var prefix = result.LogEntryPrefix();
            return $"{prefix}{result.Message}";
        }
    }
}
