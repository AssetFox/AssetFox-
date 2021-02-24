import {AxiosPromise} from 'axios';
import {API, coreAxiosInstance} from '@/shared/utils/axios-instance';
import {TargetConditionGoalLibrary} from '@/shared/models/iAM/target-condition-goal';

export default class TargetConditionGoalService {
    static getTargetConditionGoalLibraries(): AxiosPromise {
        return coreAxiosInstance.get(`${API.TargetConditionGoalController}/GetTargetConditionGoalLibraries`);
    }

    static addOrUpdateTargetConditionGoalLibrary(data: TargetConditionGoalLibrary, scenarioId: string): AxiosPromise {
        return coreAxiosInstance.post(`${API.TargetConditionGoalController}/AddOrUpdateTargetConditionGoalLibrary/${scenarioId}`, data);
    }

    static deleteTargetConditionGoalLibrary(libraryId: string): AxiosPromise {
        return coreAxiosInstance.delete(`${API.TargetConditionGoalController}/DeleteTargetConditionGoalLibrary/${libraryId}`);
    }
}