using System;
using System.Collections.Generic;

namespace AppliedResearchAssociates.Validation
{
    public sealed class ValidationTarget
    {
        public ValidationTarget(object targetObject, string targetKey, List<string> validationPath)
        {
            Object = targetObject ?? throw new ArgumentNullException(nameof(targetObject));
            Key = targetKey?.Trim() ?? "";
            ValidationPath = validationPath;
        }

        public string Key { get; }

        public object Object { get; }
        public List<string> ValidationPath { get; }

        public ValidationResult CreateResult(ValidationStatus status, string message) => new ValidationResult(this, status, message);

        public override bool Equals(object obj) => obj is ValidationTarget target && Object == target.Object && Key == target.Key;

        public override int GetHashCode() => HashCode.Combine(Object, Key);
    }
}
