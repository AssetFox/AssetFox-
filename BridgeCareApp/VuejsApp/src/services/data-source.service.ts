import {AxiosPromise} from 'axios';
import {API, coreAxiosInstance} from '@/shared/utils/axios-instance';


export default class DataSourceService {
    static getDataSourceTypes(): AxiosPromise {
        return coreAxiosInstance.get(`${API.DataSource}/GetDataSourceTypes`);
    }
    static getDataSources(): AxiosPromise {
        return coreAxiosInstance.get(`${API.DataSource}/GetDataSources`);
    }
}