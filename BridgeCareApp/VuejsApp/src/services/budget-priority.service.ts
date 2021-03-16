import {AxiosPromise} from 'axios';
import {API, coreAxiosInstance} from '@/shared/utils/axios-instance';
import {BudgetPriorityLibrary} from '@/shared/models/iAM/budget-priority';

export default class BudgetPriorityService {
    static getBudgetPriorityLibraries(): AxiosPromise {
        return coreAxiosInstance.get(`${API.BudgetPriority}/GetBudgetPriorityLibraries`);
    }

    static upsertBudgetPriorityLibrary(data: BudgetPriorityLibrary, scenarioId: string): AxiosPromise {
        return coreAxiosInstance.post(`${API.BudgetPriority}/UpsertBudgetPriorityLibrary/${scenarioId}`, data);
    }

    static deleteBudgetPriorityLibrary(libraryId: string): AxiosPromise {
        return coreAxiosInstance.delete(`${API.BudgetPriority}/DeleteBudgetPriorityLibrary/${libraryId}`);
    }
}