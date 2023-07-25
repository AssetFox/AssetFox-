using System;
using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAM.Analysis;

public abstract class CompilableExpression : WeakEntity, IValidator
{
    public string Expression
    {
        get => _Expression ?? string.Empty;
        set
        {
            if (Expression != value)
            {
                _Expression = value;
                _EnsureCompiled = _Compile;
            }
        }
    }

    public bool ExpressionIsBlank => string.IsNullOrWhiteSpace(Expression);

    public virtual ValidatorBag Subvalidators => new ValidatorBag();

    public virtual ValidationResultBag GetDirectValidationResults()
    {
        var results = new ValidationResultBag();

        try
        {
            EnsureCompiled();
        }
        catch (MalformedInputException e)
        {
            results.Add(ValidationStatus.Error, e.Message, this, nameof(Expression));
        }

        return results;
    }

    protected CompilableExpression() => _EnsureCompiled = _Compile;

    protected static MalformedInputException ExpressionCouldNotBeCompiled() => new MalformedInputException(EXPRESSION_COULD_NOT_BE_COMPILED);

    protected static MalformedInputException ExpressionCouldNotBeCompiled(Exception innerException) => new MalformedInputException(EXPRESSION_COULD_NOT_BE_COMPILED, innerException);

    protected static MalformedInputException ExpressionCouldNotBeCompiled(string message) => new MalformedInputException(EXPRESSION_COULD_NOT_BE_COMPILED + " " + message);

    protected abstract void Compile();

    protected void EnsureCompiled() => _EnsureCompiled?.Invoke();

    private const string EXPRESSION_COULD_NOT_BE_COMPILED = "Expression could not be compiled.";

    private Action _EnsureCompiled;

    private string _Expression;

    private void _Compile()
    {
        Compile();
        _EnsureCompiled = null;
    }

    public string ShortDescription => _Expression ?? "Uncompiled expression";
}
