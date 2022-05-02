import { BenefitQuantifier, Network } from '@/shared/models/iAM/network';
import NetworkService from '../services/network.service';
import { any, clone, find, findIndex, propEq, update } from 'ramda';
import { AxiosResponse } from 'axios';
import { hasValue } from '@/shared/utils/has-value-util';
import prepend from 'ramda/es/prepend';
import { http2XX } from '@/shared/utils/http-utils';
import { NetworkRollupDetail } from '@/shared/models/iAM/network-rollup-detail';

const state = {
    networks: [] as Network[],
    compatibleNetworks: [] as Network[]
};

const mutations = {
    networksMutator(state: any, networks: Network[]) {
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
    async getNetworks({commit}: any) {
        await NetworkService.getNetworks()
            .then((response: AxiosResponse<any[]>) => {
                if (hasValue(response, 'data')) {
                    commit('networksMutator', response.data as Network[]);
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
        return await NetworkService.createNetwork(payload.name)
            .then((response: AxiosResponse) => {
                if (hasValue(response, 'data')) {
                    const network: Network = response.data;
                    commit('createdNetworkMutator', network);
                    dispatch('addSuccessNotification', {
                        message: 'Network created',
                    });
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
};

const getters = {};

export default {
    state,
    getters,
    actions,
    mutations,
};
