using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace AppliedResearchAssociates.CalculateEvaluate;

internal sealed class CalculateEvaluateCompilerVisitor : CalculateEvaluateParserBaseVisitor<Expression>
{
    private static readonly IReadOnlyDictionary<(string, int), MethodInfo> MethodPerSignature;

    private static readonly ValueTypeInfo Number = ValueTypeInfo.Create(nameof(CalculateEvaluateScope.GetNumber), double.Parse);

    private static readonly ParameterExpression ScopeParameter = Expression.Parameter(typeof(CalculateEvaluateScope), "scope");

    private static readonly MethodInfo StringComparerEquals = typeof(StringComparer).GetMethod(nameof(StringComparer.Equals), new[] { typeof(string), typeof(string) });

    private static readonly Expression StringComparerOrdinalIgnoreCase = Expression.Constant(StringComparer.OrdinalIgnoreCase);

    private static readonly MethodInfo StringTrim = typeof(string).GetMethod(nameof(string.Trim), Type.EmptyTypes);

    private static readonly ValueTypeInfo Text = ValueTypeInfo.Create(nameof(CalculateEvaluateScope.GetText), static _ => _);

    private static readonly ValueTypeInfo Timestamp = ValueTypeInfo.Create(nameof(CalculateEvaluateScope.GetTimestamp), Convert.ToDateTime);

    private readonly IReadOnlyDictionary<string, CalculateEvaluateParameterType> ParameterTypes;

    private CalculateEvaluateParameterType? ComparisonOperandType;

    static CalculateEvaluateCompilerVisitor()
    {
        var mathMethods = typeof(Math).GetMethods(BindingFlags.Public | BindingFlags.Static);
        var numberMethods = mathMethods.Where(method => method.ReturnType == typeof(double) && method.GetParameters().All(parameter => parameter.ParameterType == typeof(double))).ToArray();
        NumberFunctionDescriptions = numberMethods.Select(method => new NumberFunctionDescription(method.Name, method.GetParameters().Select(parameter => parameter.Name))).ToArray();
        MethodPerSignature = numberMethods.ToDictionary(method => (method.Name, method.GetParameters().Length), ValueTupleEqualityComparer.Create<string, int>(StringComparer.OrdinalIgnoreCase));
    }

    public CalculateEvaluateCompilerVisitor(IReadOnlyDictionary<string, CalculateEvaluateParameterType> parameterTypes) => ParameterTypes = parameterTypes ?? throw new ArgumentNullException(nameof(parameterTypes));

    public static IReadOnlyDictionary<string, double> NumberConstants { get; } = GetNumberConstants();

    public static IReadOnlyCollection<NumberFunctionDescription> NumberFunctionDescriptions { get; }

    public SortedSet<string> ReferencedParameters { get; } = new SortedSet<string>(StringComparer.OrdinalIgnoreCase);

    private static IReadOnlyDictionary<string, double> GetNumberConstants()
    {
        var mathFields = typeof(Math).GetFields(BindingFlags.Public | BindingFlags.Static);
        var numberFields = mathFields.Where(field => field.FieldType == typeof(double));
        var result = numberFields.ToDictionary(field => field.Name, field => (double)field.GetValue(null), StringComparer.OrdinalIgnoreCase);
        return result;
    }

    private static CalculateEvaluateCompilationException UnknownReference(string identifierText) => new($"Unknown reference \"{identifierText}\".");

    private static ValueTypeInfo GetValueTypeInfo(CalculateEvaluateParameterType valueType) => valueType switch
    {
        CalculateEvaluateParameterType.Number => Number,
        CalculateEvaluateParameterType.Text => Text,
        CalculateEvaluateParameterType.Timestamp => Timestamp,
        _ => throw new Exception("Invalid value type.")
    };

    private sealed record ValueTypeInfo(MethodInfo GetterMethod, Func<string, ConstantExpression> ParseLiteral)
    {
        public static ValueTypeInfo Create<T>(string getterName, Func<string, T> parse)
        {
            var getterMethod = typeof(CalculateEvaluateScope).GetMethod(getterName);

            return new ValueTypeInfo(getterMethod, literal =>
            {
                T value;

                try
                {
                    value = parse(literal);
                }
                catch (Exception e)
                {
                    throw new CalculateEvaluateCompilationException($"Failed to parse literal \"{literal}\".", e);
                }

                return Expression.Constant(value);
            });
        }
    }

    #region "Calculate"

    public override Expression VisitAdditionOrSubtraction(CalculateEvaluateParser.AdditionOrSubtractionContext context)
    {
        var leftOperand = Visit(context.left);
        var rightOperand = Visit(context.right);

        var result = context.operation.Type switch
        {
            CalculateEvaluateLexer.PLUS => Expression.AddChecked(leftOperand, rightOperand),
            CalculateEvaluateLexer.MINUS => Expression.SubtractChecked(leftOperand, rightOperand),
            _ => throw new InvalidOperationException("Operation is neither addition nor subtraction."),
        };

        return result;
    }

