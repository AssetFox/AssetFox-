import {AxiosPromise, AxiosResponse} from 'axios';
import {axiosInstance, bridgecareCoreAxiosInstance} from '@/shared/utils/axios-instance';
import { NewNetwork } from '@/shared/models/iAM/newNetwork';
import { hasValue } from '@/shared/utils/has-value-util';

export default class NetworkService {
    static getNetworks() : AxiosPromise {
        return new Promise<AxiosResponse<NewNetwork[]>>((resolve) => {
            bridgecareCoreAxiosInstance.get('api/Network/GetAllNetworks')
            .then((response: AxiosResponse<NewNetwork[]>) => {
                if(hasValue(response)){
                    return resolve(response);
                }
            })
            .catch((error: any) => {
                return resolve(error.response);
            });
        });
    }

    static createNetwork(networkName: any): AxiosPromise {
        return new Promise<AxiosResponse<NewNetwork>>((resolve) => {
            bridgecareCoreAxiosInstance.post(`api/Network/CreateNetwork/${networkName}`).then((response: AxiosResponse<NewNetwork>) => {
                    if (hasValue(response)) {
                        return resolve(response);
                    }
                });
        });
    }
}
