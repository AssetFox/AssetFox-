import {getBlankGuid} from '@/shared/utils/uuid-utils';

export interface CriterionFilterEditorDialogData {
    showDialog: boolean;
    userId: string;
    criteriaId: string;
    criteria: string | null;
    userName: string;
    hasCriteria: boolean;
    hasAccess: boolean;
}

export const emptyCriterionFilterEditorDialogData: CriterionFilterEditorDialogData = {
    showDialog: false,
    userId: getBlankGuid(),
    criteriaId: getBlankGuid(),
    criteria: '',
    userName: '',
    hasCriteria: false,
    hasAccess: false,
};
