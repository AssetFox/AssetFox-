import {AxiosPromise} from 'axios';
import {API, coreAxiosInstance} from '@/shared/utils/axios-instance';

export default class NetworkService {
    static getNetworks(): AxiosPromise {
        return coreAxiosInstance.get(`${API.Network}/GetAllNetworks`);
    }

    static createNetwork(networkName: any): AxiosPromise {
        return coreAxiosInstance.post(`${API.Network}/CreateNetwork/${networkName}`);
    }
}
