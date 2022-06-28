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