    public override Expression VisitCalculationGrouping(CalculateEvaluateParser.CalculationGroupingContext context)
    {
        var result = Visit(context.calculation());
        return result;
    }

    public override Expression VisitCalculationRoot(CalculateEvaluateParser.CalculationRootContext context)
    {
        var body = Visit(context.calculation());
        var lambda = Expression.Lambda<CalculateEvaluateDelegate<double>>(body, ScopeParameter);
        return lambda;
    }

    public override Expression VisitInvocation(CalculateEvaluateParser.InvocationContext context)
    {
        var identifierText = context.IDENTIFIER().GetText();
        var arguments = context.arguments().calculation();

        if (!MethodPerSignature.TryGetValue((identifierText, arguments.Length), out var method))
        {
            throw new CalculateEvaluateCompilationException($"Unknown function \"{identifierText}\" or invalid number of arguments.");
        }

        var result = Expression.Call(method, arguments.Select(Visit));
        return result;
    }

    public override Expression VisitMultiplicationOrDivision(CalculateEvaluateParser.MultiplicationOrDivisionContext context)
    {
        var leftOperand = Visit(context.left);
        var rightOperand = Visit(context.right);

        Expression result = context.operation.Type switch
        {
            CalculateEvaluateLexer.TIMES => Expression.MultiplyChecked(leftOperand, rightOperand),
            CalculateEvaluateLexer.DIVIDED_BY => Expression.Divide(leftOperand, rightOperand),
            _ => throw new InvalidOperationException("Operation is neither multiplication nor division."),
        };

        return result;
    }

    public override Expression VisitNegation(CalculateEvaluateParser.NegationContext context)
    {
        var operand = Visit(context.calculation());
        var result = Expression.NegateChecked(operand);
        return result;
    }

    public override Expression VisitNumberLiteral(CalculateEvaluateParser.NumberLiteralContext context)
    {
        var numberText = context.NUMBER().GetText();
        var result = Number.ParseLiteral(numberText);
        return result;
    }

    public override Expression VisitNumberParameterReference(CalculateEvaluateParser.NumberParameterReferenceContext context)
    {
        var identifierText = context.IDENTIFIER().GetText();

        if (!TryVisitNumberParameterReference(identifierText, out var result))
        {
            throw UnknownReference(identifierText);
        }

        return result;
    }

    public override Expression VisitNumberReference(CalculateEvaluateParser.NumberReferenceContext context)
    {
        var identifierText = context.IDENTIFIER().GetText();

        Expression result;
        if (NumberConstants.TryGetValue(identifierText, out var constant))
        {
            result = Expression.Constant(constant);
        }
        else if (!TryVisitNumberParameterReference(identifierText, out result))
        {
            throw UnknownReference(identifierText);
        }

        return result;
    }

    private bool TryVisitNumberParameterReference(string identifierText, out Expression result)
    {
        if (!ParameterTypes.TryGetValue(identifierText, out var parameterType))
        {
            result = default;
            return false;
        }

        if (parameterType != CalculateEvaluateParameterType.Number)
        {
            throw new CalculateEvaluateCompilationException($"Parameter \"{identifierText}\" is not a number.");
        }

        _ = ReferencedParameters.Add(identifierText);

        var identifierString = Expression.Constant(identifierText);
        result = Expression.Call(ScopeParameter, Number.GetterMethod, identifierString);
        return true;
    }

    #endregion "Calculate"

    #region "Evaluate"

    public override Expression VisitEqual(CalculateEvaluateParser.EqualContext context)
    {
        var result = GetComparisonExpression(
            context.left,
            context.right,
            Expression.Equal,
            GetCaseInsensitiveTextEqualityComparison_Equal);

        return result;
    }

    public override Expression VisitEvaluationGrouping(CalculateEvaluateParser.EvaluationGroupingContext context)
    {
        var result = Visit(context.evaluation());
        return result;
    }

    public override Expression VisitEvaluationRoot(CalculateEvaluateParser.EvaluationRootContext context)
    {
        var body = Visit(context.evaluation());
        var lambda = Expression.Lambda<CalculateEvaluateDelegate<bool>>(body, ScopeParameter);
        return lambda;
    }

    public override Expression VisitGreaterThan(CalculateEvaluateParser.GreaterThanContext context)
    {
        var result = GetComparisonExpression(
            context.left,
            context.right,
            Expression.GreaterThan);

        return result;
    }

    public override Expression VisitGreaterThanOrEqual(CalculateEvaluateParser.GreaterThanOrEqualContext context)
    {
        var result = GetComparisonExpression(
            context.left,
            context.right,
            Expression.GreaterThanOrEqual);

        return result;
    }

    public override Expression VisitLessThan(CalculateEvaluateParser.LessThanContext context)
    {
        var result = GetComparisonExpression(
            context.left,
            context.right,
            Expression.LessThan);

        return result;
    }

