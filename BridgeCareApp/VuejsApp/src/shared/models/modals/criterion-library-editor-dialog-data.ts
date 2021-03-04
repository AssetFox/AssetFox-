import {getBlankGuid} from '@/shared/utils/uuid-utils';

export interface CriterionLibraryEditorDialogData {
    showDialog: boolean;
    libraryId: string;
}

export const emptyCriterionLibraryEditorDialogData: CriterionLibraryEditorDialogData = {
    showDialog: false,
    libraryId: getBlankGuid()
};
