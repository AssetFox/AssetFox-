import {CriterionLibrary, emptyCriterionLibrary} from '@/shared/models/iAM/criteria';
import {getBlankGuid} from '@/shared/utils/uuid-utils';
import {clone} from 'ramda';
import moment from 'moment';

export interface Investment {
    investmentPlan: InvestmentPlan;
    budgetLibraries: BudgetLibrary[];
}

export interface BudgetAmount {
    id: string;
    budgetName: string;
    year: number;
    value: number;
}

export interface Budget {
    id: string;
    name: string;
    budgetAmounts: BudgetAmount[];
    criterionLibrary: CriterionLibrary;
}

export interface BudgetLibrary {
    id: string;
    name: string;
    description: string;
    budgets: Budget[];
    appliedScenarioIds: string[];
    owner?: string;
    shared?: boolean;
}

export interface InvestmentPlan {
    id: string;
    firstYearOfAnalysisPeriod: number;
    inflationRatePercentage: number;
    minimumProjectCostLimit: number;
    numberOfYearsInAnalysisPeriod: number;
}

export interface BudgetYearsGridData {
    year: number;
    [budgetName: string]: number | null;
}

export interface SimpleBudgetDetail {
    id: string;
    name: string;
}

export const emptyBudgetLibrary: BudgetLibrary = {
    id: getBlankGuid(),
    name: '',
    description: '',
    budgets: [],
    appliedScenarioIds: []
};

export const emptyBudget: Budget = {
    id: getBlankGuid(),
    name: '',
    budgetAmounts: [],
    criterionLibrary: clone(emptyCriterionLibrary)
};

export const emptyBudgetAmount: BudgetAmount = {
    id: getBlankGuid(),
    budgetName: '',
    year: moment().year(),
    value: 0
};

export const emptyInvestmentPlan: InvestmentPlan = {
    id: getBlankGuid(),
    firstYearOfAnalysisPeriod: moment().year(),
    inflationRatePercentage: 0,
    minimumProjectCostLimit: 500000,
    numberOfYearsInAnalysisPeriod: 1
};
