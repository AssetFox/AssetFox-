import {AxiosPromise} from 'axios';
import {PerformanceCurveLibrary} from '@/shared/models/iAM/performance';
import {API, coreAxiosInstance} from '@/shared/utils/axios-instance';


export default class PerformanceCurveService {
    static getPerformanceCurveLibraries(): AxiosPromise {
        return coreAxiosInstance.get(`${API.PerformanceCurveController}/GetPerformanceCurveLibraries`);
    }

    static upsertPerformanceCurveLibrary(data: PerformanceCurveLibrary, scenarioId: string): AxiosPromise {
        return coreAxiosInstance
        .post(`${API.PerformanceCurveController}/UpsertPerformanceCurveLibrary/${scenarioId}`, data);
    }

    static deletePerformanceCurveLibrary(libraryId: string): AxiosPromise {
        return coreAxiosInstance
        .delete(`${API.PerformanceCurveController}/DeletePerformanceCurveLibrary/${libraryId}`);
    }
}
