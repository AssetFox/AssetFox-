import {DeficientConditionGoal} from '@/shared/models/iAM/deficient-condition-goal';
import {getBlankGuid} from '@/shared/utils/uuid-utils';

export interface CreateDeficientConditionGoalLibraryDialogData {
    showDialog: boolean;
    deficientConditionGoals: DeficientConditionGoal[];
    scenarioId: string;
}

export const emptyCreateDeficientConditionGoalLibraryDialogData: CreateDeficientConditionGoalLibraryDialogData = {
    showDialog: false,
    deficientConditionGoals: [],
    scenarioId: getBlankGuid()
};
