import { CashFlowRule } from '@/shared/models/iAM/cash-flow';

export interface CreateCashFlowRuleLibraryDialogData {
    showDialog: boolean;
    cashFlowRules: CashFlowRule[];
}

export const emptyCreateCashFlowLibraryDialogData: CreateCashFlowRuleLibraryDialogData = {
    showDialog: false,
    cashFlowRules: [],
};
