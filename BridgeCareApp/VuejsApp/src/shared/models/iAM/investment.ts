import {
    CriterionLibrary,
    emptyCriterionLibrary,
} from '@/shared/models/iAM/criteria';
import { getBlankGuid } from '@/shared/utils/uuid-utils';
import { clone } from 'ramda';
import moment from 'moment';

export interface Investment {
    investmentPlan: InvestmentPlan;
    scenarioBudgets: Budget[];
}

export interface BudgetAmount {
    id: string;
    budgetName: string;
    year: number;
    value: number;
}

export interface Budget {
    id: string;
    budgetOrder: number;
    name: string;
    libraryId: string;
    isModified: boolean;
    budgetAmounts: BudgetAmount[];
    criterionLibrary: CriterionLibrary;
}

export interface BudgetLibraryUser {
    userId: string;
    username: string;
    canModify: boolean;
    isOwner: boolean;
}

export interface BudgetLibrary {
    id: string;
    name: string;
    description: string;
    budgets: Budget[];
    users: BudgetLibraryUser[];
    appliedScenarioIds: string[];
    owner?: string;
    isShared: boolean;
}

export interface InvestmentPlan {
    id: string;
    firstYearOfAnalysisPeriod: number;
    inflationRatePercentage: number;
    minimumProjectCostLimit: number;
    numberOfYearsInAnalysisPeriod: number;
    shouldAccumulateUnusedBudgetAmounts: boolean;
}

export interface BudgetYearsGridData {
    year: number;
    values: {[budgetName: string]: number | null};
}

export interface SimpleBudgetDetail {
    id: string;
    name: string;
}

export interface InvestmentBudgetFileImport {
    file: File;
    overwriteBudgets: boolean;
}

export interface LibraryInvestmentBudgetFileImport
    extends InvestmentBudgetFileImport {
    libraryId: string;
}

export interface ScenarioInvestmentBudgetFileImport
    extends InvestmentBudgetFileImport {
    scenarioId: string;
}

export const emptyBudgetLibrary: BudgetLibrary = {
    id: getBlankGuid(),
    name: '',
    description: '',
    budgets: [],
    users: [],
    appliedScenarioIds: [],
    isShared: false
};

export const emptyBudgetLibraryUsers: BudgetLibraryUser[] = [{
    userId: '',
    username: '',
    canModify: false,
    isOwner: false
}];

export const emptyBudget: Budget = {
    id: getBlankGuid(),
    name: '',
    budgetAmounts: [],
    libraryId: getBlankGuid(),
    isModified: false,
    criterionLibrary: clone(emptyCriterionLibrary),
    budgetOrder: 0
};

export const emptyBudgetAmount: BudgetAmount = {
    id: getBlankGuid(),
    budgetName: '',
    year: moment().year(),
    value: 0,
};

export const emptyInvestmentPlan: InvestmentPlan = {
    id: getBlankGuid(),
    firstYearOfAnalysisPeriod: moment().year(),
    inflationRatePercentage: 0,
    minimumProjectCostLimit: 0,
    numberOfYearsInAnalysisPeriod: 1,
    shouldAccumulateUnusedBudgetAmounts: false
};
