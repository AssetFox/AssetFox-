import {Analysis, emptyAnalysis, emptyScenario, Scenario} from '@/shared/models/iAM/scenario';
import ScenarioService from '@/services/scenario.service';
import {AxiosResponse} from 'axios';
import {any, clone, findIndex, propEq, remove, find, update, reject, prepend} from 'ramda';
import {hasValue} from '@/shared/utils/has-value-util';
import {http2XX} from '@/shared/utils/http-utils';
import AnalysisMethodService from '@/services/analysis-method.service';
import ReportsService from '@/services/reports.service';
import {convertFromMongoToVue} from '@/shared/utils/mongo-model-conversion-utils';
import moment from 'moment';
import {getUserName} from '@/shared/utils/get-user-info';
import {Simulation} from '@/shared/models/iAM/simulation';
import {SimulationAnalysisDetail} from '@/shared/models/iAM/simulation-analysis-detail';

const state = {
    scenarios: [] as Scenario[],
    benefitAttributes: [] as string[],
    selectedScenario: clone(emptyScenario) as Scenario,
    analysis: clone(emptyAnalysis) as Analysis,
    missingSummaryReportAttributes: [] as string[]
};

const mutations = {
    scenariosMutator(state: any, scenarios: Scenario[]) {
        state.scenarios = clone(scenarios);
    },
    createdScenarioMutator(state: any, createdScenario: Scenario) {
        state.scenarios = prepend(createdScenario, state.scenarios);
    },
    updatedScenarioMutator(state: any, updatedScenario: Scenario) {
        if (any(propEq('simulationId', updatedScenario.simulationId), state.scenarios)) {
            state.scenarios = update(
                findIndex(propEq('simulationId', updatedScenario.simulationId), state.scenarios),
                updatedScenario,
                state.scenarios
            );
        }
    },
    addMigratedScenariosMutator(state: any, migratedScenarios: Scenario[]) {
        migratedScenarios.forEach((migratedScenario: Scenario) => {
            if (any(propEq('id', migratedScenario.id), state.scenarios)) {
                state.scenarios = reject(propEq('id', migratedScenario.id), state.scenarios);
            }
            state.scenarios = prepend(migratedScenario, state.scenarios);
        });
    },
    removeScenarioMutator(state: any, simulationId: number) {
        if (any(propEq('simulationId', simulationId), state.scenarios)) {
            state.scenarios = reject(propEq('simulationId', simulationId), state.scenarios);
        }
    },
    selectedScenarioMutator(state: any, simulationId: number) {
        if (any((scenario: Scenario) => scenario.simulationId === simulationId), state.scenarios) {
            state.selectedScenario = find(propEq('simulationId', simulationId), state.scenarios) as Scenario;
        } else {
            state.selectedScenario = clone(emptyScenario);
        }
    },
    analysisMutator(state: any, analysis: Analysis) {
        state.analysis = clone(analysis);
    },
    missingSummaryReportAttributesMutator(state: any, missingAttributes: string[]) {
        state.missingSummaryReportAttributes = clone(missingAttributes);
    },
    simulationAnalysisDetailMutator(state: any, simulationAnalysisDetail: SimulationAnalysisDetail) {
        if (any(propEq('id', simulationAnalysisDetail.simulationId), state.scenarios)) {
            const updatedScenario: Scenario = find(propEq('id', simulationAnalysisDetail.simulationId), state.scenarios) as Scenario;
            updatedScenario.lastRun = simulationAnalysisDetail.lastRun;
            updatedScenario.status = simulationAnalysisDetail.status;
            updatedScenario.runTime = simulationAnalysisDetail.runTime;

            state.scenarios = update(
                findIndex(propEq('simulationId', updatedScenario.simulationId), state.scenarios),
                updatedScenario,
                state.scenarios
            );
        }
    }
};

