import {emptyBudgetLibraryUsers, BudgetLibraryUser} from '@/shared/models/iAM/investment';
import {clone} from 'ramda';

export interface ShareBudgetLibraryDialogData {
    showDialog: boolean;
    budgetLibraryUsers: BudgetLibraryUser[];
}

export const emptyShareBudgetLibraryDialogData: ShareBudgetLibraryDialogData = {
    showDialog: false,
    budgetLibraryUsers: clone(emptyBudgetLibraryUsers)
};

export interface BudgetLibraryUserGridRow {
    id: string;
    username: string;
    isShared: boolean;
    canModify: boolean;
}