import {emptyBudgetPriorityLibrary, BudgetPriorityLibrary} from '@/shared/models/iAM/budget-priority';
import {clone} from 'ramda';

export interface ShareBudgetPriorityLibraryDialogData {
    showDialog: boolean;
    budgetPriorityLibrary: BudgetPriorityLibrary;
}

export const emptyShareBudgetPriorityLibraryDialogData: ShareBudgetPriorityLibraryDialogData = {
    showDialog: false,
    budgetPriorityLibrary: clone(emptyBudgetPriorityLibrary)
};

export interface BudgetPriorityLibraryUserGridRow {
    id: string;
    username: string;
    isShared: boolean;
    canModify: boolean;
}