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
    isShared: boolean;
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
    appliedScenarioIds: [],
    isShared: false
};

export const emptyBudget: Budget = {
    id: getBlankGuid(),
    name: '',
    budgetAmounts: [],
    criterionLibrary: clone(emptyCriterionLibrary),
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
};
