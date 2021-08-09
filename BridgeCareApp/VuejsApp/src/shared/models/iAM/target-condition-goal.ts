import {CriterionLibrary, emptyCriterionLibrary} from '@/shared/models/iAM/criteria';
import {clone} from 'ramda';
import {getBlankGuid} from '@/shared/utils/uuid-utils';

export interface TargetConditionGoal {
    id: string;
    name: string;
    attribute: string;
    target: number;
    year: number | null;
    criterionLibrary: CriterionLibrary;
}

export interface TargetConditionGoalLibrary {
    id: string;
    name: string;
    description: string;
    targetConditionGoals: TargetConditionGoal[];
    //appliedScenarioIds: string[];
    owner?: string;
    shared?: boolean;
}

export const emptyTargetConditionGoal: TargetConditionGoal = {
    id: getBlankGuid(),
    name: '',
    attribute: '',
    target: 1,
    year: null,
    criterionLibrary: clone(emptyCriterionLibrary)
};

export const emptyTargetConditionGoalLibrary: TargetConditionGoalLibrary = {
    id: getBlankGuid(),
    name: '',
    description: '',
    targetConditionGoals: [],
    //appliedScenarioIds: []
};
