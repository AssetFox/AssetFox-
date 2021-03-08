import {Treatment} from '@/shared/models/iAM/treatment';
import {getBlankGuid} from '@/shared/utils/uuid-utils';

export interface CreateTreatmentLibraryDialogData {
    showDialog: boolean;
    selectedTreatmentLibraryTreatments: Treatment[];
    scenarioId: string;
}

export const emptyCreateTreatmentLibraryDialogData: CreateTreatmentLibraryDialogData = {
    showDialog: false,
    selectedTreatmentLibraryTreatments: [],
    scenarioId: getBlankGuid()
};
