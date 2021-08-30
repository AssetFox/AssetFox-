import {AxiosResponse} from 'axios';
import {clone} from 'ramda';
import {hasValue} from '@/shared/utils/has-value-util';
import AnalysisDefaultDataService from '@/services/analysisDefaultData.service';
import {AnalysisDefaultData, emptyAnalysisDefaultData} from '@/shared/models/iAM/DefaultData';

const state = {
    analysisDefaultData: clone(emptyAnalysisDefaultData) as AnalysisDefaultData
};

const mutations = {
    analysisDefaultDataMutator(state: any, analysisDefaultData: AnalysisDefaultData) {
        state.analysisDefaultData = clone(analysisDefaultData);
    }
};

const actions = {
    async getAnalysisDefaultData({commit}: any) {
        await AnalysisDefaultDataService.getAnalysisDefaultData()
            .then((response: AxiosResponse) => {
                if (hasValue(response, 'data')) {
                    commit('analysisDefaultDataMutator', response.data as AnalysisDefaultData);
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
