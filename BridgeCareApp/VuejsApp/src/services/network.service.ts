import {AxiosPromise, AxiosResponse} from 'axios';
import {axiosInstance, bridgecareCoreAxiosInstance} from '@/shared/utils/axios-instance';
import { NetworkCreationData } from '@/shared/models/modals/network-creation-data';
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

    static createNetwork(createScenarioData: NetworkCreationData): AxiosPromise {
        var name = JSON.stringify(createScenarioData.name);
        return new Promise<AxiosResponse<NewNetwork>>((resolve) => {
            bridgecareCoreAxiosInstance.request({
                method: 'post',
                url: 'api/Network/CreateNetwork',
                data: name
            }).then((response: AxiosResponse<NewNetwork>) => {
                    if (hasValue(response)) {
                        return resolve(response);
                    }
                });
        });
    }
}
