import { API, coreAxiosInstance } from '@/shared/utils/axios-instance';
import { AxiosPromise } from 'axios';
import {
    RemainingLifeLimit,
    RemainingLifeLimitLibrary,
} from '@/shared/models/iAM/remaining-life-limit';
import { LibraryUpsertPagingRequest, PagingRequest, PaginSync } from '@/shared/models/iAM/paging';

export default class RemainingLifeLimitService {
    static getRemainingLifeLimitLibraries(): AxiosPromise {
        return coreAxiosInstance.get(
            `${API.RemainingLifeLimit}/GetRemainingLifeLimitLibraries`,
        );
    }

    static getScenarioRemainingLifeLimitPage(scenarioId: string, data:PagingRequest<RemainingLifeLimit>): AxiosPromise {
        return coreAxiosInstance.post(
            `${API.RemainingLifeLimit}/GetScenarioRemainingLifeLimitPage/${scenarioId}`, data
        );
    }

    static getLibraryRemainingLifeLimitPage(libraryId: string, data:PagingRequest<RemainingLifeLimit>): AxiosPromise {
        return coreAxiosInstance.post(
            `${API.RemainingLifeLimit}/GetLibraryRemainingLifeLimitPage/${libraryId}`, data
        );
    }

    static upsertRemainingLifeLimitLibrary(
        data: LibraryUpsertPagingRequest<RemainingLifeLimitLibrary, RemainingLifeLimit>,
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
        data: PaginSync<RemainingLifeLimit>,
        scenarioId: string,
    ): AxiosPromise {
        return coreAxiosInstance.post(
            `${API.RemainingLifeLimit}/UpsertScenarioRemainingLifeLimits/${scenarioId}`,
            data,
        );
    }
}
