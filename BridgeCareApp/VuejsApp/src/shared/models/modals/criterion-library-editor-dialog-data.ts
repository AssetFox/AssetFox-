import {getBlankGuid} from '@/shared/utils/uuid-utils';

export interface CriterionLibraryEditorDialogData {
    showDialog: boolean;
    libraryId: string;
    isCallFromScenario: boolean;
}

export const emptyCriterionLibraryEditorDialogData: CriterionLibraryEditorDialogData = {
    showDialog: false,
    libraryId: getBlankGuid(),
    isCallFromScenario: false
};
