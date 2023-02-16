import {clone, any, propEq, update, findIndex, append, reject} from 'ramda';
import {AxiosResponse} from 'axios';
import {
    Datasource, 
    DataSourceType, 
    ExcelDataSource, 
    RawDataColumns, 
    SqlDataSource, 
    SqlCommandResponse, 
    emptySqlCommandResponse,
    TestConnection,
    noneDatasource
} from '@/shared/models/iAM/data-source';
import {hasValue} from '@/shared/utils/has-value-util';
import DataSourceService from '@/services/data-source.service';
import { http2XX } from '@/shared/utils/http-utils';
import CommittedProjectsService from '@/services/committed-projects.service';

const state = {
    dataSources: [] as Datasource[],
    dataSourceTypes: [] as string[],
    excelColumns: [] as any[],
    isSuccessfulImport: false as boolean,
    sqlCommandResponse: emptySqlCommandResponse as SqlCommandResponse
};

const mutations = {
    dataSourceMutator(state: any, dataSources: Datasource[]) {
        dataSources.push(noneDatasource);
        state.dataSources = clone(dataSources);
    },
    dataSourceTypesMutator(state: any, dataSourceTypes: string[]) {
        state.dataSourceTypes = clone(dataSourceTypes);
    },
    excelColumnsMutator(state: any, excelColumns: RawDataColumns[]) {
        state.excelColumns = clone(excelColumns);
    },
    isSuccessfulImportMutator(state: any, isSuccessful: boolean) {
        state.isSuccessfulImport = isSuccessful;
    },
    checkSqlCommandMutator(state: any, sqlresponse: SqlCommandResponse) {
        state.sqlCommandResponse = sqlresponse;
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
    },
    async upsertSqlDataSource(
        {dispatch, commit }: any,
        payload: SqlDataSource,
    ) {
        await DataSourceService.upsertSqlDatasource(
            payload
        ).then((response: AxiosResponse) => {
            if (
                hasValue(response, 'status') &&
                http2XX.test(response.status.toString())
            ) {
                dispatch('addSuccessNotification', {
                    message: 'Modified data sources',
                });
            }
        });
    },
    async upsertExcelDataSource(
        {dispatch, commit }: any,
        payload: ExcelDataSource,
    ) {
        await DataSourceService.upsertExcelDatasource(
            payload
        ).then((response: AxiosResponse) => {
            if (
                hasValue(response, 'status') &&
                http2XX.test(response.status.toString())
            ) {
                dispatch('addSuccessNotification', {
                    message: 'Modified data sources',
                });
            }
        });
    },
    async deleteDataSource(
        {dispatch, commit, state  }: any,
        payload: string,
    ) {
        await DataSourceService.DeleteDataSource(
            payload
        ).then((response: AxiosResponse) => {
            if (
                hasValue(response, 'status') &&
                http2XX.test(response.status.toString())
            ) {

                dispatch('addSuccessNotification', {
                    message: 'Deleted data sources',
                });
                const dataSources: Datasource[] = reject(
                    propEq('id', payload),
                    state.dataSources,
                );
                commit('dataSourceMutator', dataSources)
            }
        });
    },
    async checkSqlCommand(
        {commit}: any,
        payload: TestConnection
    ) {
        await DataSourceService.checkSqlConnection(
            payload
        ).then((response: AxiosResponse<SqlCommandResponse>) => {
            if (
                hasValue(response, 'status')
            ) {
                commit('checkSqlCommandMutator', response.data);
            }
        });
    },
    async getExcelSpreadsheetColumnHeaders(
        {commit}: any,
        payload: string
    ) {
        await DataSourceService.getExcelSpreadsheetColumnHeaders(
            payload
        ).then((response: AxiosResponse) => {
        if (
            hasValue(response, 'status')
        ) {
            commit('excelColumnsMutator', response.data);
        }
    });
    },
    async importExcelSpreadsheetFile(
        {commit, dispatch}: any,
        payload: any,
    ) {
        await DataSourceService.importExcelSpreadsheet(
            payload.file,
            payload.id
        ).then((response: AxiosResponse) => {
            if (hasValue(response, 'data')) {
                commit('isSuccessfulImportMutator', true);
                dispatch('addSuccessNotification',{
                    message: 'Excel file imported'
                });
            }
        });
    },
};
export default {
    state,
    actions,
    mutations
};