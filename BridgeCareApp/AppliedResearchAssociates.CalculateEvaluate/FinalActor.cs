using System;

namespace AppliedResearchAssociates.CalculateEvaluate
{
    internal sealed class FinalActor<T>
    {
        public FinalActor(T value, Action<FinalActor<T>> finalAction)
        {
            Value = value;
            FinalAction = finalAction;
        }

        public T Value { get; }

        private readonly Action<FinalActor<T>> FinalAction;

        ~FinalActor()
        {
            FinalAction(this);
        }
    }
}
