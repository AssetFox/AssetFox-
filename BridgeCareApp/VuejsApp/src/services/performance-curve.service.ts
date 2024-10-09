import { AxiosPromise } from 'axios';
import {
    PerformanceCurve,
    PerformanceCurveLibrary,
} from '@/shared/models/iAM/performance';
import { LibraryUser } from '@/shared/models/iAM/user';
import { API, coreAxiosInstance } from '@/shared/utils/axios-instance';
import { UserCriteriaFilter } from '@/shared/models/iAM/user-criteria-filter';
import { LibraryUpsertPagingRequest, PagingRequest, PaginSync } from '@/shared/models/iAM/paging';

export default class PerformanceCurveService {
    static getPerformanceCurveLibraries(): AxiosPromise {
        return coreAxiosInstance.get(
            `${API.PerformanceCurve}/GetPerformanceCurveLibraries`,
        );
    }

    static getPerformanceCurvePage(scenarioId: string, data:PagingRequest<PerformanceCurve>): AxiosPromise {
        return coreAxiosInstance.post(
            `${API.PerformanceCurve}/GetScenarioPerformanceCurvePage/${scenarioId}`, data
        );
    }

    static getPerformanceLibraryModifiedDate (libraryId: string): AxiosPromise {
        return coreAxiosInstance.get(`${API.PerformanceCurve}/GetPerformanceLibraryModifiedDate/${libraryId}`)
    }


    static GetLibraryPerformanceCurvePage(library: string, data:PagingRequest<PerformanceCurve>): AxiosPromise {
        return coreAxiosInstance.post(
            `${API.PerformanceCurve}/GetLibraryPerformanceCurvePage/${library}`, data
        );
    }

    static UpsertPerformanceCurveLibrary(data:LibraryUpsertPagingRequest<PerformanceCurveLibrary, PerformanceCurve>): AxiosPromise {
        return coreAxiosInstance.post(
            `${API.PerformanceCurve}/UpsertPerformanceCurveLibrary`,
            data,
        );
    }

    static UpsertScenarioPerformanceCurves(data: PaginSync<PerformanceCurve>, scenarioId: string){
        return coreAxiosInstance.post(
            `${API.PerformanceCurve}/UpsertScenarioPerformanceCurves/${scenarioId}`, data
        );
    }

    static deletePerformanceCurveLibrary(libraryId: string): AxiosPromise {
        return coreAxiosInstance.delete(
            `${API.PerformanceCurve}/DeletePerformanceCurveLibrary/${libraryId}`,
        );
    }

    static GetPerformanceCurves(scenarioId: string): AxiosPromise {
        return coreAxiosInstance.get(
            `${API.PerformanceCurve}/GetScenarioPerformanceCurves/${scenarioId}`
        );
    }

    static GetDistinctScenarioPerformanceFactorAttributeNames(): AxiosPromise {
        return coreAxiosInstance.get(
            `${API.PerformanceCurve}/GetDistinctScenarioPerformanceFactorAttributeNames`
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
    static  GetPerformanceCurveLibraryUsers(libraryId: string): AxiosPromise {
        return coreAxiosInstance.get(`${API.PerformanceCurve}/GetPerformanceCurveLibraryUsers/${libraryId}`);
    }
    static getIsSharedLibrary(performanceCurveLibraryId: string): AxiosPromise {
        return coreAxiosInstance.get(
            `${API.PerformanceCurve}/GetIsSharedLibrary/${performanceCurveLibraryId}`
        );
    }

    static upsertOrDeletePerformanceCurveLibraryUsers(libraryId: string, proposedUsers: LibraryUser[]): AxiosPromise {
        return coreAxiosInstance.post(
            `${API.PerformanceCurve}/UpsertOrDeletePerformanceCurveLibraryUsers/${libraryId}`, 
            proposedUsers
        );
    }
    static getHasPermittedAccess(): AxiosPromise {
        return coreAxiosInstance.get(
            `${API.PerformanceCurve}/GetHasPermittedAccess`,
        );
    }
}
