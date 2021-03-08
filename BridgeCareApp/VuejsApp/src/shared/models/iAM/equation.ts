import {getBlankGuid} from '@/shared/utils/uuid-utils';

export interface Equation {
    id: string;
    expression: string;
}

export interface EquationValidationResult {
    isValid: boolean;
    message: string;
}

export const emptyEquation: Equation = {
    id: getBlankGuid(),
    expression: ''
};
