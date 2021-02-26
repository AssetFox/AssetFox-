import {hasValue} from '@/shared/utils/has-value-util';
import {CashFlowRule, CashFlowDistributionRule} from '@/shared/models/iAM/cash-flow';
import {findIndex, propEq, contains} from 'ramda';
import {getPropertyValues} from '@/shared/utils/getter-utils';
import {CriteriaDrivenBudget} from '@/shared/models/iAM/investment';

export interface InputValidationRules {
    [Rules: string]: any;
}

/***********************************************GENERAL RULES**********************************************************/
const generalRules = {
    'valueIsNotEmpty': (value: any) => {
        return hasValue(value) || 'Value cannot be empty';
    },
    'valueIsWithinRange': (value: number, range: number[]) => {
        return (value >= range[0] && value <= range[1]) || `Value must be in range ${range[0]} - ${range[1]}`;
    }
};
/***********************************************CASH FLOW RULES********************************************************/
const cashFlowRules = {
    'isDurationGreaterThanPreviousDuration': (distributionRule: CashFlowDistributionRule, cashFlowRule: CashFlowRule) => {
        const index: number = findIndex(
            propEq('id', distributionRule.id), cashFlowRule.cashFlowDistributionRules);

        if (index > 0) {
            return distributionRule.durationInYears > cashFlowRule.cashFlowDistributionRules[index - 1].durationInYears ||
                'Value must be greater than previous value';
        }

        return true;
    },
    'isAmountGreaterThanOrEqualToPreviousAmount': (distributionRule: CashFlowDistributionRule, cashFlowRule: CashFlowRule) => {
        const index: number = findIndex(
            propEq('id', distributionRule.id), cashFlowRule.cashFlowDistributionRules);

        if (index > 0) {
            const currentAmount: number | null = hasValue(distributionRule.costCeiling)
                ? parseFloat(distributionRule.costCeiling!.toString().replace(/(\$*)(\,*)/g, ''))
                : null;

            const previousAmount: number | null = hasValue(cashFlowRule.cashFlowDistributionRules[index - 1].costCeiling)
                ? cashFlowRule.cashFlowDistributionRules[index - 1].costCeiling!
                : null;

            return !hasValue(currentAmount) || (hasValue(currentAmount) && !hasValue(previousAmount)) ||
                (hasValue(currentAmount) && hasValue(previousAmount) && currentAmount! >= previousAmount!) ||
                'Value must be greater than or equal to previous value';
        }

        return true;
    },
    'doesTotalOfPercentsEqualOneHundred': (values: string) => {
        let total: number = 0;

        if (values.indexOf('/')) {
            const percents: number[] = values.split('/').map((value: string) => parseInt(value));
            total = percents.reduce((x: number, y: number) => x + y);
        }

        return total === 100 || 'Total of percents must equal 100';
    }
};
/**********************************************INVESTMENT RULES********************************************************/
const investmentRules = {
    'budgetNameIsUnique': (budget: CriteriaDrivenBudget, budgets: CriteriaDrivenBudget[]) => {
        const otherBudgetNames: string[] = getPropertyValues(
            'budgetName', budgets.filter((b: CriteriaDrivenBudget) => b.id !== budget.id));
        return !contains(budget.budgetName, otherBudgetNames) || 'Budget name must be unique';
    }
};
/***********************************************TREATMENT RULES********************************************************/
const treatmentRules = {
    'hasChangeValueOrEquation': (changeValue: string, expression: string) => {
        return hasValue(changeValue) || hasValue(expression) || 'Must have an equation to be blank';
    }
};
/**************************************************ALL RULES***********************************************************/
export const rules: InputValidationRules = {
    'generalRules': generalRules,
    'cashFlowRules': cashFlowRules,
    'investmentRules': investmentRules,
    'treatmentRules': treatmentRules
};