export interface ValidationResult {
    isValid: boolean;
    validationMessage: string;
}

export interface CriterionValidationResult extends ValidationResult {
    resultsCount: number;
}

export interface ValidationParameter {
    expression: string;
}

export interface EquationValidationParameters extends ValidationParameter {
    isPiecewise: boolean;
}