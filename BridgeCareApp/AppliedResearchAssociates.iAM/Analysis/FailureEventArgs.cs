using System;

namespace AppliedResearchAssociates.iAM.Analysis
{
    public sealed class FailureEventArgs : EventArgs
    {
        public FailureEventArgs(string message) => Message = message;

        public string Message { get; }
    }
}
