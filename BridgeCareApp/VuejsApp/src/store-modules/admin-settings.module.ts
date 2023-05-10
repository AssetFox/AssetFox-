import {AxiosResponse} from 'axios';
import AdminSettingsService from '@/services/admin-settings.service';
import {hasValue} from '@/shared/utils/has-value-util';

const state = {
    agencyLogo: '',
    productLogo: '',
    isSuccessfulImport: false as boolean,
};

const mutations = {
    agencyLogoMutator(state: any, agencyLogo: File) {
        state.agencyLogo = agencyLogo;
    },
    productLogoMutator(state: any, productLogo: File) {
        state.productLogo = productLogo;
    },
    isSuccessfulImportMutator(state: any, isSuccessful: boolean) {
        state.isSuccessfulImport = isSuccessful;
    }
};

const actions = {
    async getAgencyLogo({commit}: any) {
        await AdminSettingsService.getAgencyLogo()
        .then((response: AxiosResponse<string>) => {
            if (hasValue(response, 'data')) {
                commit('agencyLogoMutator', response.data);
            }
        });
    },
    async getProductLogo({commit}: any) {
        await AdminSettingsService.getProductLogo()
        .then((response: AxiosResponse<string>) => {
            if (hasValue(response, 'data')) {
                commit('productLogoMutator', response.data);
            }
        });
    },
    async importAgencyLogo({commit, dispatch}: any, payload: File) {
        await AdminSettingsService.importProductLogo(payload)
        .then((response: AxiosResponse) => {
            if (hasValue(response, 'data')) {
                commit('isSuccessfulImportMutator', true);
                dispatch('addSuccessNotification',{
                    message: 'Agency logo imported'
                });
            }
        });
    },
    async importProductLogo({commit, dispatch}: any, payload: File) {
        await AdminSettingsService.importProductLogo(payload)
        .then((response: AxiosResponse) => {
            if (hasValue(response, 'data')) {
                commit('isSuccessfulImportMutator', true);
                dispatch('addSuccessNotification',{
                    message: 'Product logo imported'
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
    mutations
};
