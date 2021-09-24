﻿using System;

namespace AppliedResearchAssociates.iAM.Analysis.Engine
{
    public sealed class WarningEventArgs : EventArgs
    {
        public WarningEventArgs(string message) => Message = message;

        public string Message { get; }
    }
}