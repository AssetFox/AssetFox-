import {AxiosResponse} from 'axios';
import {clone} from 'ramda';
import {hasValue} from '@/shared/utils/has-value-util';
import AnalysisMethodService from '@/services/analysis-method.service';
import {AnalysisMethod, emptyAnalysisMethod} from '@/shared/models/iAM/analysis-method';
import {http2XX} from '@/shared/utils/http-utils';

const state = {
    analysisMethod: clone(emptyAnalysisMethod) as AnalysisMethod
};

const mutations = {
    analysisMethodMutator(state: any, analysisMethod: AnalysisMethod) {
        state.analysisMethod = clone(analysisMethod);
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
    async upsertAnalysisMethod({dispatch, commit}: any, payload: any) {
        return await AnalysisMethodService.upsetAnalysisMethod(payload.analysisMethod, payload.scenarioId)
            .then((response: AxiosResponse) => {
                if (hasValue(response, 'status') && http2XX.test(response.status.toString())) {
                    commit('analysisMethodMutator', payload.analysisMethod);
                    dispatch('addSuccessNotification', {
                        message: 'Upserted analysis method',
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
