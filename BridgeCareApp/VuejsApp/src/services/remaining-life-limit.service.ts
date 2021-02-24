import {API, coreAxiosInstance} from '@/shared/utils/axios-instance';
import {AxiosPromise} from 'axios';
import {RemainingLifeLimitLibrary} from '@/shared/models/iAM/remaining-life-limit';

export default class RemainingLifeLimitService {
    static getRemainingLifeLimitLibraries(): AxiosPromise {
        return coreAxiosInstance.get(`${API.RemainingLifeLimitController}/GetRemainingLifeLimitLibraries`);
    }

    static addOrUpdateRemainingLifeLimitLibrary(data: RemainingLifeLimitLibrary, scenarioId: string): AxiosPromise {
        return coreAxiosInstance.post(`${API.RemainingLifeLimitController}/AddOrUpdateRemainingLifeLimitLibrary/${scenarioId}`, data);
    }

    static deleteRemainingLifeLimitLibrary(libraryId: string): AxiosPromise {
        return coreAxiosInstance.delete(`${API.RemainingLifeLimitController}/DeleteRemainingLifeLimitLibrary/${libraryId}`);
    }
}
