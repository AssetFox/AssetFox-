import { BudgetPriority, BudgetPriorityLibrary } from '@/shared/models/iAM/budget-priority';

export interface CreateBudgetPriorityLibraryDialogData {
    showDialog: boolean;
    budgetPriorities: BudgetPriority[];
    budgetPriorityLibrary: BudgetPriorityLibrary
}

export const emptyCreateBudgetPriorityLibraryDialogData: CreateBudgetPriorityLibraryDialogData = {
    showDialog: false,
    budgetPriorities: [],
};
