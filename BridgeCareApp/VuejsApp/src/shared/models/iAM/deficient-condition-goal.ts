import {getBlankGuid} from '@/shared/utils/uuid-utils';
import {CriterionLibrary, emptyCriterionLibrary} from '@/shared/models/iAM/criteria';
import {clone} from 'ramda';

export interface DeficientConditionGoal {
    id: string;
    name: string;
    attribute: string;
    deficientLimit: number;
    allowedDeficientPercentage: number;
    criterionLibrary: CriterionLibrary;
}

export interface DeficientConditionGoalLibrary {
    id: string;
    name: string;
    description: string;
    deficientConditionGoals: DeficientConditionGoal[];
    owner?: string;
    isShared: boolean;
}

export const emptyDeficientConditionGoal: DeficientConditionGoal = {
    id: getBlankGuid(),
    name: '',
    attribute: '',
    deficientLimit: 0,
    allowedDeficientPercentage: 0,
    criterionLibrary: clone(emptyCriterionLibrary)
};

export const emptyDeficientConditionGoalLibrary: DeficientConditionGoalLibrary = {
    id: getBlankGuid(),
    name: '',
    description: '',
    deficientConditionGoals: [],
    isShared: false
};
