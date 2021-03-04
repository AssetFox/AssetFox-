import {CashFlowRule} from '@/shared/models/iAM/cash-flow';
import {getBlankGuid} from '@/shared/utils/uuid-utils';

export interface CreateCashFlowRuleLibraryDialogData {
    showDialog: boolean;
    cashFlowRules: CashFlowRule[];
    scenarioId: string;
}

export const emptyCreateCashFlowLibraryDialogData: CreateCashFlowRuleLibraryDialogData = {
    showDialog: false,
    cashFlowRules: [],
    scenarioId: getBlankGuid()
};