using System;
using System.Text.RegularExpressions;
using AppliedResearchAssociates.CalculateEvaluate;
using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAM.Analysis;

public sealed class AttributeValueChange : CompilableExpression
{
    internal AttributeValueChange()
    {
    }

    public Attribute Attribute { get; set; }

    public override ValidationResultBag GetDirectValidationResults()
    {
        if (Attribute is null)
        {
            return new ValidationResultBag();
        }
        else
        {
            switch (Attribute)
            {
            case NumberAttribute _:
                return base.GetDirectValidationResults();
            case TextAttribute _:
                return new ValidationResultBag();
            default:
                throw new InvalidOperationException("Invalid attribute type.");
            }
        }
    }

    public ConsequenceApplicator GetApplicator(CalculateEvaluateScope scope)
    {
        switch (Attribute)
        {
        case NumberAttribute _:
            EnsureCompiled();
            if (NumberChanger is null)
            {
                return null;
            }
            else
            {
                var oldNumber = scope.GetNumber(Attribute.Name);
                var newNumber = NumberChanger.Invoke(oldNumber);
                return new ConsequenceApplicator(Attribute, () => scope.SetNumber(Attribute.Name, newNumber), newNumber);
            }

        case TextAttribute _:
            var newText = Expression;
            return new ConsequenceApplicator(Attribute, () => scope.SetText(Attribute.Name, newText), null);

        default:
            throw new InvalidOperationException("Invalid attribute type.");
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

    #region Number-changing operations

    private double Add(double value) => value + Operand;

    private double AddPercentage(double value) => value * (1 + Operand);

    private double Set(double value) => Operand;

    private double SetPercentage(double value) => value * Operand;

    private double Subtract(double value) => value - Operand;

    private double SubtractPercentage(double value) => value * (1 - Operand);

    #endregion Number-changing operations
}
