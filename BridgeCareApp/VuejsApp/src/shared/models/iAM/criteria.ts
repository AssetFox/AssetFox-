import {getBlankGuid} from '@/shared/utils/uuid-utils';

export interface CriteriaEditorAttribute {
    name: string;
    values: string[];
}

export interface Criteria {
    logicalOperator: string;
    children?: CriteriaType[];
}

export interface CriteriaType {
    type: string;
    query: Criteria | CriteriaRule;
}

export interface CriteriaRule {
    rule: string;
    selectedOperator?: string;
    selectedOperand: string;
    value?: string;
}

export interface CriteriaValidationResult {
    isValid: boolean;
    numberOfResults: number;
    message: string;
}

export interface CriterionLibrary {
    id: string;
    name: string;
    mergedCriteriaExpression: string | null;
    description: string;
    owner?: string;
    shared?: boolean;
    forScenario: boolean
}

export interface CriteriaEditorData {
    mergedCriteriaExpression: string | null;
    isLibraryContext: boolean;
}

export interface CriteriaEditorResult {
    validated: boolean;
    criteria: string | null;
}

export const emptyCriteria: Criteria = {
    logicalOperator: 'AND',
    children: []
};

export const emptyCriterionLibrary: CriterionLibrary = {
    id: getBlankGuid(),
    name: '',
    description: '',
    mergedCriteriaExpression: '',
    forScenario: false
};

export const emptyCriteriaEditorData: CriteriaEditorData = {
    mergedCriteriaExpression: '',
    isLibraryContext: false
};
