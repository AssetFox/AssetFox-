import {CloneScenarioData, emptyScenario, QueuedWork, Scenario} from '@/shared/models/iAM/scenario';
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
    currentSharedScenariosPage: [] as Scenario[],
    currentUserScenarioPage: [] as Scenario[],  
    totalSharedScenarios: 0 as number,
    totalUserScenarios: 0 as number,   
    selectedScenario: clone(emptyScenario) as Scenario,
    currentUserOrSharedScenario: clone(emptyScenario) as Scenario,
    queuedSimulations: [] as QueuedWork[],
    currentWorkQueuePage: [] as QueuedWork[],
    totalQueuedSimulations: 0 as number,
    fastQueueitems: [] as QueuedWork[],
    currentFastWorkQueuePage: [] as QueuedWork[],
    totalFastQueuedItems: 0 as number,
    simulationRunSettingId: 0 as number,
    simulationRunSettingName: '' as string,
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
    selectedScenarioMutator(state: any, id: string) {
        if (any(propEq('id', id), state.currentSharedScenariosPage)) {
            state.selectedScenario = find(propEq('id', id), state.currentSharedScenariosPage) as Scenario;
        } 
        else if(any(propEq('id', id), state.currentUserScenarioPage)) {
            state.selectedScenario = find(propEq('id', id), state.currentUserScenarioPage) as Scenario;
        }
        else {
            state.selectedScenario = id == emptyScenario.id ? clone(emptyScenario) : state.currentUserOrSharedScenario;
        }
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
        if(any(propEq('id', simulationAnalysisDetail.simulationId), state.currentWorkQueuePage)) {
            const updatedSimulation: QueuedWork = find(propEq('id', simulationAnalysisDetail.simulationId), state.currentWorkQueuePage) as QueuedWork;
            updatedSimulation.status = simulationAnalysisDetail.status;

            state.currentWorkQueuePage = update(
                findIndex(propEq('id', updatedSimulation.id), state.currentWorkQueuePage),
                updatedSimulation,
                state.currentWorkQueuePage
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
    },
    workQueuePageMutator(state: any, queuedSimulations: PagingPage<QueuedWork>){
        state.currentWorkQueuePage = clone(queuedSimulations.items);
        state.totalQueuedSimulations = queuedSimulations.totalItems;
    },
    workQueStatusUpdateMutator(state: any, queuedWorkUpdated: QueuedWork) {
        if (any(propEq('id', queuedWorkUpdated.id), state.currentWorkQueuePage)) {
            const updatedQueuedWork: QueuedWork = find(propEq('id', queuedWorkUpdated.id), state.currentWorkQueuePage) as QueuedWork;
            updatedQueuedWork.status = queuedWorkUpdated.status;

            state.currentWorkQueuePage = update(
                findIndex(propEq('id', updatedQueuedWork.id), state.currentWorkQueuePage),
                updatedQueuedWork,
                state.currentWorkQueuePage
            );           
        }
    },
    fastWorkQueuePageMutator(state: any, queuedItems: PagingPage<QueuedWork>){
        state.currentFastWorkQueuePage = clone(queuedItems.items);
        state.totalFastQueuedItems = queuedItems.totalItems;
    },
    fastWorkQueStatusUpdateMutator(state: any, queuedWorkUpdated: QueuedWork) {
        if (any(propEq('id', queuedWorkUpdated.id), state.currentFastWorkQueuePage)) {
            const updatedQueuedWork: QueuedWork = find(propEq('id', queuedWorkUpdated.id), state.currentFastWorkQueuePage) as QueuedWork;
            updatedQueuedWork.status = queuedWorkUpdated.status;

            state.currentFastWorkQueuePage = update(
                findIndex(propEq('id', updatedQueuedWork.id), state.currentFastWorkQueuePage),
                updatedQueuedWork,
                state.currentFastWorkQueuePage
            );           
        }
    },
    setSimulationRunSettingId(state: any, id: any) {
        state.simulationRunSettingId = id;
    },
    setSimulationRunSettingName(state: any, id: any) {
        state.simulationRunSettingName = id;
    },
};

const actions = {
    selectScenario({commit}: any, payload: any) {
        commit('selectedScenarioMutator', payload.scenarioId);
    },
    updateSimulationAnalysisDetail({commit}: any, payload: any) {
        commit('simulationAnalysisDetailMutator', payload.simulationAnalysisDetail);
    },
    updateQueuedWorkStatus({commit}: any, payload: any) {
        commit('workQueStatusUpdateMutator', payload.workQueueStatusUpdate);
    },
    updateFastQueuedWorkStatus({commit}: any, payload: any) {
        commit('fastWorkQueStatusUpdateMutator', payload.workQueueStatusUpdate);
    },
    updateSimulationReportDetail({commit}: any, payload: any) {
        commit('simulationReportDetailMutator', payload.simulationReportDetail);
    },
    updateSimulationRunSettingId({ commit }: any, payload: any) {
        commit('setSimulationRunSettingId', payload);
    },
    updateSimulationRunSettingName({ commit }: any, payload: any) {
        commit('setSimulationRunSettingName', payload);
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
    async getWorkQueuePage({commit}: any, payload: PagingRequest<QueuedWork>) {
        await ScenarioService.getWorkQueuePage(payload)
            .then((response: AxiosResponse) => {
                if (hasValue(response, 'data')) {
                    commit('workQueuePageMutator', response.data as PagingPage<QueuedWork>);
                }
            });
    },    
    async getFastWorkQueuePage({commit}: any, payload: PagingRequest<QueuedWork>) {
        await ScenarioService.getFastWorkQueuePage(payload)
            .then((response: AxiosResponse) => {
                if (hasValue(response, 'data')) {
                    commit('fastWorkQueuePageMutator', response.data as PagingPage<QueuedWork>);
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
                    message: 'Created scenario ' + payload.scenario.name,
                    });
                }
            });
    },
    async cloneScenario({dispatch, commit}: any, payload: any) {
        let cloneScenarioData: CloneScenarioData = {
            scenarioId: payload.scenarioId,
            destinationNetworkId: payload.destinationNetworkId,
            networkId: payload.networkId,
            scenarioName: payload.scenarioName
        }

        return await ScenarioService.cloneScenario(cloneScenarioData)
            .then((response: AxiosResponse) => {
                if (hasValue(response, 'data')) {
                    dispatch('addSuccessNotification', {
                        message: 'Cloned scenario ' + cloneScenarioData.scenarioName,
                    });
                }
            },
        );
    },
    async cloneScenarioWithDestinationNetwork({dispatch, commit}: any, payload: any) {
        let cloneScenarioData: CloneScenarioData = {
            scenarioId: payload.id,
            destinationNetworkId: payload.destinationNetworkId,
            networkId: payload.networkId,
            scenarioName: payload.name
        }

        return await ScenarioService.cloneScenario(cloneScenarioData)
            .then((response: AxiosResponse) => {
                if (hasValue(response, 'data')) {
                    dispatch('addSuccessNotification', {
                        message: 'Cloned scenario ' + cloneScenarioData.scenarioName,
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
                        message: 'Updated scenario ' + payload.scenario.name,
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
                        message: 'Deleting scenario ' + payload.scenarioName,
                    });
                }
            },
        );
    },
    async cancelWorkQueueItem({dispatch, state, commit}: any, payload: any) {
        return await ScenarioService.cancelWorkQueueItem(payload.simulationId)
            .then((response: AxiosResponse) => {
                if (hasValue(response, 'status') && http2XX.test(response.status.toString())) {
                    dispatch('addSuccessNotification', {
                        message: 'Canceling work queue operation',
                    });
                }
            },
        );
    },
    async cancelFastQueueItem({dispatch, state, commit}: any, workId:string) {
        return await ScenarioService.cancelFastQueueItem(workId)
            .then((response: AxiosResponse) => {
                if (hasValue(response, 'status') && http2XX.test(response.status.toString())) {
                    dispatch('addSuccessNotification', {
                        message: 'Canceling work queue operation',
                    });
                }
            },
        );
    },
    async getCurrentUserOrSharedScenario({commit}: any, payload: any) {
        if(state.currentUserOrSharedScenario.id == emptyScenario.id)
        {
            await ScenarioService.getCurrentUserOrSharedScenario(payload.simulationId)
                .then((response: AxiosResponse) => {
                    if (hasValue(response, 'data')) {
                        commit('UserUserOrSharedScenarioMutator', response.data as Scenario);
                    }
                }
            );
        }
   },
   async getSimulationRunSetting({commit}: any, payload: any) {
        if(state.currentUserOrSharedScenario.id == emptyScenario.id)
        {
            await ScenarioService.getSimulationRunSetting(payload.simulationId)
                .then((response: AxiosResponse) => {
                    if (hasValue(response, 'data')) {
                        commit(response.data as Scenario);
                    }
                }
            );
        }
    },
    async GetSimulationRunSetting({commit}: any, payload: any) {
        await ScenarioService.getScenariosReportSettings()
            .then((response: AxiosResponse) => {
                if (hasValue(response, 'data')) {
                    commit('scenariosMutator', response.data as Scenario[]);
                }
            });
    },

};

const getters = { simulationRunSettingId: (state: { simulationRunSettingId: any; }) => state.simulationRunSettingId,
                    simulationRunSettingName: (state: { simulationRunSettingName: any; }) => state.simulationRunSettingName,};

export default {
    state,
    getters,
    actions,
    mutations,
};