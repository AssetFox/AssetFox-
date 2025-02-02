﻿using System;
using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAM.Analysis;

public abstract class ConditionGoal : WeakEntity, IValidator
{
    public virtual INumericAttribute Attribute { get; set; }

    public Criterion Criterion { get; }

    public string Name { get; set; }

    public virtual ValidatorBag Subvalidators => new ValidatorBag { Criterion };

    public virtual ValidationResultBag GetDirectValidationResults()
    {
        var results = new ValidationResultBag();

        if (Attribute == null)
        {
            results.Add(ValidationStatus.Error, "Attribute is unset.", this, nameof(Attribute));
        }

        if (string.IsNullOrWhiteSpace(Name))
        {
            results.Add(ValidationStatus.Warning, "Name is blank.", this, nameof(Name));
        }

        return results;
    }

    public abstract bool IsMet(double actual);

    protected ConditionGoal(Explorer explorer) => Criterion = new Criterion(explorer ?? throw new ArgumentNullException(nameof(explorer)));
    public string ShortDescription => Name;
}
