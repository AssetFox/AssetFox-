using System;
using System.Collections.Generic;
using System.Text;

namespace AppliedResearchAssociates.iAM.Analysis
{
    public class ProgressEventArgs : EventArgs
    {
        public ProgressEventArgs(ProgressStatus progressStatus, int year, double percentComplete) {
            ProgressStatus = progressStatus;
            Year = year;
            PercentComplete = percentComplete;
        }

        public ProgressStatus ProgressStatus { get; }
        public int Year { get; }
        public double PercentComplete { get; }
    }
}
