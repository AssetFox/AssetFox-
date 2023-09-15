using System;

namespace AppliedResearchAssociates.iAM.Analysis.Engine;

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

    public override string ToString()
    {
        var result = $"Analysis progress: {PercentComplete:f0}% ({ProgressStatus})";
        if (Year.HasValue)
        {
            result += $", year {Year}.";
        }
        else
        {
            result += ".";
        }

        return result;
    }
}
