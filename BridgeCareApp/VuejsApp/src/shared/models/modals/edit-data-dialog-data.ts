export interface EditAdminDataDialogData{
    showDialog: boolean;
    settingName: string;
    settingsList: string[];
    selectedSettings: string[];
}

export const emptyEditAdminDataDialogData: EditAdminDataDialogData = {
    showDialog: false,
    selectedSettings: [],
    settingName: '',
    settingsList: []
}