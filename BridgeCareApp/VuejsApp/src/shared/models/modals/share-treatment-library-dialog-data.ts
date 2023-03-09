import {emptyTreatmentLibrary, TreatmentLibrary} from '@/shared/models/iAM/treatment';
import {clone} from 'ramda';

export interface ShareTreatmentLibraryDialogData {
    showDialog: boolean;
    treatmentLibrary: TreatmentLibrary;
}

export const emptyShareTreatmentLibraryDialogData: ShareTreatmentLibraryDialogData = {
    showDialog: false,
    treatmentLibrary: clone(emptyTreatmentLibrary)
};

export interface TreatmentLibraryUserGridRow {
    id: string;
    username: string;
    isShared: boolean;
    canModify: boolean;
}