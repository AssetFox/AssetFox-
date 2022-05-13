using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace AppliedResearchAssociates.CalculateEvaluate
{
    internal sealed class CalculateEvaluateCompilerVisitor : CalculateEvaluateParserBaseVisitor<Expression>
    {
        public CalculateEvaluateCompilerVisitor(IReadOnlyDictionary<string, CalculateEvaluateParameterType> parameterTypes) => ParameterTypes = parameterTypes ?? throw new ArgumentNullException(nameof(parameterTypes));

        public SortedSet<string> ReferencedParameters { get; } = new SortedSet<string>(StringComparer.OrdinalIgnoreCase);

        #region "Calculate"

        static CalculateEvaluateCompilerVisitor()
        {
            var mathMethods = typeof(Math).GetMethods(BindingFlags.Public | BindingFlags.Static);
            var numberMethods = mathMethods.Where(method => method.ReturnType == typeof(double) && method.GetParameters().All(parameter => parameter.ParameterType == typeof(double))).ToArray();
            NumberFunctionDescriptions = numberMethods.Select(method => new NumberFunctionDescription(method.Name, method.GetParameters().Select(parameter => parameter.Name))).ToArray();
            MethodPerSignature = numberMethods.ToDictionary(method => (method.Name, method.GetParameters().Length), ValueTupleEqualityComparer.Create<string, int>(StringComparer.OrdinalIgnoreCase));
        }

        public static IReadOnlyDictionary<string, double> NumberConstants { get; } = GetNumberConstants();

        public static IReadOnlyCollection<NumberFunctionDescription> NumberFunctionDescriptions { get; }

        public override Expression VisitAdditionOrSubtraction(CalculateEvaluateParser.AdditionOrSubtractionContext context)
        {
            var leftOperand = Visit(context.left);
            var rightOperand = Visit(context.right);

            Expression result;
            switch (context.operation.Type)
            {
            case CalculateEvaluateLexer.PLUS:
                result = Expression.AddChecked(leftOperand, rightOperand);
                break;

            case CalculateEvaluateLexer.MINUS:
                result = Expression.SubtractChecked(leftOperand, rightOperand);
                break;

            default:
                throw new InvalidOperationException("Operation is neither addition nor subtraction.");
            }

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

            Expression result;
            switch (context.operation.Type)
            {
            case CalculateEvaluateLexer.TIMES:
                result = Expression.MultiplyChecked(leftOperand, rightOperand);
                break;

            case CalculateEvaluateLexer.DIVIDED_BY:
                result = Expression.Divide(leftOperand, rightOperand);
                break;

            default:
                throw new InvalidOperationException("Operation is neither multiplication nor division.");
            }

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

        private static readonly IReadOnlyDictionary<(string, int), MethodInfo> MethodPerSignature;

        private static IReadOnlyDictionary<string, double> GetNumberConstants()
        {
            var mathFields = typeof(Math).GetFields(BindingFlags.Public | BindingFlags.Static);
            var numberFields = mathFields.Where(field => field.FieldType == typeof(double));
            var result = numberFields.ToDictionary(field => field.Name, field => (double)field.GetValue(null), StringComparer.OrdinalIgnoreCase);
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
            result = Expression.Call(ScopeParameter, Number.GetterInfo, identifierString);
            return true;
        }

        #endregion "Calculate"

        #region "Evaluate"

        public override Expression VisitEqual(CalculateEvaluateParser.EqualContext context)
        {
            var result = GetComparisonExpression(context.parameterReference(), context.comparisonOperand(), Expression.Equal, GetCaseInsensitiveTextEqualityComparison_Equal);
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
            var result = GetComparisonExpression(context.parameterReference(), context.comparisonOperand(), Expression.GreaterThan);
            return result;
        }

        public override Expression VisitGreaterThanOrEqual(CalculateEvaluateParser.GreaterThanOrEqualContext context)
        {
            var result = GetComparisonExpression(context.parameterReference(), context.comparisonOperand(), Expression.GreaterThanOrEqual);
            return result;
        }

        public override Expression VisitLessThan(CalculateEvaluateParser.LessThanContext context)
        {
            var result = GetComparisonExpression(context.parameterReference(), context.comparisonOperand(), Expression.LessThan);
            return result;
        }

        public override Expression VisitLessThanOrEqual(CalculateEvaluateParser.LessThanOrEqualContext context)
        {
            var result = GetComparisonExpression(context.parameterReference(), context.comparisonOperand(), Expression.LessThanOrEqual);
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
            var result = GetComparisonExpression(context.parameterReference(), context.comparisonOperand(), Expression.NotEqual, GetCaseInsensitiveTextEqualityComparison_NotEqual);
            return result;
        }

        private static readonly MethodInfo StringComparerEquals = typeof(StringComparer).GetMethod(nameof(StringComparer.Equals), new[] { typeof(string), typeof(string) });

        private static readonly Expression StringComparerOrdinalIgnoreCase = Expression.Constant(StringComparer.OrdinalIgnoreCase);

        private static readonly MethodInfo StringTrim = typeof(string).GetMethod(nameof(string.Trim), Type.EmptyTypes);

        private static Expression GetCaseInsensitiveTextEqualityComparison_Equal(Expression string1, Expression string2)
        {
            string1 = Expression.Call(string1, StringTrim);
            string2 = Expression.Call(string2, StringTrim);
            return Expression.Call(StringComparerOrdinalIgnoreCase, StringComparerEquals, string1, string2);
        }

        private static Expression GetCaseInsensitiveTextEqualityComparison_NotEqual(Expression string1, Expression string2) => Expression.Not(GetCaseInsensitiveTextEqualityComparison_Equal(string1, string2));

        private Expression GetComparisonExpression(CalculateEvaluateParser.ParameterReferenceContext parameterReference, CalculateEvaluateParser.ComparisonOperandContext comparisonOperand, Func<Expression, Expression, Expression> getDefaultComparison, Func<Expression, Expression, Expression> getTextComparison = null)
        {
            var identifierText = parameterReference.IDENTIFIER().GetText();

            if (!ParameterTypes.TryGetValue(identifierText, out var parameterType))
            {
                throw UnknownReference(identifierText);
            }

            ScopeInfo argumentInfo;
            switch (parameterType)
            {
            case CalculateEvaluateParameterType.Number:
                argumentInfo = Number;
                break;

            case CalculateEvaluateParameterType.Text:
                if (getTextComparison is null)
                {
                    throw new CalculateEvaluateCompilationException("Ordering comparisons do not support text parameters.");
                }
                argumentInfo = Text;
                break;

            case CalculateEvaluateParameterType.Timestamp:
                argumentInfo = Timestamp;
                break;

            default:
                throw new InvalidOperationException("Invalid parameter type.");
            }

            _ = ReferencedParameters.Add(identifierText);

            var identifier = Expression.Constant(identifierText);
            var reference = Expression.Call(ScopeParameter, argumentInfo.GetterInfo, identifier);

            var referenceOperand = comparisonOperand.parameterReference();
            var literalOperand = comparisonOperand.literal();
            var numberOperand = comparisonOperand.NUMBER();
            Expression operand;

            if (referenceOperand != null && literalOperand == null && numberOperand == null)
            {
                var operandIdentifierText = referenceOperand.IDENTIFIER().GetText();

                if (!ParameterTypes.TryGetValue(operandIdentifierText, out var operandType))
                {
                    throw UnknownReference(operandIdentifierText);
                }

                if (operandType != parameterType)
                {
                    throw new CalculateEvaluateCompilationException("Comparison types do not match.");
                }

                _ = ReferencedParameters.Add(operandIdentifierText);

                var operandIdentifier = Expression.Constant(operandIdentifierText);
                operand = Expression.Call(ScopeParameter, argumentInfo.GetterInfo, operandIdentifier);
            }
            else if (referenceOperand == null && literalOperand != null && numberOperand == null)
            {
                var literalContent = literalOperand.content?.Text ?? "";
                operand = argumentInfo.ParseLiteral(literalContent);
            }
            else if (referenceOperand == null && literalOperand == null && numberOperand != null)
            {
                if (parameterType != CalculateEvaluateParameterType.Number)
                {
                    throw new CalculateEvaluateCompilationException("Non-number comparison operand is a number.");
                }

                var numberText = numberOperand.GetText();
                operand = argumentInfo.ParseLiteral(numberText);
            }
            else
            {
                throw new InvalidOperationException("Comparison operand context does not have exactly one sub-context.");
            }

            var getComparison = parameterType == CalculateEvaluateParameterType.Text ? getTextComparison : getDefaultComparison;
            var result = getComparison(reference, operand);
            return result;
        }

        #endregion "Evaluate"

        private static readonly ScopeInfo Number = GetScopeInfo(nameof(CalculateEvaluateScope.GetNumber), double.Parse);

        private static readonly ParameterExpression ScopeParameter = Expression.Parameter(typeof(CalculateEvaluateScope), "scope");

        private static readonly ScopeInfo Text = GetScopeInfo(nameof(CalculateEvaluateScope.GetText), _ => _);

        private static readonly ScopeInfo Timestamp = GetScopeInfo(nameof(CalculateEvaluateScope.GetTimestamp), Convert.ToDateTime);

        private readonly IReadOnlyDictionary<string, CalculateEvaluateParameterType> ParameterTypes;

        private static CalculateEvaluateCompilationException UnknownReference(string identifierText) => new CalculateEvaluateCompilationException($"Unknown reference \"{identifierText}\".");

        private static ScopeInfo GetScopeInfo<T>(string getterName, Func<string, T> parse)
        {
            var getterInfo = typeof(CalculateEvaluateScope).GetMethod(getterName);
            return new ScopeInfo(getterInfo, literal =>
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

        private sealed class ScopeInfo
        {
            public ScopeInfo(MethodInfo getterInfo, Func<string, ConstantExpression> parseLiteral)
            {
                GetterInfo = getterInfo ?? throw new ArgumentNullException(nameof(getterInfo));
                ParseLiteral = parseLiteral ?? throw new ArgumentNullException(nameof(parseLiteral));
            }

            public MethodInfo GetterInfo { get; }

            public Func<string, ConstantExpression> ParseLiteral { get; }
        }
    }
}
