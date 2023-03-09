import { Budget } from '@/shared/models/iAM/investment';

export interface CreateBudgetLibraryDialogData {
    showDialog: boolean;
    budgets: Budget[];
}

export const emptyCreateBudgetLibraryDialogData: CreateBudgetLibraryDialogData = {
    showDialog: false,
    budgets: [],
};
