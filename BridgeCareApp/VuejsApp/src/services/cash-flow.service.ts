import { API, coreAxiosInstance } from '@/shared/utils/axios-instance';
import { AxiosPromise } from 'axios';
import { LibraryUser } from '@/shared/models/iAM/user';
import {
    CashFlowRule,
    CashFlowRuleLibrary,
} from '@/shared/models/iAM/cash-flow';
import { LibraryUpsertPagingRequest, PagingRequest, PaginSync } from '@/shared/models/iAM/paging';

export default class CashFlowService {
    static getCashFlowRuleLibraries(): AxiosPromise {
        return coreAxiosInstance.get(
            `${API.CashFlow}/GetCashFlowRuleLibraries`,
        );
    }

    static getScenarioCashFlowRulePage(scenarioId: string, data:PagingRequest<CashFlowRule>): AxiosPromise {
        return coreAxiosInstance.post(
            `${API.CashFlow}/GetScenarioCashFlowRulePage/${scenarioId}`, data
        );
    }

    static getLibraryCashFlowRulePage(libraryId: string, data:PagingRequest<CashFlowRule>): AxiosPromise {
        return coreAxiosInstance.post(
            `${API.CashFlow}/GetLibraryCashFlowRulePage/${libraryId}`, data
        );
    }

    static upsertCashFlowRuleLibrary(data: LibraryUpsertPagingRequest<CashFlowRuleLibrary, CashFlowRule>): AxiosPromise {
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
        data: PaginSync<CashFlowRule>,
        scenarioId: string,
    ): AxiosPromise {
        return coreAxiosInstance.post(
            `${API.CashFlow}/UpsertScenarioCashFlowRules/${scenarioId}`,
            data,
        );
    }
    static getCashFlowRuleLibraryUsers(libraryId: string): AxiosPromise {
        return coreAxiosInstance.get(`${API.CashFlow}/GetCashFlowRuleLibraryUsers/${libraryId}`);
    }

    static upsertOrDeleteCashFlowRuleLibraryUsers(libraryId: string, proposedUsers: LibraryUser[]): AxiosPromise {
        return coreAxiosInstance.post(
            `${API.CashFlow}/UpsertOrDeleteCashFlowRuleLibraryUsers/${libraryId}`
            , proposedUsers
        );
    }
    static getIsSharedCashFlowRuleLibrary(cashFlowRuleLibraryId: string): AxiosPromise {
        return coreAxiosInstance.get(
            `${API.CashFlow}/GetIsSharedLibrary/${cashFlowRuleLibraryId}`
        );
    }
    static getHasPermittedAccess(): AxiosPromise {
        return coreAxiosInstance.get(
            `${API.CashFlow}/GetHasPermittedAccess`,
        );
    }
}
