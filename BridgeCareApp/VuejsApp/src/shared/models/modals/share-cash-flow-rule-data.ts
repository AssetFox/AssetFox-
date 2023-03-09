import {emptyCashFlowRuleLibrary, CashFlowRuleLibrary} from '@/shared/models/iAM/cash-flow';
import {clone} from 'ramda';

export interface ShareCashFlowRuleLibraryDialogData {
    showDialog: boolean;
    cashFlowRuleLibrary: CashFlowRuleLibrary;
}

export const emptyShareCashFlowRuleLibraryDialogData: ShareCashFlowRuleLibraryDialogData = {
    showDialog: false,
    cashFlowRuleLibrary: clone(emptyCashFlowRuleLibrary)
};

export interface CashFlowRuleLibraryUserGridRow {
    id: string;
    username: string;
    isShared: boolean;
    canModify: boolean;
}