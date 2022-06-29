import {AxiosPromise} from 'axios';
import {API, coreAxiosInstance} from '@/shared/utils/axios-instance';
import { Datasource } from '@/shared/models/iAM/data-source';


export default class DataSourceService {
    static getDataSourceTypes(): AxiosPromise {
        return coreAxiosInstance.get(`${API.DataSource}/GetDataSourceTypes`);
    }
    static getDataSources(): AxiosPromise {
        return coreAxiosInstance.get(`${API.DataSource}/GetDataSources`);
    }
    static upsertSqlDatasource(
        data: Datasource,
    ): AxiosPromise {
        return coreAxiosInstance.post(
            `${API.DataSource}/UpsertSqlDataSource/`,
            data
        );
    }
    static upsertExcelDatasource(
        data: Datasource,
    ): AxiosPromise {
        return coreAxiosInstance.post(
            `${API.DataSource}/UpsertExcelDataSource/`,
            data
        );
    }
    static getExcelSpreadsheetColumnHeaders(
        datasourceId: string
    ): AxiosPromise {
        return coreAxiosInstance.get(
            `${API.RawData}/GetExcelSpreadsheetColumnHeaders/${datasourceId}`
        );
    }
    static importExcelSpreadsheet(
        file: File,
        id: string,
    ) {
        let formData = new FormData();
        formData.append('file', file);
        return coreAxiosInstance.post(
            `${API.RawData}/ImportExcelSpreadsheet/${id}`,
            formData,
            {headers: {'Content-Type': 'multipart/form-data'}},
        );
    }
}