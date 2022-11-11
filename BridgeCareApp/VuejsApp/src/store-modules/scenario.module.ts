import {CloneScenarioData, emptyScenario, QueuedSimulation, Scenario} from '@/shared/models/iAM/scenario';
import ScenarioService from '@/services/scenario.service';
import {AxiosResponse} from 'axios';
import {any, clone, find, findIndex, prepend, propEq, reject, update} from 'ramda';
import {hasValue} from '@/shared/utils/has-value-util';
import {http2XX} from '@/shared/utils/http-utils';
import {SimulationAnalysisDetail} from '@/shared/models/iAM/simulation-analysis-detail';
import {SimulationReportDetail} from '@/shared/models/iAM/simulation-report-detail';
import { PagingPage, PagingRequest } from '@/shared/models/iAM/paging';

const state = {
    scenarios: [] as Scenario[],
    queuedSimulations: [] as QueuedSimulation[],
    currentSharedScenariosPage: [] as Scenario[],
    currentUserScenarioPage: [] as Scenario[],
    currentSimulationQueuePage: [] as QueuedSimulation[],
    totalSharedScenarios: 0 as number,
    totalUserScenarios: 0 as number,
    totalQueuedSimulations: 0 as number,
    selectedScenario: clone(emptyScenario) as Scenario,
    currentUserOrSharedScenario: clone(emptyScenario) as Scenario,
};

