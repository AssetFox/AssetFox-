import {emptyTargetConditionGoalLibrary, TargetConditionGoalLibrary} from '@/shared/models/iAM/target-condition-goal';
import {clone} from 'ramda';

export interface ShareTargetConditionGoalLibraryDialogData {
    showDialog: boolean;
    targetConditionGoalLibrary: TargetConditionGoalLibrary;
}

export const emptyShareTargetConditionGoalLibraryDialogData: ShareTargetConditionGoalLibraryDialogData = {
    showDialog: false,
    targetConditionGoalLibrary: clone(emptyTargetConditionGoalLibrary)
};

export interface TargetConditionGoalLibraryUserGridRow {
    id: string;
    username: string;
    isShared: boolean;
    canModify: boolean;
}