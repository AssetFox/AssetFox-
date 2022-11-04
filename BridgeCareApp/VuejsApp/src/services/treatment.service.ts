import { AxiosPromise } from 'axios';
import { LibraryUser } from '@/shared/models/iAM/user';
import { Treatment, TreatmentLibrary } from '@/shared/models/iAM/treatment';
import { API, coreAxiosInstance } from '@/shared/utils/axios-instance';
import { LibraryUpsertPagingRequest, PaginSync } from '@/shared/models/iAM/paging';

export default class TreatmentService {
    static getTreatmentLibraries(): AxiosPromise {
        return coreAxiosInstance.get(`${API.Treatment}/GetTreatmentLibraries`);
    }

    static upsertTreatmentLibrary(data: LibraryUpsertPagingRequest<TreatmentLibrary, Treatment>): AxiosPromise {
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

    static getSimpleTreatmentsByLibraryId(libraryId: string): AxiosPromise {
        return coreAxiosInstance.get(
            `${API.Treatment}/GetSimpleTreatmentsByLibraryId/${libraryId}`,
        );
    }

    static getSelectedTreatmentById(treatmentId: string): AxiosPromise {
        return coreAxiosInstance.get(
            `${API.Treatment}/GetSelectedTreatmentById/${treatmentId}`,
        );
    }

    static getScenarioSelectedTreatmentById(treatmentId: string): AxiosPromise {
        return coreAxiosInstance.get(
            `${API.Treatment}/GetScenarioSelectedTreatmentById/${treatmentId}`,
        );
    }

    static getSimpleTreatmentsByScenarioId(scenarioId: string): AxiosPromise {
        return coreAxiosInstance.get(
            `${API.Treatment}/GetSimpleTreatmentsByScenarioId/${scenarioId}`,
        );
    }

    static upsertScenarioSelectedTreatments(
        data: PaginSync<Treatment>,
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
            ?
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
            ?     
               coreAxiosInstance.get(               
                  `${API.Treatment}/ExportScenarioTreatmentsExcelFile/${id}`,
              )
            : coreAxiosInstance.get(                
                  `${API.Treatment}/ExportLibraryTreatmentsExcelFile/${id}`,
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

    static downloadTreatmentsTemplate(
        forScenario: boolean = false,
    ): AxiosPromise {
        return forScenario
            ?     
               coreAxiosInstance.get(               
                  `${API.Treatment}/DownloadScenarioTreatmentsTemplate`,
              )
            : coreAxiosInstance.get(                
                  `${API.Treatment}/DownloadLibraryTreatmentsTemplate`,
              );
    }

    static getHasPermittedAccess(): AxiosPromise {
        return coreAxiosInstance.get(
            `${API.Treatment}/GetHasPermittedAccess`,
        );
    }
    static getHasOwnerAccess(treatment: Treatment) {
        return coreAxiosInstance.get(
            `${API.Treatment}/GetHasOwnerAccess/${treatment.id}`
        );
    }
    static getHasViewAccess() {
        return coreAxiosInstance.get(
            `${API.Treatment}/GetHasViewAccess`
        );
    }
    static getHasSharedAccess(treatment: Treatment) {
        return coreAxiosInstance.get(
            `${API.Treatment}/GetHasSharedAccess/${treatment.id}`
        );
    }
    static setSharedStatus(treatmentLibrary: TreatmentLibrary, userId: string) {
        return coreAxiosInstance.post(`${API.Treatment}/SetSharedStatus/${treatmentLibrary}, ${userId}`);
    }
    static getTreatmentLibraryUsers(libraryId: string): AxiosPromise {
        return coreAxiosInstance.get(`${API.Treatment}/GetTreatmentLibraryUsers/${libraryId}`);
    }
    static upsertOrDeleteTreatmentLibraryUsers(libraryId: string, proposedUsers: LibraryUser[]): AxiosPromise {
        return coreAxiosInstance.post(`${API.Treatment}/UpsertOrDeleteTreatmentLibraryUsers/${libraryId}`, proposedUsers);
    }
}
