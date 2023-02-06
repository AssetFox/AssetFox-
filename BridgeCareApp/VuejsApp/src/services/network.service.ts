import {AxiosPromise} from 'axios';
import {API, coreAxiosInstance} from '@/shared/utils/axios-instance';
import { BenefitQuantifier } from '@/shared/models/iAM/network';
import { Attribute } from '@/shared/models/iAM/attribute';

export default class NetworkService {
    static getNetworks(): AxiosPromise {
        return coreAxiosInstance.get(`${API.Network}/GetAllNetworks`);
    }

    static createNetwork(networkName: any, data: Attribute): AxiosPromise {
        return coreAxiosInstance.post(`${API.Network}/CreateNetwork/${networkName}`, data);
    }

    static deleteNetwork(netowrkId: string): AxiosPromise {
        return coreAxiosInstance.delete(`${API.Network}/DeleteNetwork/${netowrkId}`)
    }

    static getCompatibleNetworks(networkId: any): AxiosPromise {
        return coreAxiosInstance.post(`${API.Network}/GetCompatibleNetworks/${networkId}`)
    }

    static upsertBenefitQuantifier(data: BenefitQuantifier): AxiosPromise {
        return coreAxiosInstance.post(`${API.Network}/UpsertBenefitQuantifier`, data);
    }
}
