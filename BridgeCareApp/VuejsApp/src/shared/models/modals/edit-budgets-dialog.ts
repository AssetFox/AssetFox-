import {Budget} from '@/shared/models/iAM/investment';

export interface EditBudgetsDialogData {
    showDialog: boolean;
    budgets: Budget[]
}

export const emptyEditBudgetsDialogData: EditBudgetsDialogData = {
    showDialog: false,
    budgets: []
};
