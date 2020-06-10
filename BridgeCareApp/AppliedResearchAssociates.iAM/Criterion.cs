using System;
using AppliedResearchAssociates.CalculateEvaluate;

namespace AppliedResearchAssociates.iAM
{
    public sealed class Criterion : CompilableExpression
    {
        public Criterion(Explorer explorer) => Explorer = explorer ?? throw new ArgumentNullException(nameof(explorer));

        public bool? Evaluate(CalculateEvaluateScope scope)
        {
            EnsureCompiled();
            return Evaluator?.Invoke(scope);
        }

        public bool EvaluateOrDefault(CalculateEvaluateScope scope) => Evaluate(scope) ?? true;

        protected override void Compile()
        {
            if (ExpressionIsBlank)
            {
                Evaluator = null;
                return;
            }

            try
            {
                Evaluator = Explorer.Compiler.GetEvaluator(Expression);
            }
            catch (CalculateEvaluateException e)
            {
                throw ExpressionCouldNotBeCompiled(e);
            }
        }

        private readonly Explorer Explorer;

        private Evaluator Evaluator;
    }
}
