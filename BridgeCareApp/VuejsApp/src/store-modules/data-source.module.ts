import {clone, any, propEq, update, findIndex, append} from 'ramda';
import {AxiosResponse} from 'axios';
import {Datasource, DataSourceType} from '@/shared/models/iAM/data-source';
import {hasValue} from '@/shared/utils/has-value-util';
import DataSourceService from '@/services/data-source.service';

const state = {
    dataSources: [] as Datasource[],
    dataSourceTypes: [] as string[]
};

const mutations = {
    dataSourceMutator(state: any, dataSources: Datasource[]) {
        state.dataSources = clone(dataSources);
    },
    dataSourceTypesMutator(state: any, dataSourceTypes: string[]) {
        state.dataSourceTypes = clone(dataSourceTypes);
    }
}
const actions = {
    async getDataSources({commit}: any) {
        await DataSourceService.getDataSources()
        .then((response: AxiosResponse<Datasource[]>) => {
            if (hasValue(response, 'data')) {
                commit('dataSourceMutator', response.data);
            }
        });
    },
    async getDataSourceTypes({commit}: any) {
        await DataSourceService.getDataSourceTypes()
        .then((response: AxiosResponse<string[]>) => {
            if (hasValue(response, 'data')) {
                commit('dataSourceTypesMutator', response.data);
            }
        });
    }
};
export default {
    state,
    actions,
    mutations
};