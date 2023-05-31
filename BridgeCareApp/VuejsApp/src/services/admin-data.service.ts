import {AxiosPromise} from 'axios';
import {API, coreAxiosInstance} from '@/shared/utils/axios-instance';

export default class AdminDataService {
    static getKeyFields(): AxiosPromise {
        return coreAxiosInstance.get(`${API.AdminData}/GetKeyFields`);
    }
    static setKeyFields(keyFields: string): AxiosPromise {
        return coreAxiosInstance.post(`${API.AdminData}/SetKeyFields/${keyFields}`);
    }
    static getPrimaryNetwork(): AxiosPromise {
        return coreAxiosInstance.get(`${API.AdminData}/GetPrimaryNetwork`);
    }
    static setPrimaryNetwork(name: string): AxiosPromise {
        return coreAxiosInstance.post(`${API.AdminData}/SetPrimaryNetwork/${name}`);
    }
    static getSimulationReportNames(): AxiosPromise {
        return coreAxiosInstance.get(`${API.AdminData}/GetSimulationReportNames`);
    }
    static setSimulationReports(simulationReports: string): AxiosPromise {
        return coreAxiosInstance.post(`${API.AdminData}/SetSimulationReports/${simulationReports}`);
    }
    static getInventoryReports(): AxiosPromise {
        return coreAxiosInstance.get(`${API.AdminData}/GetInventoryReports`);
    }
    static setInventoryReports(reportNames: string): AxiosPromise {
        return coreAxiosInstance.post(`${API.AdminData}/SetInventoryReports/${reportNames}`);
    }
    static GetAttributeName(): AxiosPromise {
        return coreAxiosInstance.get(`${API.AdminData}/GetAttributeName`);
    }    
}