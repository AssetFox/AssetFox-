import { API, coreAxiosInstance } from '@/shared/utils/axios-instance';
import { AxiosPromise } from 'axios';
import {
    CashFlowRule,
    CashFlowRuleLibrary,
} from '@/shared/models/iAM/cash-flow';

export default class CashFlowService {
    static getCashFlowRuleLibraries(): AxiosPromise {
        return coreAxiosInstance.get(
            `${API.CashFlow}/GetCashFlowRuleLibraries`,
        );
    }

    static upsertCashFlowRuleLibrary(data: CashFlowRuleLibrary): AxiosPromise {
        return coreAxiosInstance.post(
            `${API.CashFlow}/UpsertCashFlowRuleLibrary`,
            data,
        );
    }

    static deleteCashFlowRuleLibrary(libraryId: string): AxiosPromise {
        return coreAxiosInstance.delete(
            `${API.CashFlow}/DeleteCashFlowRuleLibrary/${libraryId}`,
        );
    }

    static getScenarioCashFlowRules(scenarioId: string): AxiosPromise {
        return coreAxiosInstance.get(
            `${API.CashFlow}/GetScenarioCashFlowRules/${scenarioId}`,
        );
    }

    static upsertScenarioCashFlowRules(
        data: CashFlowRule[],
        scenarioId: string,
    ): AxiosPromise {
        return coreAxiosInstance.post(
            `${API.CashFlow}/UpsertScenarioCashFlowRules/${scenarioId}`,
            data,
        );
    }
}
