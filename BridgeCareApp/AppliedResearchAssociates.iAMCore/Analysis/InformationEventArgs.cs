﻿using System;

namespace AppliedResearchAssociates.iAMCore.Analysis
{
    public sealed class InformationEventArgs : EventArgs
    {
        public InformationEventArgs(string message) => Message = message;

        public string Message { get; }
    }
}
