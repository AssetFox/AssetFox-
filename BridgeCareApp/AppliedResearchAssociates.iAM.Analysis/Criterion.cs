using System;
using AppliedResearchAssociates.CalculateEvaluate;
using AppliedResearchAssociates.iAM.Analysis.Engine;

namespace AppliedResearchAssociates.iAM.Analysis;

public sealed class Criterion : CompilableExpression
{
    internal Criterion(Explorer explorer) => Explorer = explorer ?? throw new ArgumentNullException(nameof(explorer));

    public bool? Evaluate(CalculateEvaluateScope scope)
    {
        EnsureCompiled();
        return Evaluator?.Delegate(scope);
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