    public override Expression VisitLessThanOrEqual(CalculateEvaluateParser.LessThanOrEqualContext context)
    {
        var result = GetComparisonExpression(
            context.left,
            context.right,
            Expression.LessThanOrEqual);

        return result;
    }

    public override Expression VisitLogicalConjunction(CalculateEvaluateParser.LogicalConjunctionContext context)
    {
        var leftOperand = Visit(context.left);
        var rightOperand = Visit(context.right);
        var result = Expression.AndAlso(leftOperand, rightOperand);
        return result;
    }

    public override Expression VisitLogicalDisjunction(CalculateEvaluateParser.LogicalDisjunctionContext context)
    {
        var leftOperand = Visit(context.left);
        var rightOperand = Visit(context.right);
        var result = Expression.OrElse(leftOperand, rightOperand);
        return result;
    }

    public override Expression VisitNotEqual(CalculateEvaluateParser.NotEqualContext context)
    {
        var result = GetComparisonExpression(
            context.left,
            context.right,
            Expression.NotEqual,
            GetCaseInsensitiveTextEqualityComparison_NotEqual);

        return result;
    }

    public override Expression VisitParameterReference(CalculateEvaluateParser.ParameterReferenceContext context)
    {
        var innerParameterReference = context.parameterReference();
        if (innerParameterReference is not null)
        {
            return VisitParameterReference(innerParameterReference);
        }

        var identifierText = context.IDENTIFIER().GetText();

        if (!ParameterTypes.TryGetValue(identifierText, out var parameterType))
        {
            throw UnknownReference(identifierText);
        }

        ComparisonOperandType = parameterType;
        _ = ReferencedParameters.Add(identifierText);

        var getterMethod = GetValueTypeInfo(parameterType).GetterMethod;
        var identifier = Expression.Constant(identifierText);
        var parameterReference = Expression.Call(ScopeParameter, getterMethod, identifier);
        return parameterReference;
    }

    private static Expression GetCaseInsensitiveTextEqualityComparison_Equal(Expression string1, Expression string2)
    {
        string1 = Expression.Call(string1, StringTrim);
        string2 = Expression.Call(string2, StringTrim);
        return Expression.Call(StringComparerOrdinalIgnoreCase, StringComparerEquals, string1, string2);
    }

    private static Expression GetCaseInsensitiveTextEqualityComparison_NotEqual(Expression string1, Expression string2)
    {
        var equalityExpression = GetCaseInsensitiveTextEqualityComparison_Equal(string1, string2);
        return Expression.Not(equalityExpression);
    }

    private Expression GetComparisonExpression(
        CalculateEvaluateParser.ComparisonOperandContext left,
        CalculateEvaluateParser.ComparisonOperandContext right,
        Func<Expression, Expression, Expression> getComparisonExpression,
        Func<Expression, Expression, Expression> getTextComparisonExpression = null)
    {
        var getLeftOperand = HandleComparisonOperand(left);
        var leftOperandType = ComparisonOperandType;
        var getRightOperand = HandleComparisonOperand(right);
        var rightOperandType = ComparisonOperandType;

        if (!leftOperandType.HasValue && !rightOperandType.HasValue)
        {
            throw new CalculateEvaluateCompilationException("Comparison operands are both literals.");
        }

        CalculateEvaluateParameterType operandType = default;

        if (leftOperandType.HasValue && !rightOperandType.HasValue)
        {
            operandType = leftOperandType.Value;
        }
        else if (!leftOperandType.HasValue && rightOperandType.HasValue)
        {
            operandType = rightOperandType.Value;
        }
        else if (leftOperandType.Value != rightOperandType.Value)
        {
            throw new CalculateEvaluateCompilationException("Comparison operands are not the same type.");
        }

        if (operandType == CalculateEvaluateParameterType.Text)
        {
            getComparisonExpression = getTextComparisonExpression
                ?? throw new CalculateEvaluateCompilationException("Ordering comparison is not supported for text operands.");
        }

        var leftOperand = getLeftOperand(operandType);
        var rightOperand = getRightOperand(operandType);
        var comparisonExpression = getComparisonExpression(leftOperand, rightOperand);
        return comparisonExpression;
    }

    private Func<CalculateEvaluateParameterType, Expression> HandleComparisonOperand(CalculateEvaluateParser.ComparisonOperandContext context)
    {
        var parameterReference = context.parameterReference();
        var calculation = context.calculation();
        var literal = context.literal();

        if (parameterReference is not null)
        {
            var expression = VisitParameterReference(parameterReference);
            return _ => expression;
        }

        if (calculation is not null)
        {
            ComparisonOperandType = CalculateEvaluateParameterType.Number;
            var expression = Visit(calculation);
            return _ => expression;
        }

        ComparisonOperandType = null;
        var literalText = literal.content?.Text ?? "";
        return valueType => GetValueTypeInfo(valueType).ParseLiteral(literalText);
    }

    #endregion "Evaluate"
}
