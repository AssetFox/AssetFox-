import {BudgetPriority} from '@/shared/models/iAM/budget-priority';
import {getBlankGuid} from '@/shared/utils/uuid-utils';

export interface CreateBudgetPriorityLibraryDialogData {
    showDialog: boolean;
    budgetPriorities: BudgetPriority[];
    scenarioId: string;
}

export const emptyCreateBudgetPriorityLibraryDialogData: CreateBudgetPriorityLibraryDialogData = {
    showDialog: false,
    budgetPriorities: [],
    scenarioId: getBlankGuid()
};
