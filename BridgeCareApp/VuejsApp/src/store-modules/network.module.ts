import {Network} from '@/shared/models/iAM/network';
import NetworkService from '../services/network.service';
import {clone} from 'ramda';
import {AxiosResponse} from 'axios';
import {hasValue} from '@/shared/utils/has-value-util';
import { NewNetwork } from '@/shared/models/iAM/newNetwork';
import prepend from 'ramda/es/prepend';
import { bridgecareCoreAxiosInstance } from '@/shared/utils/axios-instance';

const state = {
    networks: [] as Network[],
    newNetworks: [] as NewNetwork[]
};

// When we completely move to new Aggregation and Analysis. "NewNetwork" is the class which maps to the API
// "NewNetwork" will be used in networksMutator as well
const mutations = {
    networksMutator(state: any, networks: NewNetwork[]) {
        state.newNetworks = clone(networks);
    },
    createdNetworkMutator(state: any, createdNetwork: NewNetwork) {
        state.newNetworks = prepend(createdNetwork, state.newNetworks);
    }
};

const actions = {
    async getNetworks({commit}: any) {
        await NetworkService.getNetworks()
            .then((response: AxiosResponse<Network[]>) => {
                if (hasValue(response, 'data')) {
                    commit('networksMutator', response.data);
                }
            });
    },

    async createNetwork({dispatch, commit}: any, payload: any){
        return await NetworkService.createNetwork(payload.networkName)
            .then((response: AxiosResponse<NewNetwork>) => {
                if (hasValue(response, 'data')) {
                    const network: NewNetwork = response.data;
                    var networkObj: NewNetwork = {
                        ...response.data,
                        name: payload.createNetworkData.name
                    }
                    commit('createdNetworkMutator', networkObj);
                    dispatch('setSuccessMessage', {message: 'Successfully created network'});
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
