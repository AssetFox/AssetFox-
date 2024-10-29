using System;
using System.Linq;
using Xunit;

namespace AppliedResearchAssociates.CalculateEvaluate.Testing
{
    public class CalculateEvaluateCompilerTests
    {
        [Fact]
        public void BadLex() => Assert.Throws<CalculateEvaluateLexingException>(() => ParameterlessCalculation("2 # 2", 4));

        [Fact]
        public void BadParse() => Assert.Throws<CalculateEvaluateParsingException>(() => ParameterlessCalculation("2 ( 2", 4));

        #region "Calculate"

        [Fact]
        [Trait("Category", CATEGORY_CALCULATE)]
        public void Addition() => ParameterlessCalculation($"{n0} + {n1}", n0 + n1);

        [Fact]
        [Trait("Category", CATEGORY_CALCULATE)]
        public void CalculationAssociativity() => MultipleParameterCalculation($"2 + 3 * 4 + 5", 19);

        [Fact]
        [Trait("Category", CATEGORY_CALCULATE)]
        public void CalculationGrouping() => ParameterlessCalculation("2 * (3 + 4) * 5", 70);

        [Fact]
        [Trait("Category", CATEGORY_CALCULATE)]
        public void ConstantReference() => ParameterlessCalculation("pi", Math.PI);

        [Fact]
        [Trait("Category", CATEGORY_CALCULATE)]
        public void Division() => ParameterlessCalculation($"{n0} / {n1}", n0 / n1);

        [Fact]
        [Trait("Category", CATEGORY_CALCULATE)]
        public void Invocation() => ParameterlessCalculation($"log({n0})", Math.Log(n0));

        [Fact]
        [Trait("Category", CATEGORY_CALCULATE)]
        public void Multiplication() => ParameterlessCalculation($"{n0} * {n1}", n0 * n1);

        [Fact]
        [Trait("Category", CATEGORY_CALCULATE)]
        public void Negation() => ParameterlessCalculation($"-{n0}", -n0);

        [Fact]
        [Trait("Category", CATEGORY_CALCULATE)]
        public void NumberLiteral() => ParameterlessCalculation($"{n0}", n0);

        [Fact]
        [Trait("Category", CATEGORY_CALCULATE)]
        public void NumberParameterReference()
        {
            var compiler = new CalculateEvaluateCompiler();
            compiler.ParameterTypes["PARAM"] = CalculateEvaluateParameterType.Number;
            var expression = "[param] * param";
            var calculator = compiler.GetCalculator(expression);
            var scope = new CalculateEvaluateScope();
            scope.SetNumber("PaRaM", n0);
            var result = calculator.Delegate(scope);
            Assert.Equal(n0 * n0, result);
        }

        [Fact]
        [Trait("Category", CATEGORY_CALCULATE)]
        public void Subtraction() => ParameterlessCalculation($"{n0} - {n1}", n0 - n1);

        #endregion "Calculate"

        #region "Evaluate"

        [Fact]
        [Trait("Category", CATEGORY_EVALUATE)]
        public void CompareWithNumberOperand() => SingleNumberParameterEvaluation($"[param]={n1}", Assert.False);

        [Fact]
        [Trait("Category", CATEGORY_EVALUATE)]
        public void CompareWithReferenceOperand() => MultipleNumberParameterEvaluation($"[param0]=param1", Assert.False, n0, n1);

        [Fact]
        [Trait("Category", CATEGORY_EVALUATE)]
        public void EvaluationAssociativity() => MultipleNumberParameterEvaluation($"param0='{n0}' or [param0]='{n1}' and [param1]='{n0}'", Assert.True, n0, n1);

        [Fact]
        [Trait("Category", CATEGORY_EVALUATE)]
        public void EvaluationGrouping() => MultipleNumberParameterEvaluation($"(param0='{n0}' or [param0]='{n1}') and [param1]='{n0}'", Assert.False, n0, n1);

        [Fact]
        [Trait("Category", CATEGORY_EVALUATE)]
        public void LogicalConjunction() => MultipleNumberParameterEvaluation($"[param0]='{n0}' and param1='{n0}'", Assert.False, n0, n1);

        [Fact]
        [Trait("Category", CATEGORY_EVALUATE)]
        public void LogicalDisjunction() => MultipleNumberParameterEvaluation($"[param0]='{n0}' or param1='{n0}'", Assert.True, n0, n1);

        [Fact]
        [Trait("Category", CATEGORY_EVALUATE)]
        public void NumberEqual() => SingleNumberParameterEvaluation($"[param]='{n1}'", Assert.False);

