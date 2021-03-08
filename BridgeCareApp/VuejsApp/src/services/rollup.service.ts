import {AxiosPromise, AxiosResponse} from 'axios';
import {axiosInstance, coreAxiosInstance, nodejsAxiosInstance} from '@/shared/utils/axios-instance';
import {hasValue} from '@/shared/utils/has-value-util';
import {any, propEq} from 'ramda';
import {Rollup} from '@/shared/models/iAM/rollup';
import { Network } from '@/shared/models/iAM/network';
import { http2XX } from '@/shared/utils/http-utils';

export default class RollupService {
    static getMongoRollups(): AxiosPromise {
        return new Promise<AxiosResponse<Rollup[]>>((resolve) => {
            nodejsAxiosInstance.get('api/GetMongoRollups')
                .then((response: AxiosResponse<Rollup[]>) => {
                    if (hasValue(response)) {
                        return resolve(response);
                    }
                })
                .catch((error: any) => {
                    return resolve(error.response);
                });
        });
    }

    static getLegacyNetworks(networks: Rollup[]): AxiosPromise {
        return new Promise<AxiosResponse<Rollup[]>>((resolve) => {
            axiosInstance.get('api/GetNetworks')
                .then((responseLegacy: AxiosResponse<Rollup[]>) => {
                    if (hasValue(responseLegacy)) {
                        var resultant: Rollup[] = [];
                        responseLegacy.data.forEach(network => {
                            if (!any(propEq('networkId', network.networkId), networks)) {
                                network.rollupStatus = 'N/A';
                                resultant.push(network);
                            }
                        });

                        if (resultant.length != 0) {
                            nodejsAxiosInstance.post('api/AddLegacyNetworks', resultant)
                                .then((res: AxiosResponse<Rollup[]>) => {
                                    if (hasValue(res)) {
                                        return resolve(res);
                                    }
                                })
                                .catch((error: any) => {
                                    return resolve(error.response);
                                });
                        }
                    }
                })
                .catch((error: any) => {
                    return resolve(error.response);
                });
        });
    }

    static aggregateNetworkData(networkId: string) : AxiosPromise {
        return new Promise<AxiosResponse<string>>((resolve) => {
            coreAxiosInstance.post(`api/Aggregation/AggregateNetworkData/${networkId}`)
            .then((response: AxiosResponse<string>) => {
                if (hasValue(response, 'status') && http2XX.test(response.status.toString())){
                    return resolve(response);
                }
            })
            .catch((error: any) => {
                return resolve(error.response);
            });
        });
    }

    static rollupNetwork(selectedNetwork: Rollup): AxiosPromise {
        return axiosInstance.post('/api/RunRollup', selectedNetwork);
    }
}
