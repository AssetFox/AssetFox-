import { API, coreAxiosInstance } from '@/shared/utils/axios-instance';
import { AxiosPromise } from 'axios';
import {
    RemainingLifeLimit,
    RemainingLifeLimitLibrary,
} from '@/shared/models/iAM/remaining-life-limit';

export default class RemainingLifeLimitService {
    static getRemainingLifeLimitLibraries(): AxiosPromise {
        return coreAxiosInstance.get(
            `${API.RemainingLifeLimit}/GetRemainingLifeLimitLibraries`,
        );
    }

    static upsertRemainingLifeLimitLibrary(
        data: RemainingLifeLimitLibrary,
    ): AxiosPromise {
        return coreAxiosInstance.post(
            `${API.RemainingLifeLimit}/UpsertRemainingLifeLimitLibrary/`,
            data,
        );
    }

    static deleteRemainingLifeLimitLibrary(libraryId: string): AxiosPromise {
        return coreAxiosInstance.delete(
            `${API.RemainingLifeLimit}/DeleteRemainingLifeLimitLibrary/${libraryId}`,
        );
    }
    static getScenarioRemainingLifeLimit(scenarioId: string): AxiosPromise {
        return coreAxiosInstance.get(
            `${API.RemainingLifeLimit}/GetScenarioRemainingLifeLimits/${scenarioId}`,
        );
    }

    static upsertScenarioRemainingLifeLimits(
        data: RemainingLifeLimit[],
        scenarioId: string,
    ): AxiosPromise {
        return coreAxiosInstance.post(
            `${API.RemainingLifeLimit}/UpsertScenarioRemainingLifeLimits/${scenarioId}`,
            data,
        );
    }
}
