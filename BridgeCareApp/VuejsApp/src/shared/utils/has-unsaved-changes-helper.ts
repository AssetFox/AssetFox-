import {
    emptyBudgetLibrary,
    emptyInvestmentPlan,
} from '@/shared/models/iAM/investment';
import { emptyPerformanceCurveLibrary } from '@/shared/models/iAM/performance';
import { emptyTreatmentLibrary } from '@/shared/models/iAM/treatment';
import { emptyBudgetPriorityLibrary } from '@/shared/models/iAM/budget-priority';
import { emptyTargetConditionGoalLibrary } from '@/shared/models/iAM/target-condition-goal';
import { emptyDeficientConditionGoalLibrary } from '@/shared/models/iAM/deficient-condition-goal';
import { emptyRemainingLifeLimitLibrary } from '@/shared/models/iAM/remaining-life-limit';
import { emptyCashFlowRuleLibrary } from '@/shared/models/iAM/cash-flow';
import { emptyCriterionLibrary } from '@/shared/models/iAM/criteria';
import { clone, isEmpty, keys, symmetricDifference } from 'ramda';
import { hasValue } from '@/shared/utils/has-value-util';
import { sorter } from '@/shared/utils/sorter-utils';
import {
    emptyCalculatedAttribute,
    emptyCalculatedAttributeLibrary,
} from '../models/iAM/calculated-attribute';

export const hasUnsavedChangesCore = (
    objectType: string,
    object1: any,
    object2: any,
) => {
    const localObject = sortNonObjectLists(clone(object1));
    const stateObject = sortNonObjectLists(clone(object2));

    switch (objectType) {
        case 'cash-flow':
            return (
                !isEqual(localObject, emptyCashFlowRuleLibrary) &&
                !isEqual(localObject, stateObject)
            );
        case 'remaining-life-limit':
            return (
                !isEqual(localObject, emptyRemainingLifeLimitLibrary) &&
                !isEqual(localObject, stateObject)
            );
        case 'criterion-library':
            return (
                !isEqual(localObject, emptyCriterionLibrary) &&
                !isEqual(localObject, stateObject)
            );
        default:
            return !isEqual(localObject, stateObject);
    }
};

export const hasUnsavedChanges = (
    editor: string,
    localSelectedLibrary: any,
    stateSelectedLibrary: any,
    stateScenarioLibrary: any,
) => {
    const localLibrary = sortNonObjectLists(clone(localSelectedLibrary));
    const selectedLibrary = sortNonObjectLists(clone(stateSelectedLibrary));
    const scenarioLibrary = sortNonObjectLists(clone(stateScenarioLibrary));

    switch (editor) {
        case 'investment':
            return (
                !isEqual(localLibrary, emptyBudgetLibrary) &&
                !isEqual(localLibrary, selectedLibrary) &&
                !isEqual(localLibrary, scenarioLibrary)
            );
        case 'performance':
            return (
                !isEqual(localLibrary, emptyPerformanceCurveLibrary) &&
                !isEqual(localLibrary, selectedLibrary) &&
                !isEqual(localLibrary, scenarioLibrary)
            );
        case 'treatment':
            return (
                !isEqual(localLibrary, emptyTreatmentLibrary) &&
                !isEqual(localLibrary, selectedLibrary) &&
                !isEqual(localLibrary, scenarioLibrary)
            );
        case 'priority':
            return (
                !isEqual(localLibrary, emptyBudgetPriorityLibrary) &&
                !isEqual(localLibrary, selectedLibrary) &&
                !isEqual(localLibrary, scenarioLibrary)
            );
        case 'target':
            return (
                !isEqual(localLibrary, emptyTargetConditionGoalLibrary) &&
                !isEqual(localLibrary, selectedLibrary) &&
                !isEqual(localLibrary, scenarioLibrary)
            );
        case 'deficient':
            return (
                !isEqual(localLibrary, emptyDeficientConditionGoalLibrary) &&
                !isEqual(localLibrary, selectedLibrary) &&
                !isEqual(localLibrary, scenarioLibrary)
            );
        case 'remaininglifelimit':
            return (
                !isEqual(localLibrary, emptyRemainingLifeLimitLibrary) &&
                !isEqual(localLibrary, selectedLibrary) &&
                !isEqual(localLibrary, scenarioLibrary)
            );
        case 'cashflow':
            return (
                !isEqual(localLibrary, emptyCashFlowRuleLibrary) &&
                !isEqual(localLibrary, selectedLibrary) &&
                !isEqual(localLibrary, scenarioLibrary)
            );
        case 'criteria':
            return (
                !isEqual(localLibrary, emptyCriterionLibrary) &&
                !isEqual(localLibrary, selectedLibrary)
            );
        case 'calculatedattribute':
            return (
                !isEqual(localLibrary, emptyCalculatedAttributeLibrary) &&
                !isEqual(localLibrary, selectedLibrary) &&
                !isEqual(localLibrary, scenarioLibrary)
            );
        default:
            return false;
    }
};

export const sortNonObjectLists = (item: any) => {
    if (hasValue(item)) {
        keys(item).forEach(prop => {
            if (Array.isArray(item[prop])) {
                if (
                    item[prop].every(
                        (arrItem: any) => typeof arrItem === 'object',
                    )
                ) {
                    item[prop] = item[prop].map((arrItem: any) =>
                        sortNonObjectLists(arrItem),
                    );
                } else {
                    item[prop] = sorter(item[prop]);
                }
            }
        });
    }

    return item;
};

export const isEqual = (item1: any | any[], item2: any | any[]) => {
    if (
        (Array.isArray(item1) && !Array.isArray(item2)) ||
        (!Array.isArray(item1) && Array.isArray(item2))
    ) {
        return false;
    }

    if (Array.isArray(item1) && Array.isArray(item2)) {
        return isEmpty(symmetricDifference(item1, item2));
    }

    return isEmpty(symmetricDifference([item1], [item2]));
};
