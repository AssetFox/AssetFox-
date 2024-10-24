import {AxiosResponse} from 'axios';
import {clone} from 'ramda';
import {hasValue} from '@/shared/utils/has-value-util';
import AnalysisMethodService from '@/services/analysis-method.service';
import {AnalysisMethod, emptyAnalysisMethod} from '@/shared/models/iAM/analysis-method';
import {http2XX} from '@/shared/utils/http-utils';
import mitt, { Emitter, EventType } from 'mitt';
import Vue, { computed, ref, shallowReactive, shallowRef, watch, onMounted, onBeforeUnmount, inject } from 'vue'; 


const $emitter = inject('emitter') as Emitter<Record<EventType, unknown>>;

const state = {
    analysisMethod: clone(emptyAnalysisMethod) as AnalysisMethod,
    simulationAnalysisSetting: true
};

const mutations = {
    analysisMethodMutator(state: any, analysisMethod: AnalysisMethod) {
        state.analysisMethod = clone(analysisMethod);
    },
    simulationAnalysisMethodMutator(state: any, isSettingStored: true)
    {
        state.simulationAnalysisSetting = isSettingStored;
    }
};

const actions = {
    async getAnalysisMethod({commit}: any, payload: any) {
        await AnalysisMethodService.getAnalysisMethod(payload.scenarioId)
            .then((response: AxiosResponse) => {
                if (hasValue(response, 'data')) {
                    commit('analysisMethodMutator', response.data as AnalysisMethod);
                }
            });
    },
    async getSimulationAnalysisSetting({ commit }: any, payload: any) {
        try {
            const response: AxiosResponse = await AnalysisMethodService.getSimulationAnalysisSetting(payload.scenarioId);
                if (response.data === false) {
                commit('simulationAnalysisMethodMutator', false);
            } else if (response.data && typeof response.data === 'object') {
                if ('isSettingStored' in response.data) {
                    commit('simulationAnalysisMethodMutator', response.data.isSettingStored);
                }
            }
        } catch (error) {
            console.error('Error fetching analysis setting:', error);
            commit('simulationAnalysisMethodMutator', false); 
        }
    },
    async upsertAnalysisMethod({dispatch, commit}: any, payload: any) {
        return await AnalysisMethodService.upsetAnalysisMethod(payload.analysisMethod, payload.scenarioId)
            .then((response: AxiosResponse) => {
                if (hasValue(response, 'status') && http2XX.test(response.status.toString())) {
                    commit('analysisMethodMutator', payload.analysisMethod);
                    dispatch('addSuccessNotification', {
                        message: 'Upserted analysis method',
                });
                }
                return response.data;
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
