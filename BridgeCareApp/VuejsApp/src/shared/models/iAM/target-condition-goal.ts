import {CriterionLibrary, emptyCriterionLibrary} from '@/shared/models/iAM/criteria';
import {clone} from 'ramda';
import {getBlankGuid} from '@/shared/utils/uuid-utils';

export interface TargetConditionGoal {
    id: string;
    name: string;
    attribute: string;
    target: number;
    year: number | null;
    libraryId: string;
    criterionLibrary: CriterionLibrary;
}

export interface TargetConditionGoalLibrary {
    id: string;
    name: string;
    description: string;
    targetConditionGoals: TargetConditionGoal[];
    owner?: string;
    users: TargetConditionGoalLibraryUser[];
    isShared: boolean;
}

export const emptyTargetConditionGoal: TargetConditionGoal = {
    id: getBlankGuid(),
    name: '',
    attribute: '',
    target: 1,
    year: null,
    libraryId: getBlankGuid(),
    criterionLibrary: clone(emptyCriterionLibrary)
};

export const emptyTargetConditionGoalLibrary: TargetConditionGoalLibrary = {
    id: getBlankGuid(),
    name: '',
    description: '',
    targetConditionGoals: [],
    users: [],
    isShared: false
};
export interface TargetConditionGoalLibraryUser {
    userId: string;
    username: string;
    canModify: boolean;
    isOwner: boolean;
}
export const emptyTargetConditionGoalLibraryUsers: TargetConditionGoalLibraryUser[] = [{
    userId: '',
    username: '',
    canModify: false,
    isOwner: false
}];

