import { Budget } from '@/shared/models/iAM/investment';
import { getBlankGuid } from '@/shared/utils/uuid-utils';

export interface EditBudgetsDialogData {
    showDialog: boolean;
    budgets: Budget[];
    scenarioId: string;
}

export const emptyEditBudgetsDialogData: EditBudgetsDialogData = {
    showDialog: false,
    budgets: [],
    scenarioId: getBlankGuid(),
};
