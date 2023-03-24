import {getBlankGuid} from '@/shared/utils/uuid-utils';
import {CriterionLibrary, emptyCriterionLibrary} from '@/shared/models/iAM/criteria';
import {clone} from 'ramda';

export interface CashFlowDistributionRule {
    id: string;
    durationInYears: number;
    costCeiling: number | null;
    yearlyPercentages: string;
}

export interface CashFlowRule {
    id: string;
    name: string;
    libraryId: string;
    isModified: boolean;
    criterionLibrary: CriterionLibrary;
    cashFlowDistributionRules: CashFlowDistributionRule[];
}

export interface CashFlowRuleLibrary {
    id: string;
    name: string;
    description: string;
    cashFlowRules: CashFlowRule[];
    appliedScenarioIds: string[];
    users: CashFlowRuleLibraryUser[];
    owner?: string;
    isShared: boolean;
}

export const emptyCashFlowRuleLibrary: CashFlowRuleLibrary = {
    id: getBlankGuid(),
    name: '',
    description: '',
    cashFlowRules: [],
    users: [],
    appliedScenarioIds: [],
    isShared: false
};

export const emptyCashFlowRule: CashFlowRule = {
    id: getBlankGuid(),
    name: '',
    isModified: false,
    libraryId: getBlankGuid(),
    criterionLibrary: clone(emptyCriterionLibrary),
    cashFlowDistributionRules: []
};

export const emptyCashFlowDistributionRule: CashFlowDistributionRule = {
    id: getBlankGuid(),
    durationInYears: 1,
    costCeiling: 0,
    yearlyPercentages: '100'
};
export interface CashFlowRuleLibraryUser {
    userId: string;
    username: string;
    canModify: boolean;
    isOwner: boolean;
}
export const emptyCashFlowRuleLibraryUsers: CashFlowRuleLibraryUser[] = [{
    userId: '',
    username: '',
    canModify: false,
    isOwner: false
}];