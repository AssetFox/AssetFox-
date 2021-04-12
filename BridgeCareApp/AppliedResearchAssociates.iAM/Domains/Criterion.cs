using System;
using AppliedResearchAssociates.CalculateEvaluate;
using AppliedResearchAssociates.iAM.Analysis;

namespace AppliedResearchAssociates.iAM.Domains
{
    public sealed class Criterion : CompilableExpression
    {
        public Criterion(Explorer explorer) => Explorer = explorer ?? throw new ArgumentNullException(nameof(explorer));

        internal bool? Evaluate(SectionContext scope)
        {
            EnsureCompiled();
            return Evaluator?.Delegate(scope);
        }

        internal bool EvaluateOrDefault(SectionContext scope) => Evaluate(scope) ?? true;

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
