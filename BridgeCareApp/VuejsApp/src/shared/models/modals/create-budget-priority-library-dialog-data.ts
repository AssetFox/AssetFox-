import { BudgetPriority } from '@/shared/models/iAM/budget-priority';

export interface CreateBudgetPriorityLibraryDialogData {
    showDialog: boolean;
    budgetPriorities: BudgetPriority[];
}

export const emptyCreateBudgetPriorityLibraryDialogData: CreateBudgetPriorityLibraryDialogData = {
    showDialog: false,
    budgetPriorities: [],
};
