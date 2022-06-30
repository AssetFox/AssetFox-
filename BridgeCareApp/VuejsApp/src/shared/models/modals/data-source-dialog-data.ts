import { Datasource, emptyDatasource } from "../iAM/data-source";

export interface CreateDataSourceDialogData {
    showDialog: boolean;
}

export const emptyCreateDataSourceDialogData: CreateDataSourceDialogData = {
    showDialog: false,
};
