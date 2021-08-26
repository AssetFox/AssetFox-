import { Treatment } from '@/shared/models/iAM/treatment';

export interface CreateTreatmentLibraryDialogData {
    showDialog: boolean;
    selectedTreatmentLibraryTreatments: Treatment[];
}

export const emptyCreateTreatmentLibraryDialogData: CreateTreatmentLibraryDialogData = {
    showDialog: false,
    selectedTreatmentLibraryTreatments: [],
};
