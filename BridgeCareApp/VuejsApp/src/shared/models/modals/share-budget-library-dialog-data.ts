import {emptyBudgetLibrary, BudgetLibrary} from '@/shared/models/iAM/investment';
import {clone} from 'ramda';

export interface ShareBudgetLibraryDialogData {
    showDialog: boolean;
    budgetLibrary: BudgetLibrary;
}

export const emptyShareBudgetLibraryDialogData: ShareBudgetLibraryDialogData = {
    showDialog: false,
    budgetLibrary: clone(emptyBudgetLibrary)
};

export interface BudgetLibraryUserGridRow {
    id: string;
    username: string;
    isShared: boolean;
    canModify: boolean;
}