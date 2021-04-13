using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AppliedResearchAssociates.CalculateEvaluate;
using AppliedResearchAssociates.iAM.Analysis;
using MathNet.Numerics.Interpolation;

namespace AppliedResearchAssociates.iAM.Domains
{
    public sealed class Equation : CompilableExpression
    {
        public Equation(Explorer explorer) => Explorer = explorer ?? throw new ArgumentNullException(nameof(explorer));

        public EquationFormat Format =>
            Calculator is object ? EquationFormat.CalculationExpression :
            ValueVersusAge is object ? EquationFormat.PiecewiseExpression :
            EquationFormat.Unknown;

        public IReadOnlyCollection<string> ReferencedParameters
        {
            get
            {
                try
                {
                    EnsureCompiled();
                }
                catch (MalformedInputException)
                {
                    return Array.Empty<string>();
                }
                return Calculator.ReferencedParameters;
            }
        }

        internal double Compute(SectionContext scope) => Compute(scope, null);

        internal double Compute(SectionContext scope, PerformanceCurve curve)
        {
            EnsureCompiled();
            if (Format == EquationFormat.PiecewiseExpression)
            {
                var actualAge = scope.GetNumber(Explorer.AgeAttribute.Name);
                if (curve is null)
                {
                    return ValueVersusAge.Interpolate(actualAge);
                }
                else
                {
                    var previousValue = scope.GetNumber(curve.Attribute.Name);
                    var apparentPreviousAge = AgeVersusValue.Interpolate(previousValue);

                    var previousAge = actualAge - 1;
                    if (curve.Shift && previousAge != 0)
                    {
                        var shiftFactor = apparentPreviousAge / previousAge;
                        var shiftedAge = actualAge * shiftFactor;
                        return ValueVersusAge.Interpolate(shiftedAge);
                    }
                    else
                    {
                        var apparentAge = apparentPreviousAge + 1;
                        return ValueVersusAge.Interpolate(apparentAge);
                    }
                }
            }

            if (Format == EquationFormat.CalculationExpression)
            {
                return Calculator.Delegate(scope);
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
