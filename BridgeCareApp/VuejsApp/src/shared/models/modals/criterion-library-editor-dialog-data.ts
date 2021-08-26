import {getBlankGuid} from '@/shared/utils/uuid-utils';

export interface CriterionLibraryEditorDialogData {
    showDialog: boolean;
    libraryId: string;
    isCallFromScenario: boolean;
    isCriterionForLibrary: boolean; // This is TRUE, when criterion is edited for a Library
}

export const emptyCriterionLibraryEditorDialogData: CriterionLibraryEditorDialogData = {
    showDialog: false,
    libraryId: getBlankGuid(),
    isCallFromScenario: false,
    isCriterionForLibrary: false,
};
