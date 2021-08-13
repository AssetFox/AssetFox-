import {AxiosPromise} from 'axios';
import {API, coreAxiosInstance} from '@/shared/utils/axios-instance';
import { BudgetPriority, BudgetPriorityLibrary } from '@/shared/models/iAM/budget-priority';

export default class BudgetPriorityService {
    static getBudgetPriorityLibraries(): AxiosPromise {
        return coreAxiosInstance.get(`${API.BudgetPriority}/GetBudgetPriorityLibraries`);
    }

    static upsertBudgetPriorityLibrary(data: BudgetPriorityLibrary): AxiosPromise {
        return coreAxiosInstance.post(`${API.BudgetPriority}/UpsertBudgetPriorityLibrary`, data);
    }

    static deleteBudgetPriorityLibrary(libraryId: string): AxiosPromise {
        return coreAxiosInstance.delete(`${API.BudgetPriority}/DeleteBudgetPriorityLibrary/${libraryId}`);
    }

    static getScenarioBudgetPriorities(): AxiosPromise {
        return coreAxiosInstance.get(`${API.BudgetPriority}/GetScenarioBudgetPriorities`);
    }

    static upsertScenarioBudgetPriorities(data: BudgetPriority[], scenarioId: string): AxiosPromise {
        return coreAxiosInstance.post(`${API.BudgetPriority}/UpsertScenarioBudgetPriorities/${scenarioId}`, data);
}