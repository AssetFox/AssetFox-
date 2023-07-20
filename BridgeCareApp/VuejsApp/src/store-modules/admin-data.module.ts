import {AxiosResponse} from 'axios';
import {clone} from 'ramda';
import {hasValue} from '@/shared/utils/has-value-util';
import AnalysisMethodService from '@/services/analysis-method.service';
import {AnalysisMethod, emptyAnalysisMethod} from '@/shared/models/iAM/analysis-method';
import {http2XX} from '@/shared/utils/http-utils';
import { emptyNetwork, Network } from '@/shared/models/iAM/network';
import AdminDataService from '@/services/admin-data.service';

const state = {
    availableReportNames: [] as string[],
    simulationReportNames: [] as string[],
    inventoryReportNames: [] as string[],
    primaryNetwork: '' as string,
    rawdataNetwork: '' as string,
    keyFields: [] as string[],
    rawDataKeyFields: [] as string[],
    constraintType: '' as string,
};

const mutations = {
    availableReportsMutator(state: any, availableReports: string[]) {
        state.availableReportNames = availableReports !== null ? clone(availableReports) : [];
    },
    simulationReportsMutator(state: any, simulationReports: string[]) {
        state.simulationReportNames = simulationReports !== null ? clone(simulationReports) : [];
    },
    inventoryReportsMutator(state: any, inventoryReports: string[]) {
        state.inventoryReportNames = inventoryReports !== null ? clone(inventoryReports) : [];
    },
    keyFieldsMutator(state: any, keyFields: string[]) {
        state.keyFields = keyFields !== null ? clone(keyFields) : [];
    },
    rawDataKeyFieldsMutator(state: any, keyFields: string[]) {
        state.rawDataKeyFields = keyFields !== null ? clone(keyFields) : [];
    },
    primaryNetworkMutator(state: any, network: string) {
        state.primaryNetwork = network !== null ? network : '';
    },
    rawdataNetworkMutator(state: any, network: string) {
        state.rawdataNetwork = network !== null ? network : '';
    },
    constraintTypeMutator(state: any, constraintType: string) {
        state.constraintType = constraintType !== null ? constraintType : '';
    },
};

const actions = {
    async getAvailableReports({commit}: any) {
        await AdminDataService.getAvailableReportNames()
            .then((response: AxiosResponse<string[]>) => {
                if (hasValue(response, 'data')) {
                    commit('availableReportsMutator', response.data);
                }
            });
    },
    async getSimulationReports({commit}: any) {
        await AdminDataService.getSimulationReportNames()
            .then((response: AxiosResponse<string[]>) => {
                if (hasValue(response, 'data')) {
                    commit('simulationReportsMutator', response.data);
                }
            });
    },
    async getInventoryReports({commit}: any) {
        await AdminDataService.getInventoryReports()
            .then((response: AxiosResponse<string[]>) => {
                if (hasValue(response, 'data')) {
                    commit('inventoryReportsMutator', response.data);
                }
            });
    },
    async getPrimaryNetwork({commit}: any) {
        await AdminDataService.getPrimaryNetwork()
            .then((response: AxiosResponse<string>) => {
                if (hasValue(response, 'data')) {
                    commit('primaryNetworkMutator', response.data);
                }
            });
    },
    async getRawdataNetwork({commit}: any) {
        await AdminDataService.getRawdataNetwork()
            .then((response: AxiosResponse<string>) => {
                if (hasValue(response, 'data')) {
                    commit('rawdataNetworkMutator', response.data);
                }
            });
    },
    async getKeyFields({commit}: any) {
        await AdminDataService.getKeyFields()
            .then((response: AxiosResponse<string[]>) => {
                if (hasValue(response, 'data')) {
                    commit('keyFieldsMutator', response.data);
                }
            });
    },
    async getRawDataKeyFields({commit}: any) {
        await AdminDataService.getRawDataKeyFields()
            .then((response: AxiosResponse<string[]>) => {
                if (hasValue(response, 'data')) {
                    commit('rawDataKeyFieldsMutator', response.data);
                }
            });
    },
    async getConstraintType({commit}: any) {
        await AdminDataService.getConstraintType()
            .then((response: AxiosResponse<string>) => {
                if (hasValue(response, 'data')) {
                    commit('constraintTypeMutator', response.data);
                }
            });
    },
    async setSimulationReports(
        { dispatch, commit }: any,
        reports: string,
    ) {
        await AdminDataService.setSimulationReports(reports).then((response: AxiosResponse) => {
            if (hasValue(response, 'status') && http2XX.test(response.status.toString())) {
                commit('simulationReportsMutator', reports.split(','));
                dispatch('addSuccessNotification', {
                    message: 'Modified simulation reports',
                });
            }
        });
    },
    async setInventoryReports(
        { dispatch, commit }: any,
        reports: string,
    ) {
        await AdminDataService.setInventoryReports(reports).then((response: AxiosResponse) => {
            if (hasValue(response, 'status') && http2XX.test(response.status.toString())) {
                commit('inventoryReportsMutator', reports.split(','));
                dispatch('addSuccessNotification', {
                    message: 'Modified inventory reports',
                });
            }
        });
    },
    async setPrimaryNetwork(
        { dispatch, commit }: any,
        network: string,
    ) {
        await AdminDataService.setPrimaryNetwork(network).then((response: AxiosResponse) => {
            if (hasValue(response, 'status') && http2XX.test(response.status.toString())) {
                commit('primaryNetworkMutator', network);
                dispatch('addSuccessNotification', {
                    message: 'Modified primary network',
                });
            }
        });
    },
    async setRawdataNetwork(
        { dispatch, commit }: any,
        network: string,
    ) {
        await AdminDataService.setRawdataNetwork(network).then((response: AxiosResponse) => {
            if (hasValue(response, 'status') && http2XX.test(response.status.toString())) {
                commit('rawdataNetworkMutator', network);
                dispatch('addSuccessNotification', {
                    message: 'Modified primary network',
                });
            }
        });
    },
    async setKeyFields(
        { dispatch, commit }: any,
        keyFields: string,
    ) {
        await AdminDataService.setKeyFields(keyFields).then((response: AxiosResponse) => {
            if (hasValue(response, 'status') && http2XX.test(response.status.toString())) {
                commit('keyFieldsMutator', keyFields.split(','));
                dispatch('addSuccessNotification', {
                    message: 'Modified key fields',
                });
            }
        });
    },
    async setRawDataKeyFields(
        { dispatch, commit }: any,
        keyFields: string,
    ) {
        await AdminDataService.setRawDataKeyFields(keyFields).then((response: AxiosResponse) => {
            if (hasValue(response, 'status') && http2XX.test(response.status.toString())) {
                commit('rawDataKeyFieldsMutator', keyFields.split(','));
                dispatch('addSuccessNotification', {
                    message: 'Modified raw data key fields',
                });
            }
        });
    },
    async setConstraintType(
        { dispatch, commit }: any,
        constraintType: string,
    ) {
        await AdminDataService.setConstraintType(constraintType).then((response: AxiosResponse) => {
            if (hasValue(response, 'status') && http2XX.test(response.status.toString())) {
                commit('constraintTypeMutator', constraintType);
                dispatch('addSuccessNotification', {
                    message: 'Modified constraint type',
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
    mutations
};