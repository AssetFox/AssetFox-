import {AxiosPromise} from 'axios';
import {API, coreAxiosInstance} from '@/shared/utils/axios-instance';
import {ExcelDataSource, SqlDataSource, TestConnection} from '@/shared/models/iAM/data-source';


export default class DataSourceService {
    static getDataSourceTypes(): AxiosPromise {
        return coreAxiosInstance.get(`${API.DataSource}/GetDataSourceTypes`);
    }
    static getDataSources(): AxiosPromise {
        return coreAxiosInstance.get(`${API.DataSource}/GetDataSources`);
    }
    static upsertSqlDatasource(
        sqlDataSourceDTO: SqlDataSource,
    ): AxiosPromise {
        return coreAxiosInstance.post(
            `${API.DataSource}/UpsertSqlDataSource`,
            sqlDataSourceDTO,
        );
    }
    static upsertExcelDatasource(
        data: ExcelDataSource,
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
    static checkSqlConnection(
        data: TestConnection
    ): AxiosPromise {
        return coreAxiosInstance.post(`${API.DataSource}/CheckSqlConnection`, data);
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