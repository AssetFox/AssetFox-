using System;

namespace AppliedResearchAssociates.iAMCore.Analysis
{
    public sealed class FailureEventArgs : EventArgs
    {
        public FailureEventArgs(string message) => Message = message;

        public string Message { get; }
    }
}
