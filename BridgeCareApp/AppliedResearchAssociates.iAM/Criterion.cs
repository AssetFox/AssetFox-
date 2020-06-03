﻿using System;
using AppliedResearchAssociates.CalculateEvaluate;

namespace AppliedResearchAssociates.iAM
{
    public sealed class Criterion : CompilableExpression
    {
        public Criterion(Explorer explorer) => Explorer = explorer ?? throw new ArgumentNullException(nameof(explorer));

        public bool? Evaluate(CalculateEvaluateArgument argument)
        {
            EnsureCompiled();
            return Evaluator?.Invoke(argument);
        }

        public bool EvaluateOrDefault(CalculateEvaluateArgument argument) => Evaluate(argument) ?? true;

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
