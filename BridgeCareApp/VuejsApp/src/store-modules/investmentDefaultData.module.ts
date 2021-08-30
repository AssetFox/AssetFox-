import {AxiosResponse} from 'axios';
import {clone} from 'ramda';
import {hasValue} from '@/shared/utils/has-value-util';
import InvestmentDefaultDataService from '@/services/investmentDefaultData.service';
import {InvestmentDefaultData, emptyInvestmentDefaultData} from '@/shared/models/iAM/DefaultData';

const state = {
    investmentDefaultData: clone(emptyInvestmentDefaultData) as InvestmentDefaultData
};

const mutations = {
    investmentDefaultDataMutator(state: any, investmentDefaultData: InvestmentDefaultData) {
        state.investmentDefaultData = clone(investmentDefaultData);
    }
};

const actions = {
    async getInvestmentDefaultData({commit}: any) {
        await InvestmentDefaultDataService.getInvestmentDefaultData()
            .then((response: AxiosResponse) => {
                if (hasValue(response, 'data')) {
                    commit('investmentDefaultDataMutator', response.data as InvestmentDefaultData);
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