const actions = {
    selectScenario({commit}: any, payload: any) {
        commit('selectedScenarioMutator', payload.simulationId);
    },
    updateSimulationAnalysisDetail({commit}: any, payload: any) {
        commit('simulationAnalysisDetailMutator', payload.simulationAnalysisDetail);
    },
    async getMongoScenarios({dispatch, commit}: any) {
        return await ScenarioService.getMongoScenarios()
            .then((response: AxiosResponse) => {
                if (hasValue(response, 'data')) {
                    commit('scenariosMutator', response.data.map((data: any) => convertFromMongoToVue(data)));
                }
            })
            .then(() => {
                dispatch('getMigratedData', {simulationId: process.env.VUE_APP_HARDCODED_SCENARIOID_FROM_MSSQL});
            });
    },
    async getLegacyScenarios() {
        await ScenarioService.getLegacyScenarios();
    },
    async runSimulation({dispatch, commit}: any, payload: any) {
        await ScenarioService.updateScenarioStatus('Queued', payload.selectedScenario.simulationId);
        await ScenarioService.runScenarioSimulation(payload.selectedScenario, payload.userId)
            .then((response: AxiosResponse) => {
                if (hasValue(response, 'status') && http2XX.test(response.status.toString())) {
                    dispatch('setSuccessMessage', {message: 'Simulation queued'});
                }
            });
    },

    async runNewSimulation({dispatch, commit}: any, payload: any){
        await ScenarioService.runNewScenarioSimulation(payload.networkId, payload.selectedScenarioId)
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
                    dispatch('getMigratedData');
                }
            });
    },

    async getMigratedData({dispatch, commit}: any, payload: any) {
        await ScenarioService.getMigratedData()
            .then((response: AxiosResponse) => {
                if (hasValue(response, 'data')) {
                    const scenarios: Scenario[] = response.data.map((scenario: Scenario) => ({
                        ...scenario,
                        owner: getUserName(),
                        creator: getUserName()
                    })) as Scenario[];
                    commit('addMigratedScenariosMutator', scenarios);
                }
            });
    },

    async createScenario({dispatch, commit}: any, payload: any) {
        return await ScenarioService.createScenario(payload.createScenarioData, payload.userId)
            .then((response: AxiosResponse) => {
                if (hasValue(response, 'data')) {
                    commit('createdScenarioMutator', convertFromMongoToVue(response.data));
                    dispatch('setSuccessMessage', {message: 'Successfully created scenario'});
                }
            });
    },
    async cloneScenario({dispatch, commit}: any, payload: any) {
        return await ScenarioService.cloneScenario(payload.scenarioId)
            .then((response: AxiosResponse) => {
                if (hasValue(response, 'data')) {
                    commit('createdScenarioMutator', convertFromMongoToVue(response.data));
                    dispatch('setSuccessMessage', {message: 'Successfully cloned scenario'});
                }
            });
    },
    async updateScenario({dispatch, commit}: any, payload: any) {
        return await ScenarioService.updateScenario(payload.updateScenarioData, payload.scenarioId)
            .then((response: AxiosResponse<Scenario>) => {
                if (hasValue(response, 'data')) {
                    commit('updatedScenarioMutator', convertFromMongoToVue(response.data));
                    dispatch('setSuccessMessage', {message: 'Successfully updated scenario'});
                }
            });
    },
    async updateScenarioUsers({dispatch, commit}: any, payload: any) {
        return await ScenarioService.updateScenarioUsers(payload.scenario)
            .then(() => {
                commit('updatedScenarioMutator', payload.scenario);
                dispatch('setSuccessMessage', {message: 'Successfully updated scenario sharing settings'});
            });
    },
    async deleteScenario({dispatch, state, commit}: any, payload: any) {
        return await ScenarioService.deleteScenario(payload.simulationId, payload.scenarioId)
            .then((response: AxiosResponse) => {
                if (hasValue(response, 'status') && http2XX.test(response.status.toString())) {
                    commit('removeScenarioMutator', payload.simulationId);
                    dispatch('setSuccessMessage', {message: 'Successfully deleted scenario'});
                }
            });
    },
    async deleteDuplicateMongoScenario({dispatch, state, commit}: any, payload: any){
        return await ScenarioService.deleteDuplicateMongoScenario(payload.scenarios)
        .then((response: AxiosResponse) => {
            if (hasValue(response, 'data')) {
                commit('scenariosMutator', response.data.map((data: any) => convertFromMongoToVue(data)));
            }
        });
    },
    async getScenarioAnalysis({commit}: any, payload: any) {
        await AnalysisMethodService.getScenarioAnalysisData(payload.selectedScenarioId)
            .then((response: AxiosResponse) => {
                if (hasValue(response, 'data')) {
                    const analysis: Analysis = {
                        ...response.data,
                        startYear: response.data.startYear > 0 ? response.data.startYear : moment().year
                    };
                    commit('analysisMutator', analysis);
                }
            });
    },
    async saveScenarioAnalysis({dispatch, commit}: any, payload: any) {
        await AnalysisMethodService.saveScenarioAnalysisData(payload.scenarioAnalysisData, payload.objectIdMOngoDBForScenario)
            .then((response: AxiosResponse) => {
                if (hasValue(response, 'status') && http2XX.test(response.status.toString())) {
                    commit('analysisMutator', payload.scenarioAnalysisData);
                    dispatch('setSuccessMessage', {message: 'Successfully saved scenario analysis'});
                }
            });
    },
    async getSummaryReportMissingAttributes({commit}: any, payload: any) {
        await ReportsService.getSummaryReportMissingAttributes(payload.selectedScenarioId, payload.selectedNetworkId)
            .then((response: AxiosResponse) => {
                if (hasValue(response, 'data')) {
                    commit('missingSummaryReportAttributesMutator', response.data);
                }
            });
    },
    async clearSummaryReportMissingAttributes({commit}: any) {
        commit('missingSummaryReportAttributesMutator', '');
    },
    async socket_scenarioStatus({dispatch, state, commit}: any, payload: any) {
        if (hasValue(payload, 'operationType')) {
            if (hasValue(payload, 'fullDocument')) {
                const scenario: Scenario = convertFromMongoToVue(payload.fullDocument);
                switch (payload.operationType) {
                    case 'update':
                    case 'replace':
                        commit('updatedScenarioMutator', scenario);
                        break;
                    case 'insert':
                        if (!any(propEq('id', scenario.id), state.scenarios)) {
                            commit('createdScenarioMutator', scenario);
                            dispatch('setInfoMessage', {message: `Scenario '${scenario.simulationName}' has been created from another source`});
                        }
                        break;
                }
            } else if (hasValue(payload, 'documentKey')) {
                if (any(propEq('id', payload.documentKey._id), state.scenarios)) {
                    const deletedScenario: Scenario = find(propEq('id', payload.documentKey._id), state.scenarios);
                    commit('removeScenarioMutator', {id: deletedScenario.simulationId, mongoId: payload.documentKey._id});
                    dispatch('setInfoMessage',
                        {message: `Scenario '${deletedScenario.simulationName}' has been deleted from another source`}
                    );
                }
            }
        }
    },
    async setScenarioUsers({commit}: any, payload: any) {
        await ScenarioService.setScenarioUsers(payload.scenarioId, payload.scenarioUsers);
    }
};

const getters = {};

export default {
    state,
    getters,
    actions,
    mutations
};
