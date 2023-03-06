import { AxiosPromise } from 'axios';
import { API, coreAxiosInstance } from '@/shared/utils/axios-instance';
import {
    TargetConditionGoal,
    TargetConditionGoalLibrary,
} from '@/shared/models/iAM/target-condition-goal';
import { LibraryUser } from '@/shared/models/iAM/user';
import { LibraryUpsertPagingRequest, PagingRequest, PaginSync } from '@/shared/models/iAM/paging';

export default class TargetConditionGoalService {
    static getTargetConditionGoalLibraries(): AxiosPromise {
        return coreAxiosInstance.get(
            `${API.TargetConditionGoal}/GetTargetConditionGoalLibraries`,
        );
    }

    static getScenarioTargetConditionGoalPage(scenarioId: string, data:PagingRequest<TargetConditionGoal>): AxiosPromise {
        return coreAxiosInstance.post(
            `${API.TargetConditionGoal}/GetScenarioTargetConditionGoalPage/${scenarioId}`, data
        );
    }

    static getLibraryTargetConditionGoalPage(libraryId: string, data:PagingRequest<TargetConditionGoal>): AxiosPromise {
        return coreAxiosInstance.post(
            `${API.TargetConditionGoal}/GetLibraryTargetConditionGoalPage/${libraryId}`, data
        );
    }

    static upsertTargetConditionGoalLibrary(
        data: LibraryUpsertPagingRequest<TargetConditionGoalLibrary, TargetConditionGoal>,
    ): AxiosPromise {
        return coreAxiosInstance.post(
            `${API.TargetConditionGoal}/UpsertTargetConditionGoalLibrary/`,
            data,
        );
    }

    static deleteTargetConditionGoalLibrary(libraryId: string): AxiosPromise {
        return coreAxiosInstance.delete(
            `${API.TargetConditionGoal}/DeleteTargetConditionGoalLibrary/${libraryId}`,
        );
    }

    static getScenarioTargetConditionGoals(scenarioId: string): AxiosPromise {
        return coreAxiosInstance.get(
            `${API.TargetConditionGoal}/GetScenarioTargetConditionGoals/${scenarioId}`,
        );
    }

    static upsertScenarioTargetConditionGoals(
        data: PaginSync<TargetConditionGoal>,
        scenarioId: string,
    ): AxiosPromise {
        return coreAxiosInstance.post(
            `${API.TargetConditionGoal}/UpsertScenarioTargetConditionGoals/${scenarioId}`,
            data,
        );
    }
    static getTargetConditionGoalLibraryUsers(libraryId: string): AxiosPromise {
        return coreAxiosInstance.get(`${API.TargetConditionGoal}/GetTargetConditionGoalLibraryUsers/${libraryId}`);
    }

    static upsertOrDeleteTargetConditionGoalLibraryUsers(libraryId: string, proposedUsers: LibraryUser[]): AxiosPromise {
        return coreAxiosInstance.post(
            `${API.TargetConditionGoal}/UpsertOrDeleteTargetConditionGoalLibraryUsers/${libraryId}`
            , proposedUsers
        );
    }
    static getIsSharedLibrary(targetConditionGoalLibraryId: string): AxiosPromise {
        return coreAxiosInstance.get(
            `${API.TargetConditionGoal}/GetIsSharedLibrary/${targetConditionGoalLibraryId}`
        );
    }

    static getHasPermittedAccess(): AxiosPromise {
        return coreAxiosInstance.get(
            `${API.TargetConditionGoal}/GetHasPermittedAccess`,
        );
    }
}
