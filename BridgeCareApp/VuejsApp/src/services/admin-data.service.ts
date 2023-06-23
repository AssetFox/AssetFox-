import {AxiosPromise} from 'axios';
import {API, coreAxiosInstance} from '@/shared/utils/axios-instance';

export default class AdminDataService {
    static getKeyFields(): AxiosPromise {
        return coreAxiosInstance.get(`${API.AdminData}/GetKeyFields`);
    }
    static setKeyFields(keyFields: string): AxiosPromise {
        return coreAxiosInstance.post(`${API.AdminData}/SetKeyFields/${keyFields}`);
    }
    static getRawDataKeyFields(): AxiosPromise {
        return coreAxiosInstance.get(`${API.AdminData}/GetRawDataKeyFields`);
    }
    static setRawDataKeyFields(keyFields: string): AxiosPromise {
        return coreAxiosInstance.post(`${API.AdminData}/SetRawDataKeyFields/${keyFields}`);
    }
    static getPrimaryNetwork(): AxiosPromise {
        return coreAxiosInstance.get(`${API.AdminData}/GetPrimaryNetwork`);
    }
    static setPrimaryNetwork(name: string): AxiosPromise {
        return coreAxiosInstance.post(`${API.AdminData}/SetPrimaryNetwork/${name}`);
    }
    static getAvailableReportNames(): AxiosPromise {
        return coreAxiosInstance.get(`${API.AdminData}/GetAvailableReports`);
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
    
    static getConstraintType(): AxiosPromise {
        return coreAxiosInstance.get(`${API.AdminData}/GetConstraintType`);
    }
    static setConstraintType(constraintType: string): AxiosPromise {
        return coreAxiosInstance.post(`${API.AdminData}/SetConstraintType/${constraintType}`);
    }
}