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

    static importTreatments(
        file: File,
        id: string,
        forScenario: boolean
        //currentUserCriteriaFilter: UserCriteriaFilter,
    ) {
        let formData = new FormData();

        formData.append('file', file);
        formData.append(forScenario ? 'simulationId' : 'libraryId', id);
        // formData.append(
        //     'currentUserCriteriaFilter',
        //     JSON.stringify(currentUserCriteriaFilter),
        // );

        return forScenario
            ? coreAxiosInstance.post(
                  `${API.PerformanceCurve}/ImportLibraryTreatmentsFile`,
                  formData,
                  { headers: { 'Content-Type': 'multipart/form-data' } },
              )
            : coreAxiosInstance.post(
                  `${API.PerformanceCurve}/ImportLibraryTreatmentsFile`,
                  formData,
                  { headers: { 'Content-Type': 'multipart/form-data' } },
              );
    }

    static exportTreatments(
        id: string,
        forScenario: boolean = false,
    ): AxiosPromise {
        return forScenario
            ? coreAxiosInstance.get(
                  `${API.PerformanceCurve}/ExportScenarioTreatmentsExcelFile/${id}`,
              )
            : coreAxiosInstance.get(
                  `${API.PerformanceCurve}/ExportScenarioTreatmentsExcelFile/${id}`,
              );
    }
}
