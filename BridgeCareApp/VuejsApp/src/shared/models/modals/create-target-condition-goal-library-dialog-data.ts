import {TargetConditionGoal} from '@/shared/models/iAM/target-condition-goal';
import {getBlankGuid} from '@/shared/utils/uuid-utils';

export interface CreateTargetConditionGoalLibraryDialogData {
    showDialog: boolean;
    targetConditionGoals: TargetConditionGoal[];
}

export const emptyCreateTargetConditionGoalLibraryDialogData: CreateTargetConditionGoalLibraryDialogData = {
    showDialog: false,
    targetConditionGoals: [],
};
