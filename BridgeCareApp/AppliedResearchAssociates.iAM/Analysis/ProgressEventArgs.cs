using System;

namespace AppliedResearchAssociates.iAM.Analysis
{
    public class ProgressEventArgs : EventArgs
    {
        public ProgressEventArgs(ProgressStatus progressStatus, double percentComplete = 0, int? year = null)
        {
            ProgressStatus = progressStatus;
            Year = year;
            PercentComplete = percentComplete;
        }

        public ProgressStatus ProgressStatus { get; }

        public int? Year { get; }

        public double PercentComplete { get; }
    }
}
