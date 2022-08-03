import {AxiosPromise} from 'axios';
import {API, coreAxiosInstance} from '@/shared/utils/axios-instance';
import { Attribute } from '@/shared/models/iAM/attribute';

export default class AggregationService {
    static AggregateNetworkData(data: Attribute[], networkId: string): AxiosPromise {
        return coreAxiosInstance.post(`${API.Aggregation}/AggregateNetworkData/${networkId}`, data);
    }
}