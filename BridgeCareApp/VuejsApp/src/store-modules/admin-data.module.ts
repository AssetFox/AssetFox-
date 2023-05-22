import {AxiosResponse} from 'axios';
import {clone} from 'ramda';
import {hasValue} from '@/shared/utils/has-value-util';
import AnalysisMethodService from '@/services/analysis-method.service';
import {AnalysisMethod, emptyAnalysisMethod} from '@/shared/models/iAM/analysis-method';
import {http2XX} from '@/shared/utils/http-utils';
import { emptyNetwork, Network } from '@/shared/models/iAM/network';
import AdminDataService from '@/services/admin-data.service';

const state = {
    simulationReportNames: [] as string[],
    inventoryReportNames: [] as string[],
    primaryNetwork: clone(emptyNetwork) as Network,
    keyFields: [] as string[],
};

const mutations = {
    simulationReportsMutator(state: any, simulationReports: string[]) {
        state.simulationReportNames = clone(simulationReports);
    },
    inventoryReportsMutator(state: any, inventoryReports: string[]) {
        state.inventoryReportNames = clone(inventoryReports);
    },
    keyFieldsMutator(state: any, keyFields: string[]) {
        state.keyFields = clone(keyFields);
    },
    primaryNetworkMutator(state: any, network: Network) {
        state.primaryNetwork = clone(network);
    },
};

const actions = {
    async getSimulationReports({commit}: any) {
        await AdminDataService.getSimulationReportNames()
            .then((response: AxiosResponse<string[]>) => {
                if (hasValue(response, 'data')) {
                    commit('simulationReportsMutator', response.data);
                }
            });
    },
    async getInventoryReports({commit}: any) {//not in yet
        await AdminDataService.getSimulationReportNames()
            .then((response: AxiosResponse<string[]>) => {
                if (hasValue(response, 'data')) {
                    commit('inventoryReportsMutator', response.data);
                }
            });
    },
    async getPrimaryNetwork({commit}: any) {
        await AdminDataService.getSimulationReportNames()
            .then((response: AxiosResponse<string[]>) => {
                if (hasValue(response, 'data')) {
                    commit('simulationReportsMutator', response.data);
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
    async setSimulationReports( //not in yet
        { dispatch, commit }: any,
        reports: string,
    ) {
        await AdminDataService.SetInventoryReports(reports).then((response: AxiosResponse) => {
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
        await AdminDataService.SetInventoryReports(reports).then((response: AxiosResponse) => {
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
        network: Network,
    ) {
        await AdminDataService.setPrimaryNetwork(network.name).then((response: AxiosResponse) => {
            if (hasValue(response, 'status') && http2XX.test(response.status.toString())) {
                commit('primaryNetworkMutator', network);
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
};

const getters = {};

export default {
    state,
    getters,
    actions,
    mutations
};