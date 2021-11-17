import {emptyScenario, Scenario} from '@/shared/models/iAM/scenario';
import ScenarioService from '@/services/scenario.service';
import {AxiosResponse} from 'axios';
import {any, clone, find, findIndex, prepend, propEq, reject, update} from 'ramda';
import {hasValue} from '@/shared/utils/has-value-util';
import {http2XX} from '@/shared/utils/http-utils';
import ReportsService from '@/services/reports.service';
import {SimulationAnalysisDetail} from '@/shared/models/iAM/simulation-analysis-detail';
import {SimulationReportDetail} from '@/shared/models/iAM/simulation-report-detail';

const state = {
    scenarios: [] as Scenario[],
    selectedScenario: clone(emptyScenario) as Scenario
};

const mutations = {
    scenariosMutator(state: any, scenarios: Scenario[]) {
        state.scenarios = clone(scenarios);
    },
    createdScenarioMutator(state: any, createdScenario: Scenario) {
        state.scenarios = prepend(createdScenario, state.scenarios);
    },
    updatedScenarioMutator(state: any, updatedScenario: Scenario) {
        if (any(propEq('id', updatedScenario.id), state.scenarios)) {
            state.scenarios = update(
                findIndex(propEq('id', updatedScenario.id), state.scenarios),
                updatedScenario,
                state.scenarios
            );
        }
    },
    deletedScenarioMutator(state: any, id: string) {
        if (any(propEq('id', id), state.scenarios)) {
            state.scenarios = reject(propEq('id', id), state.scenarios);
        }
    },
    selectedScenarioMutator(state: any, id: string) {
        if (any(propEq('id', id), state.scenarios)) {
            state.selectedScenario = find(propEq('id', id), state.scenarios) as Scenario;
        } else {
            state.selectedScenario = clone(emptyScenario);
        }
    },
    simulationAnalysisDetailMutator(state: any, simulationAnalysisDetail: SimulationAnalysisDetail) {
        if (any(propEq('id', simulationAnalysisDetail.simulationId), state.scenarios)) {
            const updatedScenario: Scenario = find(propEq('id', simulationAnalysisDetail.simulationId), state.scenarios) as Scenario;
            updatedScenario.lastRun = simulationAnalysisDetail.lastRun;
            updatedScenario.status = simulationAnalysisDetail.status;
            updatedScenario.runTime = simulationAnalysisDetail.runTime;

            state.scenarios = update(
                findIndex(propEq('id', updatedScenario.id), state.scenarios),
                updatedScenario,
                state.scenarios
            );
        }
    },
    simulationReportDetailMutator(state: any, simulationReportDetail: SimulationReportDetail) {
        if (any(propEq('id', simulationReportDetail.simulationId), state.scenarios)) {
            const updatedScenario: Scenario = find(propEq('id', simulationReportDetail.simulationId), state.scenarios) as Scenario;
            updatedScenario.reportStatus = simulationReportDetail.status;

            state.scenarios = update(
                findIndex(propEq('id', updatedScenario.id), state.scenarios),
                updatedScenario,
                state.scenarios
            );
        }
    }
};

const actions = {
    selectScenario({commit}: any, payload: any) {
        commit('selectedScenarioMutator', payload.scenarioId);
    },
    updateSimulationAnalysisDetail({commit}: any, payload: any) {
        commit('simulationAnalysisDetailMutator', payload.simulationAnalysisDetail);
    },
    updateSimulationReportDetail({commit}: any, payload: any) {
        commit('simulationReportDetailMutator', payload.simulationReportDetail);
    },
    async getScenarios({commit}: any, payload: any) {
        await ScenarioService.getScenarios(payload.networkId)
            .then((response: AxiosResponse) => {
                if (hasValue(response, 'data')) {
                    commit('scenariosMutator', response.data as Scenario[]);
                }
            });
    },
    async runSimulation({dispatch, commit}: any, payload: any) {
        await ScenarioService.runSimulation(payload.networkId, payload.scenarioId)
            .then((response: AxiosResponse) => {
                if (hasValue(response, 'status') && http2XX.test(response.status.toString())) {
                    dispatch('setSuccessMessage', {message: 'Simulation analysis complete'});
                }
            });
    },
    async migrateLegacySimulationData({dispatch, commit}: any, payload: any) {
        await ScenarioService.migrateLegacySimulationData(payload.simulationId)
            .then((response: AxiosResponse) => {
                if (hasValue(response, 'status') && http2XX.test(response.status.toString())) {
                    dispatch('getScenarios', {networkId: payload.networkId});
                }
            });
    },
    async createScenario({dispatch, commit}: any, payload: any) {
        return await ScenarioService.createScenario(payload.scenario, payload.networkId)
            .then((response: AxiosResponse) => {
                if (hasValue(response, 'data')) {
                    commit('createdScenarioMutator', response.data as Scenario);
                    dispatch('setSuccessMessage', {message: 'Created scenario'});
                }
            });
    },
    async cloneScenario({dispatch, commit}: any, payload: any) {
        return await ScenarioService.cloneScenario(payload.scenarioId)
            .then((response: AxiosResponse) => {
                if (hasValue(response, 'data')) {
                    commit('createdScenarioMutator', response.data as Scenario);
                    dispatch('setSuccessMessage', {message: 'Cloned scenario'});
                }
            });
    },
    async updateScenario({dispatch, commit}: any, payload: any) {
        return await ScenarioService.updateScenario(payload.scenario)
            .then((response: AxiosResponse) => {
                if (hasValue(response, 'data')) {
                    commit('updatedScenarioMutator', response.data);
                    dispatch('setSuccessMessage', {message: 'Updated scenario'});
                }
            });
    },
    async deleteScenario({dispatch, state, commit}: any, payload: any) {
        return await ScenarioService.deleteScenario(payload.scenarioId)
            .then((response: AxiosResponse) => {
                if (hasValue(response, 'status') && http2XX.test(response.status.toString())) {
                    commit('deletedScenarioMutator', payload.scenarioId);
                    dispatch('setSuccessMessage', {message: 'Deleted scenario'});
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
