import { AxiosPromise } from 'axios';
import { Treatment, TreatmentLibrary } from '@/shared/models/iAM/treatment';
import { API, coreAxiosInstance } from '@/shared/utils/axios-instance';

export default class TreatmentService {
    static getTreatmentLibraries(): AxiosPromise {
        return coreAxiosInstance.get(`${API.Treatment}/GetTreatmentLibraries`);
    }

    static upsertTreatmentLibrary(data: TreatmentLibrary): AxiosPromise {
        return coreAxiosInstance.post(
            `${API.Treatment}/UpsertTreatmentLibrary/`,
            data,
        );
    }

    static deleteTreatmentLibrary(libraryId: string): AxiosPromise {
        return coreAxiosInstance.delete(
            `${API.Treatment}/DeleteTreatmentLibrary/${libraryId}`,
        );
    }

    static getScenarioSelectedTreatments(scenarioId: string): AxiosPromise {
        return coreAxiosInstance.get(
            `${API.Treatment}/GetScenarioSelectedTreatments/${scenarioId}`,
        );
    }

    static upsertScenarioSelectedTreatments(
        data: Treatment[],
        scenarioId: string,
    ): AxiosPromise {
        return coreAxiosInstance.post(
            `${API.Treatment}/UpsertScenarioSelectedTreatments/${scenarioId}`,
            data,
        );
    }
}
