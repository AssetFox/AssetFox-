import { getBlankGuid } from '@/shared/utils/uuid-utils';
import { SelectItem } from '../vue/select-item';

export interface Criteria {
    logicalOperator: string;
    children?: CriteriaType[];
}

export interface CriteriaType {
    id: string;
    type: string;
    query: Criteria | CriteriaRule;
}

export interface CriteriaRule {
    rule: string;
    selectedOperator?: string;
    selectedOperand: string;
    value?: string;
}

export interface CriterionLibrary {
    id: string;
    name: string;
    mergedCriteriaExpression: string | null;
    description: string;
    owner?: string;
    isShared: boolean;
    isSingleUse: boolean;
}

export interface CriteriaEditorData {
    mergedCriteriaExpression: string | null;
    isLibraryContext: boolean;
    networkId: string;
}

export interface CriteriaConfigRule {
    type: string;
    label: string;
    id: string;
    operators: string[];
    choices: SelectItem[];
}

export interface CriteriaEditorResult {
    validated: boolean;
    criteria: string | null;
}

export const emptyCriteria: Criteria = {
    logicalOperator: '',
    children: [],
};

export const emptyCriterionLibrary: CriterionLibrary = {
    id: getBlankGuid(),
    name: '',
    description: '',
    mergedCriteriaExpression: '',
    isShared: false,
    isSingleUse: true,
};

export const emptyCriteriaEditorData: CriteriaEditorData = {
    mergedCriteriaExpression: '',
    isLibraryContext: false,
    networkId: getBlankGuid()
};
