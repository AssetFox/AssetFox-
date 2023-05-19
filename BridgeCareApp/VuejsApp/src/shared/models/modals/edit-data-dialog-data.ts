export interface EditDataDialogData{
    showDialog: boolean;
    settingName: string;
    settingsList: string[];
    selectedSettings: string[];
}

export const emptyEditDataDialogData: EditDataDialogData = {
    showDialog: false,
    selectedSettings: [],
    settingName: '',
    settingsList: []
}