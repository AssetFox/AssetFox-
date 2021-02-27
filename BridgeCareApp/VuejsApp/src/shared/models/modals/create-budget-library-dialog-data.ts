import {Budget} from '@/shared/models/iAM/investment';
import {getBlankGuid} from '@/shared/utils/uuid-utils';

export interface CreateBudgetLibraryDialogData {
    showDialog: boolean;
    budgets: Budget[];
    scenarioId: string;
}

export const emptyCreateBudgetLibraryDialogData: CreateBudgetLibraryDialogData = {
    showDialog: false,
    budgets: [],
    scenarioId: getBlankGuid()
};
