import {AxiosResponse} from 'axios';
import AdminSiteSettingsService from '@/services/admin-site-settings.service';
import {hasValue} from '@/shared/utils/has-value-util';

const state = {
    agencyLogo: '',
    productLogo: '',
    implementationName: '',
    isSuccessfulImport: false as boolean,
};

const mutations = {
    agencyLogoMutator(state: any, agencyLogo: File) {
        state.agencyLogo = agencyLogo;
    },
    implementationNameMutator(state: any, implementationName: String) {
        state.implementationName = implementationName;
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
        await AdminSiteSettingsService.getAgencyLogo()
        .then((response: AxiosResponse<string>) => {
            if (hasValue(response, 'data')) {
                commit('agencyLogoMutator', response.data);
            }
        });
    },
    async getImplementationName({commit}: any) {
        await AdminSiteSettingsService.getImplementationName()
        .then((response: AxiosResponse<string>) => {
            if (hasValue(response, 'data')) {
                commit('implementationNameMutator', response.data);
            }
        });
    },
    async getProductLogo({commit}: any) {
        await AdminSiteSettingsService.getProductLogo()
        .then((response: AxiosResponse<string>) => {
            if (hasValue(response, 'data')) {
                commit('productLogoMutator', response.data);
            }
        });
    },
    async importAgencyLogo({commit, dispatch}: any, payload: File) {
        await AdminSiteSettingsService.importProductLogo(payload)
        .then((response: AxiosResponse) => {
            if (hasValue(response, 'data')) {
                commit('isSuccessfulImportMutator', true);
                dispatch('addSuccessNotification',{
                    message: 'Agency logo imported'
                });
            }
        });
    },
    async importImplementationName({commit, dispatch}: any, payload: String) {
        await AdminSiteSettingsService.importImplementationName(payload)
        .then((response: AxiosResponse) => {
            if (hasValue(response, 'data')) {
                commit('isSuccessfulImportMutator', true);
                dispatch('addSuccessNotification',{
                    message: 'Implementation Name imported'
                });
            }
        });
    },
    async importProductLogo({commit, dispatch}: any, payload: File) {
        await AdminSiteSettingsService.importProductLogo(payload)
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
