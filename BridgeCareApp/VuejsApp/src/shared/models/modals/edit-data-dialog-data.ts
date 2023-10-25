import { AdminSelectItem } from "../vue/admin-select-item";

export interface EditAdminDataDialogData{
    showDialog: boolean;
    settingName: string;
    settingsList: string[];
    selectedSettings: string[];
    selectedItem: string;
    AddedItems: AdminSelectItem[]
}

export const emptyEditAdminDataDialogData: EditAdminDataDialogData = {
    showDialog: false,
    selectedSettings: [],
    settingName: '',
    settingsList: [],
    selectedItem:'',
    AddedItems:[]
}