import {emptyInvestmentLibrary} from '@/shared/models/iAM/investment';
import {emptyPerformanceCurveLibrary} from '@/shared/models/iAM/performance';
import {emptyTreatmentLibrary} from '@/shared/models/iAM/treatment';
import {emptyPriorityLibrary} from '@/shared/models/iAM/priority';
import {emptyTargetConditionGoalLibrary} from '@/shared/models/iAM/target-condition-goal';
import {emptyDeficientConditionGoalLibrary} from '@/shared/models/iAM/deficient-condition-goal';
import {emptyRemainingLifeLimitLibrary} from '@/shared/models/iAM/remaining-life-limit';
import {emptyCashFlowRuleLibrary} from '@/shared/models/iAM/cash-flow';
import {emptyCriterionLibrary} from '@/shared/models/iAM/criteria';
import {clone, isEmpty, keys, symmetricDifference} from 'ramda';
import {hasValue} from '@/shared/utils/has-value-util';
import {sorter} from '@/shared/utils/sorter-utils';

export const hasUnsavedChangesCore = (editor: string, localSelectedLibrary: any, stateSelectedLibrary: any) => {
    const localLibrary = sortNonObjectLists(clone(localSelectedLibrary));
    const selectedLibrary = sortNonObjectLists(clone(stateSelectedLibrary));

    switch (editor) {
        case 'performance-curves':
            return !isEqual(localLibrary, emptyPerformanceCurveLibrary) && !isEqual(localLibrary, selectedLibrary);
        case 'cash-flow':
            return !isEqual(localLibrary, emptyCashFlowRuleLibrary) && !isEqual(localLibrary, selectedLibrary);
        case 'remaining-life-limit':
            return !isEqual(localLibrary, emptyRemainingLifeLimitLibrary) && !isEqual(localLibrary, selectedLibrary);
        case 'deficient-condition-goal':
            return !isEqual(localLibrary, emptyDeficientConditionGoalLibrary) && !isEqual(localLibrary, selectedLibrary);
        case 'target-condition-goal':
            return !isEqual(localLibrary, emptyTargetConditionGoalLibrary) && !isEqual(localLibrary, selectedLibrary);
        default:
            return false;
    }
};

export const hasUnsavedChanges = (editor: string, localSelectedLibrary: any, stateSelectedLibrary: any, stateScenarioLibrary: any) => {
    const localLibrary = sortNonObjectLists(clone(localSelectedLibrary));
    const selectedLibrary = sortNonObjectLists(clone(stateSelectedLibrary));
    const scenarioLibrary = sortNonObjectLists(clone(stateScenarioLibrary));

    switch (editor) {
        case 'investment':
            return !isEqual(localLibrary, emptyInvestmentLibrary) &&
                !isEqual(localLibrary, selectedLibrary) &&
                !isEqual(localLibrary, scenarioLibrary);
        case 'performance':
            return !isEqual(localLibrary, emptyPerformanceCurveLibrary) &&
                !isEqual(localLibrary, selectedLibrary) &&
                !isEqual(localLibrary, scenarioLibrary);
        case 'treatment':
            return !isEqual(localLibrary, emptyTreatmentLibrary) &&
                !isEqual(localLibrary, selectedLibrary) &&
                !isEqual(localLibrary, scenarioLibrary);
        case 'priority':
            return !isEqual(localLibrary, emptyPriorityLibrary) &&
                !isEqual(localLibrary, selectedLibrary) &&
                !isEqual(localLibrary, scenarioLibrary);
        case 'target':
            return !isEqual(localLibrary, emptyTargetConditionGoalLibrary) &&
                !isEqual(localLibrary, selectedLibrary) &&
                !isEqual(localLibrary, scenarioLibrary);
        case 'deficient':
            return !isEqual(localLibrary, emptyDeficientConditionGoalLibrary) &&
                !isEqual(localLibrary, selectedLibrary) &&
                !isEqual(localLibrary, scenarioLibrary);
        case 'remaininglifelimit':
            return !isEqual(localLibrary, emptyRemainingLifeLimitLibrary) &&
                !isEqual(localLibrary, selectedLibrary) &&
                !isEqual(localLibrary, scenarioLibrary);
        case 'cashflow':
            return !isEqual(localLibrary, emptyCashFlowRuleLibrary) &&
                !isEqual(localLibrary, selectedLibrary) &&
                !isEqual(localLibrary, scenarioLibrary);
        case 'criteria':
            return !isEqual(localLibrary, emptyCriterionLibrary) &&
                !isEqual(localLibrary, selectedLibrary);
        default:
            return false;
    }
};

export const sortNonObjectLists = (item: any) => {
    if (hasValue(item)) {
        keys(item).forEach((prop) => {
            if (Array.isArray(item[prop])) {
                if (item[prop].every((arrItem: any) => typeof arrItem === 'object')) {
                    item[prop] = item[prop].map((arrItem: any) => sortNonObjectLists(arrItem));
                } else {
                    item[prop] = sorter(item[prop]);
                }
            }
        });
    }

    return item;
};

export const isEqual = (item1: any, item2: any) => {
    return isEmpty(symmetricDifference([item1], [item2]));
};
