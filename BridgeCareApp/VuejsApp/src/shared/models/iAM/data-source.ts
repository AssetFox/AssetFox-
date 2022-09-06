import { getNewGuid } from "@/shared/utils/uuid-utils";

export interface Datasource {
    id: string;
    name: string;
    connectionString: string;
    locationColumn: string;
    dateColumn: string;
    secure: boolean;
    type: string;
}
export interface SqlDataSource {
    id: string;
    name: string;
    connectionString: string;
    secure: boolean;
    type: string
}
export interface ExcelDataSource {
    id: string;
    name: string;
    locationColumn: string;
    dateColumn: string;
    type: string;
    secure: boolean;
}
export interface DataSourceType {
    type: string;
}

export interface DataSourceExcelColumns {
    locationColumn: string[],
    dateColumn: string[]
}
export interface RawDataColumns {
    columnHeaders: string[],
    warningMessage: string
}
export interface SqlCommandResponse {
    isValid: boolean;
    validationMessage: string
}
export const emptySqlCommandResponse: SqlCommandResponse = {
    isValid: false,
    validationMessage: ''
}
export const emptyDatasource: Datasource = {
    id: getNewGuid(),
    name: '',
    connectionString: '',
    locationColumn: '',
    dateColumn: '',
    secure: false,
    type: ''
}
export const noneDatasource: Datasource = {
    id: getNewGuid(),
    name: 'None',
    connectionString: '',
    locationColumn: '',
    dateColumn: '',
    secure: false,
    type: ''
}
export const DSSQL: string = 'SQL';
export const DSEXCEL: string = 'Excel';