import {API, coreAxiosInstance} from '@/shared/utils/axios-instance';
import {AxiosPromise} from 'axios';
import {CashFlowRuleLibrary} from '@/shared/models/iAM/cash-flow';

export default class CashFlowService {
    static getCashFlowRuleLibraries(): AxiosPromise {
        return coreAxiosInstance.get(`${API.CashFlowController}/GetCashFlowRuleLibraries`);
    }

    static addOrUpdateCashFlowRuleLibrary(data: CashFlowRuleLibrary, scenarioId: string): AxiosPromise {
        return coreAxiosInstance.post(`${API.CashFlowController}/AddOrUpdateCashFlowRuleLibrary/${scenarioId}`, data);
    }

    static deleteCashFlowRuleLibrary(libraryId: string): AxiosPromise {
        return coreAxiosInstance.delete(`${API.CashFlowController}/DeleteCashFlowRuleLibrary/${libraryId}`);
    }
}