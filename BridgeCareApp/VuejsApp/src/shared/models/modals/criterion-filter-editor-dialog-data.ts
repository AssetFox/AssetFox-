import {getBlankGuid} from '@/shared/utils/uuid-utils';

export interface CriterionFilterEditorDialogData {
    showDialog: boolean;
    userId: string;
    name: string;
    description: string;
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
    name: '',
    description: '',
    userName: '',
    hasCriteria: false,
    hasAccess: false,
};
