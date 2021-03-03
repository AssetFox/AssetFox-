import {AxiosPromise} from 'axios';
import {API, coreAxiosInstance} from '@/shared/utils/axios-instance';
import {DeficientConditionGoalLibrary} from '@/shared/models/iAM/deficient-condition-goal';

export default class DeficientConditionGoalService {
    static getDeficientConditionGoalLibraries(): AxiosPromise {
        return coreAxiosInstance.get(`${API.DeficientConditionGoalController}/GetDeficientConditionGoalLibraries`);
    }

    static upsertDeficientConditionGoalLibrary(data: DeficientConditionGoalLibrary, scenarioId: string): AxiosPromise {
        return coreAxiosInstance.post(`${API.DeficientConditionGoalController}/UpsertDeficientConditionGoalLibrary/${scenarioId}`, data);
    }

    static deleteDeficientConditionGoalLibrary(libraryId: string): AxiosPromise {
        return coreAxiosInstance.delete(`${API.DeficientConditionGoalController}/DeleteDeficientConditionGoalLibrary/${libraryId}`);
    }
}