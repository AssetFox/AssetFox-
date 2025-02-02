﻿using System;
using System.Text.RegularExpressions;
using AppliedResearchAssociates.CalculateEvaluate;

namespace AppliedResearchAssociates.iAMCore
{
    public sealed class AttributeValueChange : CompilableExpression
    {
        internal ChangeApplicator GetApplicator(Attribute attribute, CalculateEvaluateScope scope)
        {
            switch (attribute)
            {
            case NumberAttribute _:
                var oldNumber = scope.GetNumber(attribute.Name);
                var newNumber = ChangeNumber(oldNumber);
                return new ChangeApplicator(() => scope.SetNumber(attribute.Name, newNumber), newNumber);

            case TextAttribute _:
                var newText = Expression;
                return new ChangeApplicator(() => scope.SetText(attribute.Name, newText), null);

            default:
                throw new ArgumentException("Invalid attribute type.", nameof(attribute));
            }
        }

        protected override void Compile()
        {
            if (ExpressionIsBlank)
            {
                NumberChanger = null;
                return;
            }

            var match = NumberChangePattern.Match(Expression);
            if (!match.Success || !double.TryParse(match.Groups[2].Value, out var operand))
            {
                throw ExpressionCouldNotBeCompiled();
            }

            Operand = operand;
            var operation = match.Groups[1].Value;
            var operandType = match.Groups[3].Value;

            switch (operandType)
            {
            case "%":
                Operand /= 100;
                switch (operation)
                {
                case "+":
                    NumberChanger = AddPercentage;
                    break;

                case "-":
                    NumberChanger = SubtractPercentage;
                    break;

                case "":
                    NumberChanger = SetPercentage;
                    break;

                default:
                    throw new InvalidOperationException("Invalid operation.");
                }
                break;

            case "":
                switch (operation)
                {
                case "+":
                    NumberChanger = Add;
                    break;

                case "-":
                    NumberChanger = Subtract;
                    break;

                case "":
                    NumberChanger = Set;
                    break;

                default:
                    throw new InvalidOperationException("Invalid operation.");
                }
                break;

            default:
                throw new InvalidOperationException("Invalid operand type.");
            }
        }

        private static readonly Regex NumberChangePattern = new Regex(@"(?>\A\s*((?:\+|-)?)([^%]+)(%?)\s*\z)", RegexOptions.Compiled);

        private Func<double, double> NumberChanger;

        private double Operand;

        private double ChangeNumber(double value)
        {
            EnsureCompiled();
            var newValue = NumberChanger?.Invoke(value) ?? value;
            return newValue;
        }

        #region Number-changing operations

        private double Add(double value) => value + Operand;

        private double AddPercentage(double value) => value * (1 + Operand);

        private double Set(double value) => Operand;

        private double SetPercentage(double value) => value * Operand;

        private double Subtract(double value) => value - Operand;

        private double SubtractPercentage(double value) => value * (1 - Operand);

        #endregion Number-changing operations
    }
}
