import {AxiosPromise} from 'axios';
import {API, coreAxiosInstance} from '@/shared/utils/axios-instance';

export default class NetworkService {
    static getNetworks(): AxiosPromise {
        return coreAxiosInstance.get(`${API.NetworkController}/GetAllNetworks`);
    }

    static createNetwork(networkName: any): AxiosPromise {
        return coreAxiosInstance.post(`${API.NetworkController}/CreateNetwork/${networkName}`);
    }
}
