import {AxiosPromise} from 'axios';
import {PerformanceCurveLibrary} from '@/shared/models/iAM/performance';
import {axiosInstance, nodejsAxiosInstance} from '@/shared/utils/axios-instance';
import {convertFromVueToMongo} from '@/shared/utils/mongo-model-conversion-utils';


export default class PerformanceCurvesEditorService {
    /**
     * Gets all performance Libraries a user can read/edit
     */
    static getPerformanceCurveLibraries(): AxiosPromise {
        return nodejsAxiosInstance.get('/api/GetPerformanceLibraries');
    }

    /**
     * Creates a performance library
     * @param data The performance library create data
     */
    static createPerformanceCurveLibrary(data: PerformanceCurveLibrary): AxiosPromise {
        return nodejsAxiosInstance.post('/api/CreatePerformanceLibrary', convertFromVueToMongo(data));
    }

    /**
     * Updates a performance library
     * @param data The performance library update data
     */
    static updatePerformanceCurveLibrary(data: PerformanceCurveLibrary): AxiosPromise {
        return nodejsAxiosInstance.put('/api/UpdatePerformanceLibrary', convertFromVueToMongo(data));
    }

    /**
     * Deletes a performance library
     * @param data The performance library to delete
     */
    static deletePerformanceCurveLibrary(data: PerformanceCurveLibrary): AxiosPromise {
        return nodejsAxiosInstance.delete(`/api/DeletePerformanceLibrary/${data.id}`);
    }

    /**
     * Gets a scenario's performance library
     * @param selectedScenarioId Scenario object id
     */
    static getScenarioPerformanceCurveLibrary(selectedScenarioId: number): AxiosPromise {
        return axiosInstance.get(`/api/GetScenarioPerformanceLibrary/${selectedScenarioId}`);
    }

    /**
     * Saves a scenario performance library
     * @param data The scenario performance library upsert data
     */
    static saveScenarioPerformanceCurveLibrary(data: PerformanceCurveLibrary, mongoId: string): AxiosPromise {
        // Node API call is to update last modified date. (THe date is set in the nodejs app)
        nodejsAxiosInstance.put(`/api/UpdateMongoScenario/${mongoId}`);
        return axiosInstance.post('/api/SaveScenarioPerformanceLibrary', data);
    }
}
