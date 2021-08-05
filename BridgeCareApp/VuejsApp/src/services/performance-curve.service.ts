import { AxiosPromise } from 'axios';
import {
    PerformanceCurve,
    PerformanceCurveLibrary,
} from '@/shared/models/iAM/performance';
import { API, coreAxiosInstance } from '@/shared/utils/axios-instance';

export default class PerformanceCurveService {
    static getPerformanceCurveLibraries(): AxiosPromise {
        return coreAxiosInstance.get(
            `${API.PerformanceCurve}/GetPerformanceCurveLibraries`,
        );
    }

    static upsertPerformanceCurveLibrary(
        data: PerformanceCurveLibrary,
    ): AxiosPromise {
        return coreAxiosInstance.post(
            `${API.PerformanceCurve}/UpsertPerformanceCurveLibrary`,
            data,
        );
    }

    static deletePerformanceCurveLibrary(libraryId: string): AxiosPromise {
        return coreAxiosInstance.delete(
            `${API.PerformanceCurve}/DeletePerformanceCurveLibrary/${libraryId}`,
        );
    }

    static getScenarioPerformanceCurves(scenarioId: string): AxiosPromise {
        return coreAxiosInstance.get(
            `${API.PerformanceCurve}/GetScenarioPerformanceCurves/${scenarioId}`,
        );
    }

    static upsertScenarioPerformanceCurves(
        data: PerformanceCurve[],
        scenarioId: string,
    ): AxiosPromise {
        return coreAxiosInstance.post(
            `${API.PerformanceCurve}/UpsertScenarioPerformanceCurves/${scenarioId}`,
            data,
        );
    }
}
