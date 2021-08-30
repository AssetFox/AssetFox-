import {AxiosPromise} from 'axios';
import {API, coreAxiosInstance} from '@/shared/utils/axios-instance';

export default class AnalysisDefaultDataService {
    static getAnalysisDefaultData(): AxiosPromise {
        return coreAxiosInstance.get(`${API.AnalysisDefaultData}/GetAnalysisDefaultData`);
    }
}
