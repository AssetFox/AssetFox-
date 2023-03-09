import {emptyCalculatedAttributeLibrary, CalculatedAttributeLibrary} from '@/shared/models/iAM/calculated-attribute';
import {clone} from 'ramda';

export interface ShareCalculatedAttributeLibraryDialogData {
    showDialog: boolean;
    calculatedAttributeLibrary: CalculatedAttributeLibrary;
}

export const emptyShareCalculatedAttributeLibraryDialogData: ShareCalculatedAttributeLibraryDialogData = {
    showDialog: false,
    calculatedAttributeLibrary: clone(emptyCalculatedAttributeLibrary)
};

export interface CalculatedAttributeLibraryUserGridRow {
    id: string;
    username: string;
    isShared: boolean;
    canModify: boolean;
}