const mutations = {
    scenariosMutator(state: any, scenarios: Scenario[]) {
        state.scenarios = clone(scenarios);
    },
    UserScenarioPageMutator(state: any, scenarios: PagingPage<Scenario>){
        state.currentUserScenarioPage = clone(scenarios.items);
        state.totalUserScenarios = scenarios.totalItems;
    },
    SharedScenarioPageMutator(state: any, scenarios: PagingPage<Scenario>){
        state.currentSharedScenariosPage = clone(scenarios.items);
        state.totalSharedScenarios = scenarios.totalItems;
    },
    UserUserOrSharedScenarioMutator(state: any, scenario: Scenario){
        state.currentUserOrSharedScenario = clone(scenario);        
    },
    SimulationQueuePageMutator(state: any, queuedSimulations: PagingPage<QueuedSimulation>){
        state.currentSimulationQueuePage = clone(queuedSimulations.items);
        state.totalQueuedSimulations = queuedSimulations.totalItems;
    },
    selectedScenarioMutator(state: any, id: string) {
        if (any(propEq('id', id), state.currentSharedScenariosPage)) {
            state.selectedScenario = find(propEq('id', id), state.currentSharedScenariosPage) as Scenario;
        } 
        else if(any(propEq('id', id), state.currentUserScenarioPage)) {
            state.selectedScenario = find(propEq('id', id), state.currentUserScenarioPage) as Scenario;
        }
        else {
            state.selectedScenario = state.currentUserOrSharedScenario;
        }            
        //else {
          //  state.selectedScenario = clone(emptyScenario);
        //}
    },
    simulationAnalysisDetailMutator(state: any, simulationAnalysisDetail: SimulationAnalysisDetail) {
        if (any(propEq('id', simulationAnalysisDetail.simulationId), state.currentSharedScenariosPage)) {
            const updatedScenario: Scenario = find(propEq('id', simulationAnalysisDetail.simulationId), state.currentSharedScenariosPage) as Scenario;
            updatedScenario.lastRun = simulationAnalysisDetail.lastRun;
            updatedScenario.status = simulationAnalysisDetail.status;
            updatedScenario.runTime = simulationAnalysisDetail.runTime;

            state.currentSharedScenariosPage = update(
                findIndex(propEq('id', updatedScenario.id), state.currentSharedScenariosPage),
                updatedScenario,
                state.currentSharedScenariosPage
            );
            
        }
        if(any(propEq('id', simulationAnalysisDetail.simulationId), state.currentUserScenarioPage)) {
            const updatedScenario: Scenario = find(propEq('id', simulationAnalysisDetail.simulationId), state.currentUserScenarioPage) as Scenario;
            updatedScenario.lastRun = simulationAnalysisDetail.lastRun;
            updatedScenario.status = simulationAnalysisDetail.status;
            updatedScenario.runTime = simulationAnalysisDetail.runTime;

            state.currentUserScenarioPage = update(
                findIndex(propEq('id', updatedScenario.id), state.currentUserScenarioPage),
                updatedScenario,
                state.currentUserScenarioPage
            );         
        }
        if(any(propEq('id', simulationAnalysisDetail.simulationId), state.currentSimulationQueuePage)) {
            const updatedSimulation: QueuedSimulation = find(propEq('id', simulationAnalysisDetail.simulationId), state.currentSimulationQueuePage) as QueuedSimulation;
            updatedSimulation.status = simulationAnalysisDetail.status;

            state.currentSimulationQueuePage = update(
                findIndex(propEq('id', updatedSimulation.id), state.currentSimulationQueuePage),
                updatedSimulation,
                state.currentSimulationQueuePage
            );         
        }        
    },
    simulationReportDetailMutator(state: any, simulationReportDetail: SimulationReportDetail) {
        if (any(propEq('id', simulationReportDetail.simulationId), state.currentSharedScenariosPage)) {
            const updatedScenario: Scenario = find(propEq('id', simulationReportDetail.simulationId), state.currentSharedScenariosPage) as Scenario;
            updatedScenario.reportStatus = simulationReportDetail.status;

            state.currentSharedScenariosPage = update(
                findIndex(propEq('id', updatedScenario.id), state.currentSharedScenariosPage),
                updatedScenario,
                state.currentSharedScenariosPage
            );
        }
        if (any(propEq('id', simulationReportDetail.simulationId), state.currentUserScenarioPage)) {
            const updatedScenario: Scenario = find(propEq('id', simulationReportDetail.simulationId), state.currentUserScenarioPage) as Scenario;
            updatedScenario.reportStatus = simulationReportDetail.status;

            state.currentUserScenarioPage = update(
                findIndex(propEq('id', updatedScenario.id), state.currentUserScenarioPage),
                updatedScenario,
                state.currentUserScenarioPage
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
        await ScenarioService.getScenarios()
            .then((response: AxiosResponse) => {
                if (hasValue(response, 'data')) {
                    commit('scenariosMutator', response.data as Scenario[]);
                }
            });
    },
    async getSharedScenariosPage({commit}: any, payload: PagingRequest<Scenario>) {
        await ScenarioService.getSharedScenariosPage(payload)
            .then((response: AxiosResponse) => {
                if (hasValue(response, 'data')) {
                    commit('SharedScenarioPageMutator', response.data as PagingPage<Scenario>);
                }
            });
    },
    async getSimulationQueuePage({commit}: any, payload: PagingRequest<QueuedSimulation>) {
        await ScenarioService.getSimulationQueuePage(payload)
            .then((response: AxiosResponse) => {
                if (hasValue(response, 'data')) {
                    commit('SimulationQueuePageMutator', response.data as PagingPage<QueuedSimulation>);
                }
            });
    },    
    async getUserScenariosPage({commit}: any, payload: PagingRequest<Scenario>) {
        await ScenarioService.getUserScenariosPage(payload)
            .then((response: AxiosResponse) => {
                if (hasValue(response, 'data')) {
                    commit('UserScenarioPageMutator', response.data as PagingPage<Scenario>);
                }
            });
    },
    async runSimulation({dispatch, commit}: any, payload: any) {
        await ScenarioService.runSimulation(payload.networkId, payload.scenarioId)
            .then((response: AxiosResponse) => {
                if (hasValue(response, 'status') && http2XX.test(response.status.toString())) {
                dispatch('addSuccessNotification', {
                    message: 'Simulation analysis started',
                });
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
                    dispatch('addSuccessNotification', {
                    message: 'Created scenario',
                    });
                }
            });
    },
    async cloneScenario({dispatch, commit}: any, payload: any) {
        let cloneScenarioData: CloneScenarioData = {
            scenarioId: payload.scenarioId,
            networkId: payload.networkId,
            scenarioName: payload.scenarioName
        }

        return await ScenarioService.cloneScenario(cloneScenarioData)
            .then((response: AxiosResponse) => {
                if (hasValue(response, 'data')) {
                    dispatch('addSuccessNotification', {
                        message: 'Cloned scenario',
                    });
                }
            },
        );
    },
    async updateScenario({dispatch, commit}: any, payload: any) {
        return await ScenarioService.updateScenario(payload.scenario)
            .then((response: AxiosResponse) => {
                if (hasValue(response, 'data')) {
                    dispatch('addSuccessNotification', {
                        message: 'Updated scenario',
                    });
                }
            },
        );
    },
    async deleteScenario({dispatch, state, commit}: any, payload: any) {
        return await ScenarioService.deleteScenario(payload.scenarioId)
            .then((response: AxiosResponse) => {
                if (hasValue(response, 'status') && http2XX.test(response.status.toString())) {
                    dispatch('addSuccessNotification', {
                        message: 'Deleted scenario',
                    });
                }
            },
        );
    },
    async cancelSimulation({dispatch, state, commit}: any, payload: any) {
        return await ScenarioService.cancelSimulation(payload.simulationId)
            .then((response: AxiosResponse) => {
                if (hasValue(response, 'status') && http2XX.test(response.status.toString())) {
                    dispatch('addSuccessNotification', {
                        message: 'Simulation analysis canceled',
                    });
                }
            },
        );
    },
    async getCurrentUserOrSharedScenario({commit}: any, payload: any) {   
        await ScenarioService.getCurrentUserOrSharedScenario(payload.simulationId)
            .then((response: AxiosResponse) => {
                if (hasValue(response, 'data')) {
                    commit('UserUserOrSharedScenarioMutator', response.data as Scenario);
                }
            }
        );
   },
};

const getters = {};

export default {
    state,
    getters,
    actions,
    mutations,
};