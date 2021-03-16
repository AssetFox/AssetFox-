import {Network} from '@/shared/models/iAM/network';
import NetworkService from '../services/network.service';
import {clone} from 'ramda';
import {AxiosResponse} from 'axios';
import {hasValue} from '@/shared/utils/has-value-util';
import prepend from 'ramda/es/prepend';

const state = {
    networks: [] as Network[]
};

const mutations = {
    networksMutator(state: any, networks: Network[]) {
        state.networks = clone(networks);
    },
    createdNetworkMutator(state: any, createdNetwork: Network) {
        state.newNetworks = prepend(createdNetwork, state.networks);
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
    async createNetwork({dispatch, commit}: any, payload: any) {
        return await NetworkService.createNetwork(payload.name)
            .then((response: AxiosResponse) => {
                if (hasValue(response, 'data')) {
                    const network: Network = response.data;
                    commit('createdNetworkMutator', network);
                    dispatch('setSuccessMessage', {message: 'Created network'});
                }
            });
    }
};

const getters = {};

export default {
    state,
    getters,
    actions,
    mutations
};
