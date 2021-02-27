import {AxiosPromise} from 'axios';
import {API, coreAxiosInstance} from '@/shared/utils/axios-instance';
import {BudgetPriorityLibrary} from '@/shared/models/iAM/budget-priority';

export default class BudgetPriorityService {
    static getBudgetPriorityLibraries(): AxiosPromise {
        return coreAxiosInstance.get(`${API.BudgetPriorityController}/GetBudgetPriorityLibraries`);
    }

    static addOrUpdateBudgetPriorityLibrary(data: BudgetPriorityLibrary, scenarioId: string): AxiosPromise {
        return coreAxiosInstance.post(`${API.BudgetPriorityController}/AddOrUpdateBudgetPriorityLibrary/${scenarioId}`, data);
    }

    static deleteBudgetPriorityLibrary(libraryId: string): AxiosPromise {
        return coreAxiosInstance.delete(`${API.BudgetPriorityController}/DeleteBudgetPriorityLibrary/${libraryId}`);
    }
}