import {AxiosPromise} from 'axios';
import {API, coreAxiosInstance} from '@/shared/utils/axios-instance';
import {DeficientConditionGoal, DeficientConditionGoalLibrary} from '@/shared/models/iAM/deficient-condition-goal';

export default class DeficientConditionGoalService {
    static getDeficientConditionGoalLibraries(): AxiosPromise {
        return coreAxiosInstance.get(`${API.DeficientConditionGoal}/GetDeficientConditionGoalLibraries`);
    }

    static upsertDeficientConditionGoalLibrary(data: DeficientConditionGoalLibrary): AxiosPromise {
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
        data: DeficientConditionGoal[],
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