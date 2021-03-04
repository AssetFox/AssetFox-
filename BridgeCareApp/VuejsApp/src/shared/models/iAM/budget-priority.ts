import {getBlankGuid} from '@/shared/utils/uuid-utils';
import {CriterionLibrary, emptyCriterionLibrary} from '@/shared/models/iAM/criteria';
import {clone} from 'ramda';

export interface BudgetPercentagePair {
    id: string;
    budgetId: string;
    budgetName: string;
    percentage: number;
}

export interface BudgetPriority {
    id: string;
    priorityLevel: number;
    year: number | null;
    budgetPercentagePairs: BudgetPercentagePair[];
    criterionLibrary: CriterionLibrary;
}

export interface BudgetPriorityLibrary {
    id: string;
    name: string;
    description: string;
    budgetPriorities: BudgetPriority[];
    appliedScenarioIds: string[];
    owner?: string;
    shared?: boolean;
}

export const emptyBudgetPriority: BudgetPriority = {
    id: getBlankGuid(),
    priorityLevel: 1,
    year: null,
    budgetPercentagePairs: [],
    criterionLibrary: clone(emptyCriterionLibrary)
};

export const emptyBudgetPriorityLibrary: BudgetPriorityLibrary = {
    id: getBlankGuid(),
    name: '',
    description: '',
    budgetPriorities: [],
    appliedScenarioIds: []
};

export interface BudgetPriorityGridDatum {
    id: string;
    priorityLevel: string;
    year: string;
    criteria: string;
    [budgetName: string]: string;
}
