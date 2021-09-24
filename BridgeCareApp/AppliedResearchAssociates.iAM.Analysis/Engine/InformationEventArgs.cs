﻿using System;

namespace AppliedResearchAssociates.iAM.Analysis.Engine
{
    public sealed class InformationEventArgs : EventArgs
    {
        public InformationEventArgs(string message) => Message = message;

        public string Message { get; }
    }
}