import { AxiosPromise } from 'axios';
import {
    PerformanceCurve,
    PerformanceCurveLibrary,
} from '@/shared/models/iAM/performance';
import { API, coreAxiosInstance } from '@/shared/utils/axios-instance';
import { UserCriteriaFilter } from '@/shared/models/iAM/user-criteria-filter';

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

    static importPerformanceCurves(
        file: File,
        id: string,
        forScenario: boolean,
        currentUserCriteriaFilter: UserCriteriaFilter,
    ) {
        let formData = new FormData();

        formData.append('file', file);
        formData.append(forScenario ? 'simulationId' : 'libraryId', id);
        formData.append(
            'currentUserCriteriaFilter',
            JSON.stringify(currentUserCriteriaFilter),
        );

        return forScenario
            ? coreAxiosInstance.post(
                  `${API.PerformanceCurve}/ImportScenarioPerformanceCurvesExcelFile`,
                  formData,
                  { headers: { 'Content-Type': 'multipart/form-data' } },
              )
            : coreAxiosInstance.post(
                  `${API.PerformanceCurve}/ImportLibraryPerformanceCurvesExcelFile`,
                  formData,
                  { headers: { 'Content-Type': 'multipart/form-data' } },
              );
    }

    static exportPerformanceCurves(
        id: string,
        forScenario: boolean = false,
    ): AxiosPromise {
        return forScenario
            ? coreAxiosInstance.get(
                  `${API.PerformanceCurve}/ExportScenarioPerformanceCurvesExcelFile/${id}`,
              )
            : coreAxiosInstance.get(
                  `${API.PerformanceCurve}/ExportLibraryPerformanceCurvesExcelFile/${id}`,
              );
    }

    static downloadPerformanceCurvesTemplate()
    : AxiosPromise {
        return coreAxiosInstance.get(               
            `${API.PerformanceCurve}/DownloadPerformanceCurvesTemplate`,
        );
    }

    static getHasPermittedAccess(): AxiosPromise {
        return coreAxiosInstance.get(
            `${API.PerformanceCurve}/GetHasPermittedAccess`,
        );
    }
}