        [Fact]
        [Trait("Category", CATEGORY_EVALUATE)]
        public void NumberGreaterThan() => SingleNumberParameterEvaluation($"[param]>|{n1}|", Assert.False);

        [Fact]
        [Trait("Category", CATEGORY_EVALUATE)]
        public void NumberGreaterThanOrEqual() => SingleNumberParameterEvaluation($"[param]>=|{n1}|", Assert.False);

        [Fact]
        [Trait("Category", CATEGORY_EVALUATE)]
        public void NumberLessThan() => SingleNumberParameterEvaluation($"[param]<|{n1}|", Assert.True);

        [Fact]
        [Trait("Category", CATEGORY_EVALUATE)]
        public void NumberLessThanOrEqual() => SingleNumberParameterEvaluation($"[param]<=|{n1}|", Assert.True);

        [Fact]
        [Trait("Category", CATEGORY_EVALUATE)]
        public void NumberNotEqual() => SingleNumberParameterEvaluation($"[param]<>|{n1}|", Assert.True);

        [Fact]
        [Trait("Category", CATEGORY_EVALUATE)]
        public void TextEqual() => SingleTextParameterEvaluation($"[param]=''", Assert.False);

        [Fact]
        [Trait("Category", CATEGORY_EVALUATE)]
        public void TextNotEqual() => SingleTextParameterEvaluation($"[param]<>|{s1}|", Assert.True);

        #endregion "Evaluate"

        private const string CATEGORY_CALCULATE = "Calculate";

        private const string CATEGORY_EVALUATE = "Evaluate";

        private const double n0 = 19.123, n1 = 23.456;

        private const string s0 = "foo", s1 = "bar";

        private static void MultipleNumberParameterEvaluation(string inputExpression, Action<bool> assert, params double[] parameterValues)
        {
            var parameters = parameterValues.Select(ValueTuple.Create<double, int>).ToArray();
            var compiler = new CalculateEvaluateCompiler();
            foreach (var (_, i) in parameters)
            {
                compiler.ParameterTypes["PARAM" + i] = CalculateEvaluateParameterType.Number;
            }
            var evaluator = compiler.GetEvaluator(inputExpression);
            var scope = new CalculateEvaluateScope();
            foreach (var (n, i) in parameters)
            {
                scope.SetNumber("PaRaM" + i, n);
            }
            var result = evaluator.Delegate(scope);
            assert(result);
        }

        private static void MultipleParameterCalculation(string inputExpression, double expectedOutput, params double[] parameterValues)
        {
            var parameters = parameterValues.Select(ValueTuple.Create<double, int>).ToArray();
            var compiler = new CalculateEvaluateCompiler();
            foreach (var (_, i) in parameters)
            {
                compiler.ParameterTypes["PARAM" + i] = CalculateEvaluateParameterType.Number;
            }
            var calculator = compiler.GetCalculator(inputExpression);
            var scope = new CalculateEvaluateScope();
            foreach (var (n, i) in parameters)
            {
                scope.SetNumber("PaRaM" + i, n);
            }
            var result = calculator.Delegate(scope);
            Assert.Equal(expectedOutput, result);
        }

        private static void ParameterlessCalculation(string inputExpression, double expectedOutput)
        {
            var compiler = new CalculateEvaluateCompiler();
            var calculator = compiler.GetCalculator(inputExpression);
            var result = calculator.Delegate(null);
            Assert.Equal(expectedOutput, result);
        }

        private static void SingleNumberParameterEvaluation(string inputExpression, Action<bool> assert)
        {
            var compiler = new CalculateEvaluateCompiler();
            compiler.ParameterTypes["PARAM"] = CalculateEvaluateParameterType.Number;
            var calculator = compiler.GetEvaluator(inputExpression);
            var scope = new CalculateEvaluateScope();
            scope.SetNumber("PaRaM", n0);
            var result = calculator.Delegate(scope);
            assert(result);
        }

        private static void SingleTextParameterEvaluation(string inputExpression, Action<bool> assert)
        {
            var compiler = new CalculateEvaluateCompiler();
            compiler.ParameterTypes["PARAM"] = CalculateEvaluateParameterType.Text;
            var calculator = compiler.GetEvaluator(inputExpression);
            var scope = new CalculateEvaluateScope();
            scope.SetText("PaRaM", s0);
            var result = calculator.Delegate(scope);
            assert(result);
        }
    }
}
