import {AxiosPromise} from 'axios';
import {API, coreAxiosInstance} from '@/shared/utils/axios-instance';

export default class AdminSiteSettingsService {
    static getAgencyLogo(): AxiosPromise {
        return coreAxiosInstance.get(`${API.AdminSettings}/GetAgencyLogo`);
    }
    static getProductLogo(): AxiosPromise {
        return coreAxiosInstance.get(`${API.AdminSettings}/GetImplementationLogo`);
    }
    static getImplementationName(): AxiosPromise {
        return coreAxiosInstance.get(`${API.AdminSettings}/GetImplementationName`);
    }
    static importImplementationName(input: string): AxiosPromise {
        return coreAxiosInstance.post(
            `${API.AdminSettings}/SetImplementationName/` + input,
            input,
            {headers: {'Content-Type': 'text/plain'}},
        );
    }
    static importAgencyLogo(file: File): AxiosPromise {
        var reader = new FileReader();
        reader.readAsDataURL(file);
        let formData = new FormData();
        formData.append('file', file);
        return coreAxiosInstance.post(
            `${API.AdminSettings}/SetAgencyLogo`,
            formData,
            {headers: {'Content-Type': 'multipart/form-data'}},
        );
    }
    static importProductLogo(file: File): AxiosPromise {
        var reader = new FileReader();
        reader.readAsDataURL(file);
        let formData = new FormData();
        formData.append('file', file);
        return coreAxiosInstance.post(
            `${API.AdminSettings}/SetImplementationLogo`,
            formData,
            {headers: {'Content-Type': 'multipart/form-data'}},
        );
    }
    static getAdminContactEmail(): AxiosPromise {
        return coreAxiosInstance.get(`${API.AdminSettings}/GetAdminContactEmail`);
    }
    static setAdminContactEmail(email: string): AxiosPromise {
        return coreAxiosInstance.post(
          `${API.AdminSettings}/SetAdminContactEmail/` + encodeURIComponent(email),
          null,
          { headers: { 'Content-Type': 'application/json' } }
        );
    }
}