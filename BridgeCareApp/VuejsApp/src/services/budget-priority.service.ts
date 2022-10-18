import { AxiosPromise } from 'axios';
import { API, coreAxiosInstance } from '@/shared/utils/axios-instance';
import {
    BudgetPriority,
    BudgetPriorityLibrary,
} from '@/shared/models/iAM/budget-priority';
import { LibraryUpsertPagingRequest, PagingRequest, PaginSync } from '@/shared/models/iAM/paging';

export default class BudgetPriorityService {
    static getBudgetPriorityLibraries(): AxiosPromise {
        return coreAxiosInstance.get(
            `${API.BudgetPriority}/GetBudgetPriorityLibraries`,
        );
    }

    static getScenarioBudgetPriorityPage(scenarioId: string, data:PagingRequest<BudgetPriority>): AxiosPromise {
        return coreAxiosInstance.post(
            `${API.BudgetPriority}/GetScenarioBudgetPriorityPage/${scenarioId}`, data
        );
    }

    static getLibraryBudgetPriorityPage(libraryId: string, data:PagingRequest<BudgetPriority>): AxiosPromise {
        return coreAxiosInstance.post(
            `${API.BudgetPriority}/GetLibraryBudgetPriorityPage/${libraryId}`, data
        );
    }

    static upsertBudgetPriorityLibrary(
        data:LibraryUpsertPagingRequest<BudgetPriorityLibrary, BudgetPriority>,
    ): AxiosPromise {
        return coreAxiosInstance.post(
            `${API.BudgetPriority}/UpsertBudgetPriorityLibrary`,
            data,
        );
    }

    static deleteBudgetPriorityLibrary(libraryId: string): AxiosPromise {
        return coreAxiosInstance.delete(
            `${API.BudgetPriority}/DeleteBudgetPriorityLibrary/${libraryId}`,
        );
    }

    static getScenarioBudgetPriorities(scenarioId: string): AxiosPromise {
        return coreAxiosInstance.get(
            `${API.BudgetPriority}/GetScenarioBudgetPriorities/${scenarioId}`,
        );
    }

    static upsertScenarioBudgetPriorities(
        data: PaginSync<BudgetPriority>, scenarioId: string
    ): AxiosPromise {
        return coreAxiosInstance.post(
            `${API.BudgetPriority}/UpsertScenarioBudgetPriorities/${scenarioId}`,
            data,
        );
    }

    static getHasPermittedAccess(): AxiosPromise {
        return coreAxiosInstance.get(
            `${API.BudgetPriority}/GetHasPermittedAccess`,
        );
    }
}
