using System.Collections.Generic;

namespace AppliedResearchAssociates.CalculateEvaluate
{
    public delegate T CalculateEvaluateDelegate<T>(CalculateEvaluateScope scope);

    public class CalculateEvaluateDelegateWrapper<T>
    {
        public CalculateEvaluateDelegate<T> Delegate { get; internal set; }

        public IReadOnlyCollection<string> ReferencedParameters { get; internal set; }
    }
}
