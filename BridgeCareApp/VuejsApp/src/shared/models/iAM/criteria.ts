import { getBlankGuid } from '@/shared/utils/uuid-utils';


export interface VueQuery{

}

export interface LevelOperator{
    name: string;
    identifier: string;
    level: number;
}

export interface QueryRule{
    index: number;
    value: string;
    level: number;
    identifier: string;
    uuid: string;
    ruleType: QueryRuleType;
    isGroup: boolean;
    children: QueryRule[];
}

export interface QueryRuleType{
    name: string;
    identifier: string;
    type: string;
    icon: string;
    initialValue: string;
    placeholder: string;
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
