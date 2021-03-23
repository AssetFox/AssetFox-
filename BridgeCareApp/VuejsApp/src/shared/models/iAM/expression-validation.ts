import { UserCriteriaFilter } from "./user-criteria-filter";

export interface ValidationResult {
    isValid: boolean;
    validationMessage: string;
}

export interface CriterionValidationResult extends ValidationResult {
    resultsCount: number;
}

export interface ValidationParameter {
    expression: string;
    currentUserCriteriaFilter: UserCriteriaFilter;
}

export interface EquationValidationParameters extends ValidationParameter {
    isPiecewise: boolean;
}