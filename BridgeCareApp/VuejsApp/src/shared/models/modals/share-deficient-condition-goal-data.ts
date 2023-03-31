import {emptyDeficientConditionGoalLibrary, DeficientConditionGoalLibrary} from '@/shared/models/iAM/deficient-condition-goal';
import {clone} from 'ramda';

export interface ShareDeficientConditionGoalLibraryDialogData {
    showDialog: boolean;
    deficientConditionGoalLibrary: DeficientConditionGoalLibrary;
}

export const emptyShareDeficientConditionGoalLibraryDialogData: ShareDeficientConditionGoalLibraryDialogData = {
    showDialog: false,
    deficientConditionGoalLibrary: clone(emptyDeficientConditionGoalLibrary)
};

export interface DeficientConditionGoalLibraryUserGridRow {
    id: string;
    username: string;
    isShared: boolean;
    canModify: boolean;
}