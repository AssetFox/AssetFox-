import { AxiosPromise } from 'axios';
import { API, coreAxiosInstance } from '@/shared/utils/axios-instance';
import {
    TargetConditionGoal,
    TargetConditionGoalLibrary,
} from '@/shared/models/iAM/target-condition-goal';

export default class TargetConditionGoalService {
    static getTargetConditionGoalLibraries(): AxiosPromise {
        return coreAxiosInstance.get(
            `${API.TargetConditionGoal}/GetTargetConditionGoalLibraries`,
        );
    }

    static upsertTargetConditionGoalLibrary(
        data: TargetConditionGoalLibrary,
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
        data: TargetConditionGoal[],
        scenarioId: string,
    ): AxiosPromise {
        return coreAxiosInstance.post(
            `${API.TargetConditionGoal}/UpsertScenarioTargetConditionGoals/${scenarioId}`,
            data,
        );
    }
}
