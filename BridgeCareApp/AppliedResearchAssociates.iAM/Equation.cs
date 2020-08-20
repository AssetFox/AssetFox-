using System;
using System.Linq;
using System.Text.RegularExpressions;
using AppliedResearchAssociates.CalculateEvaluate;
using MathNet.Numerics.Interpolation;

namespace AppliedResearchAssociates.iAM
{
    public sealed class Equation : CompilableExpression
    {
        public Equation(Explorer explorer) => Explorer = explorer ?? throw new ArgumentNullException(nameof(explorer));

        public double Compute(CalculateEvaluateScope scope, string attributeNameForShift = null)
        {
            EnsureCompiled();

            if (ValueVersusAge != null)
            {
                var actualAge = scope.GetNumber(Explorer.AgeAttribute.Name);

                if (attributeNameForShift == null)
                {
                    return ValueVersusAge.Interpolate(actualAge);
                }
                else
                {
                    var previousValue = scope.GetNumber(attributeNameForShift);
                    var apparentAge = AgeVersusValue.Interpolate(previousValue);
                    var shiftedAge = actualAge * apparentAge / (actualAge - 1);
                    return ValueVersusAge.Interpolate(shiftedAge);
                }
            }

            if (Calculator != null)
            {
                return Calculator(scope);
            }

            throw new InvalidOperationException("Expression has not been compiled.");
        }

        protected override void Compile()
        {
            ValueVersusAge = null;
            AgeVersusValue = null;
            Calculator = null;

            if (ExpressionIsBlank)
            {
                throw ExpressionCouldNotBeCompiled("Expression is blank.");
            }

            var match = PiecewisePattern.Match(Expression);
            if (match.Success)
            {
                double parseValue(Capture capture) => double.TryParse(capture.Value, out var result) ? result : throw ExpressionCouldNotBeCompiled();

                double[] parseGroup(int index) => match.Groups[index].Captures.Cast<Capture>().Select(parseValue).ToArray();

                var xValues = parseGroup(1);
                var yValues = parseGroup(2);

                if (xValues.Length < 2)
                {
                    throw ExpressionCouldNotBeCompiled("Piecewise expression has less than two points.");
                }

                if (xValues.Distinct().Count() != xValues.Length)
                {
                    throw ExpressionCouldNotBeCompiled("Piecewise expression has non-unique x values.");
                }

                if (yValues.Distinct().Count() != yValues.Length)
                {
                    throw ExpressionCouldNotBeCompiled("Piecewise expression has non-unique y values.");
                }

                Array.Sort(xValues, yValues);

                if (!yValues.IsSorted())
                {
                    throw ExpressionCouldNotBeCompiled("Piecewise function is not monotone.");
                }

                ValueVersusAge = LinearSpline.InterpolateSorted(xValues, yValues);
                AgeVersusValue = LinearSpline.Interpolate(yValues, xValues);
            }
            else
            {
                try
                {
                    Calculator = Explorer.Compiler.GetCalculator(Expression);
                }
                catch (CalculateEvaluateException e)
                {
                    throw ExpressionCouldNotBeCompiled(e);
                }
            }
        }

        private static readonly Regex PiecewisePattern = new Regex($@"(?>\A\s*(?:\(\s*({PatternStrings.NaturalNumber})\s*,\s*({PatternStrings.Number})\s*\)\s*)*\z)", RegexOptions.Compiled);

        private readonly Explorer Explorer;

        private LinearSpline AgeVersusValue;

        private Calculator Calculator;

        private LinearSpline ValueVersusAge;
    }
}
