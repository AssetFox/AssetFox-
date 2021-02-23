import {AxiosPromise} from 'axios';
import {PerformanceCurveLibrary} from '@/shared/models/iAM/performance';
import {API, coreAxiosInstance} from '@/shared/utils/axios-instance';


export default class PerformanceCurvesEditorService {
    /**
     * Gets all performance curve libraries a user can read/edit
     */
    static getPerformanceCurveLibraries(): AxiosPromise {
        return coreAxiosInstance.get(`${API.PerformanceCurveLibraryController}/GetPerformanceCurveLibraries`);
    }

    /**
     * Add or updates a performance curve library
     * @param data - performance curve library object data
     * @param scenarioId - scenario object id
     */
    static addOrUpdatePerformanceCurveLibrary(data: PerformanceCurveLibrary, scenarioId: string): AxiosPromise {
        return coreAxiosInstance
        .post(`${API.PerformanceCurveLibraryController}/AddOrUpdatePerformanceCurveLibrary/${scenarioId}`, data);
    }

    /**
     * Deletes a performance library
     * @param libraryId - performance curve library object id
     */
    static deletePerformanceCurveLibrary(libraryId: string): AxiosPromise {
        return coreAxiosInstance
        .delete(`${API.PerformanceCurveLibraryController}/DeletePerformanceCurveLibrary/${libraryId}`);
    }
}
