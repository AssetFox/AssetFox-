using AppliedResearchAssociates.Validation;
using System;

namespace BridgeCareCore.Models.Validation
{
    public sealed class PreChecksValidationResult
    {
        public PreChecksValidationResult(
            ValidationStatus status,
            string message
            )
        {
            Status = status;

            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentException("Description is blank.", nameof(message));
            }

            Message = message;
        }

        public string Message { get; }

        public ValidationStatus Status { get; }       
    }
}
