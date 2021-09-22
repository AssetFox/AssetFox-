using System;

namespace AppliedResearchAssociates.iAM.Analysis.Engine
{
    public sealed class FailureEventArgs : EventArgs
    {
        public FailureEventArgs(string message) => Message = message;

        public string Message { get; }
    }
}
