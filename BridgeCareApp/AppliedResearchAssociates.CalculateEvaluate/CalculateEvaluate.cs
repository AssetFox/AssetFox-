using System;

namespace AppliedResearchAssociates.CalculateEvaluate
{
    internal static class CalculateEvaluate
    {
        public static WeakReference<T> GetWeakReference<T>(this T reference) where T : class => new WeakReference<T>(reference);

        public static FinalActor<T> WithFinalAction<T>(this T value, Action<FinalActor<T>> finalAction) => new FinalActor<T>(value, finalAction ?? throw new ArgumentNullException(nameof(finalAction)));
    }
}
