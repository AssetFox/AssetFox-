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
    ) {
        let formData = new FormData();

        formData.append('file', file);
        formData.append(forScenario ? 'simulationId' : 'libraryId', id);
      
        return forScenario            
            ? // TODO: check for api name after functionality for scenario based import is in place.
              coreAxiosInstance.post(
                  `${API.Treatment}/ImportScenarioTreatmentsFile`,
                  formData,
                  { headers: { 'Content-Type': 'multipart/form-data' } },
              )
            : coreAxiosInstance.post(
                  `${API.Treatment}/ImportLibraryTreatmentsFile`,
                  formData,
                  { headers: { 'Content-Type': 'multipart/form-data' } },
              );
    }

    static exportTreatments(
        id: string,
        forScenario: boolean = false,
    ): AxiosPromise {
        return forScenario
            ?  // TODO: check for api name after functionality for scenario based export is in place.   
               coreAxiosInstance.get(               
                  `${API.Treatment}/ExportScenarioTreatmentsExcelFile/${id}`,
              )
            : coreAxiosInstance.get(
                // TODO: The api looks to be for library, its name need to be changed though
                  `${API.Treatment}/ExportScenarioTreatmentsExcelFile/${id}`,
              );
    }

    static deleteTreatment(
        treatment: Treatment,
        libraryId: string
    ):
        AxiosPromise {
        return coreAxiosInstance.post(
            `${API.Treatment}/DeleteTreatment/${libraryId}`,
            treatment,
        );
    }

    static deleteScenarioSelectableTreatment(
        scenarioSelectableTreatment: Treatment,
        simulationId: string
    ):
        AxiosPromise {
        return coreAxiosInstance.post(
            `${API.Treatment}/DeleteScenarioSelectableTreatment/${simulationId}`,
            scenarioSelectableTreatment,
        );
    }
}
