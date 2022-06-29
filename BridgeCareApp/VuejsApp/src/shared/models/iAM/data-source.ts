import { getNewGuid } from "@/shared/utils/uuid-utils";

export interface Datasource {
    id: string;
    name: string;
    connectionString: string;
    secure: boolean;
    type: string;
}

export interface DataSourceType {
    type: string;
}

export interface DataSourceExcelColumns {
    locationColumn: string,
    dateColumn: string
}
export interface RawDataColumns {
    columnHeaders: string[],
    warningMessage: string
}
export const emptyDatasource: Datasource = {
    id: getNewGuid(),
    name: '',
    connectionString: '',
    secure: false,
    type: ''
}
export const DSSQL: string = 'SQL';
export const DSEXCEL: string = 'Excel';