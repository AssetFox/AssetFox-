import { BenefitQuantifier, emptyNetwork, Network } from '@/shared/models/iAM/network';
import NetworkService from '../services/network.service';
import { any, append, clone, find, findIndex, propEq, reject, update } from 'ramda';
import { AxiosResponse } from 'axios';
import { hasValue } from '@/shared/utils/has-value-util';
import prepend from 'ramda/es/prepend';
import { http2XX } from '@/shared/utils/http-utils';
import AggregationService from '@/services/aggregation.service';

const state = {
    networks: [] as Network[],
    selectedNetwork: clone(emptyNetwork),
    compatibleNetworks: [] as Network[]
};

const mutations = {
    selectedNetworkMutator(state: any, networkId: string) {
        if (any(propEq('id', networkId), state.networks)) {
            state.selectedNetwork = find(
                propEq('id', networkId),
                state.networks,
            );
        } else {
            state.selectedNetwork = clone(
                emptyNetwork,
            );
        }
    },
    networksMutator(state: any, network: Network) {
        state.networks = any(
            propEq('id', network.id),
            state.networks,
        )
            ? update(
                  findIndex(
                      propEq('id', network.id),
                      state.networks,
                  ),
                  network,
                  state.networks,
              )
            : append(network, state.networks);
        (state.networks as Network[]).sort((one, two) => (one.name.toUpperCase() < two.name.toUpperCase() ? -1 : 1));
    },
    networksMutatorClone(state: any, networks: Network[]) {
        state.networks = clone(networks);
    },
    compatibleNetworksMutator(state: any, compatibleNetworks: Network[]) {
        state.compatibleNetworks = clone(compatibleNetworks);
    },
    createdNetworkMutator(state: any, createdNetwork: Network) {
        state.newNetworks = prepend(createdNetwork, state.networks);
    },
    benefitQuantifierMutator(state: any, benefitQuantifier: BenefitQuantifier) {
        if (any(propEq('id', benefitQuantifier.networkId), state.networks)) {
            const network: Network = find(propEq('id', benefitQuantifier.networkId), state.networks) as Network;

            state.networks = update(
              findIndex(propEq('id', network.id), state.networks),
              {...network, benefitQuantifier: benefitQuantifier},
              state.networks
            );
        }
    }
};

const actions = {
    selectNetwork({ commit }: any, networkId: string) {
        commit('selectedNetworkMutator', networkId);
    },
    async getNetworks({commit}: any) {
        await NetworkService.getNetworks()
            .then((response: AxiosResponse<any[]>) => {
                if (hasValue(response, 'data')) {
                    commit('networksMutatorClone', response.data as Network[]);
                }
            });
    },
    async getCompatibleNetworks({commit}: any, payload: any) {
        await NetworkService.getCompatibleNetworks(payload.networkId)
            .then((response: AxiosResponse) => {
                if (hasValue(response, 'data')) {
                    commit('compatibleNetworksMutator', response.data as Network[]);
                }
            });
    },
    async createNetwork({dispatch, commit}: any, payload: any) {
        return await NetworkService.createNetwork(payload.network.name, payload.parameters)
            .then((response: AxiosResponse) => {
                if (hasValue(response, 'data')) {
                    const message: string = any(
                        propEq('id', response.data),
                        state.networks,
                    )
                        ? 'Updated network'
                        : 'Added network';

                    //commit('networksMutator', payload.network);
                    dispatch('getNetworks').then(() => {
                        commit('selectedNetworkMutator', response.data);
                    });
                    //commit('selectedNetworkMutator', response.data);
                    dispatch('addSuccessNotification', { message: message });
                }
            },
        );
    },
    async deleteNetwork({dispatch, commit, state}: any, networkId: any) {
        return await NetworkService.deleteNetwork(networkId)
            .then((response: AxiosResponse) => {
                if (
                    hasValue(response, 'status') &&
                    http2XX.test(response.status.toString())
                )  {
                    dispatch('addSuccessNotification', {
                        message: 'Deleted network',
                    });
                    const networks: Network[] = reject(
                        propEq('id', networkId),
                        state.networks,
                    );
                    commit('networksMutatorClone', networks)
                }
            },
        );
    },
    async upsertBenefitQuantifier({dispatch, commit}: any, payload: any) {
        return await NetworkService.upsertBenefitQuantifier(payload.benefitQuantifier)
          .then((response: AxiosResponse) => {
              if (hasValue(response, 'status') && http2XX.test(response.status.toString())) {
                commit('benefitQuantifierMutator', payload.benefitQuantifier);
                dispatch('addSuccessNotification', {
                    message: 'Benefit quantifier upsertted',
                });
            }
        });
    },
    async aggregateNetworkData({dispatch, commit}: any, payload: any){
        return await AggregationService.AggregateNetworkData(payload.attributes, payload.networkId)
            .then((response: AxiosResponse) => {
                if (hasValue(response, 'status') && http2XX.test(response.status.toString())) {               
                  dispatch('addSuccessNotification', {
                      message: 'Aggregation Started',
                  });
              }
          });
    }
};

const getters = {};

export default {
    state,
    getters,
    actions,
    mutations,
};
