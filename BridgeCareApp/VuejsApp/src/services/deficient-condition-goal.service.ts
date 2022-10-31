import {AxiosPromise} from 'axios';
import {API, coreAxiosInstance} from '@/shared/utils/axios-instance';
import {DeficientConditionGoal, DeficientConditionGoalLibrary} from '@/shared/models/iAM/deficient-condition-goal';
import { LibraryUpsertPagingRequest, PagingRequest, PaginSync } from '@/shared/models/iAM/paging';

export default class DeficientConditionGoalService {
    static getDeficientConditionGoalLibraries(): AxiosPromise {
        return coreAxiosInstance.get(`${API.DeficientConditionGoal}/GetDeficientConditionGoalLibraries`);
    }

    static getScenarioDeficientConditionGoalPage(scenarioId: string, data:PagingRequest<DeficientConditionGoal>): AxiosPromise {
        return coreAxiosInstance.post(
            `${API.DeficientConditionGoal}/GetScenarioDeficientConditionGoalPage/${scenarioId}`, data
        );
    }

    static getLibraryDeficientConditionGoalPage(libraryId: string, data:PagingRequest<DeficientConditionGoal>): AxiosPromise {
        return coreAxiosInstance.post(
            `${API.DeficientConditionGoal}/GetLibraryDeficientConditionGoalPage/${libraryId}`, data
        );
    }

    static upsertDeficientConditionGoalLibrary( data: LibraryUpsertPagingRequest<DeficientConditionGoalLibrary, DeficientConditionGoal>): AxiosPromise {
        return coreAxiosInstance.post(`${API.DeficientConditionGoal}/UpsertDeficientConditionGoalLibrary/`, data);
    }

    static deleteDeficientConditionGoalLibrary(libraryId: string): AxiosPromise {
        return coreAxiosInstance.delete(`${API.DeficientConditionGoal}/DeleteDeficientConditionGoalLibrary/${libraryId}`);
    }

    static getScenarioDeficientConditionGoals(scenarioId: string): AxiosPromise {
        return coreAxiosInstance.get(
            `${API.DeficientConditionGoal}/GetScenarioDeficientConditionGoals/${scenarioId}`,
        );
    }

    static upsertScenarioDeficientConditionGoals(
        data: PaginSync<DeficientConditionGoal>,
        scenarioId: string,
    ): AxiosPromise {
        return coreAxiosInstance.post(
            `${API.DeficientConditionGoal}/UpsertScenarioDeficientConditionGoals/${scenarioId}`,
            data,
        );
    }

    static getHasPermittedAccess(): AxiosPromise {
        return coreAxiosInstance.get(
            `${API.DeficientConditionGoal}/GetHasPermittedAccess`,
        );
    }
}