using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAM.Analysis;

public class CriterionEquationPair : WeakEntity, IValidator
{
    internal CriterionEquationPair(Explorer explorer)
    {
        Criterion = new Criterion(explorer);
        Equation = new Equation(explorer);
    }

    public Criterion Criterion { get; }

    public Equation Equation { get; }

    public ValidatorBag Subvalidators => new ValidatorBag { Criterion, Equation };

    public ValidationResultBag GetDirectValidationResults() => new ValidationResultBag();

    public string ShortDescription => nameof(CriterionEquationPair);
}
