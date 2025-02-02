﻿using System;

namespace AppliedResearchAssociates.iAMCore.Analysis
{
    public sealed class WarningEventArgs : EventArgs
    {
        public WarningEventArgs(string message) => Message = message;

        public string Message { get; }
    }
}
