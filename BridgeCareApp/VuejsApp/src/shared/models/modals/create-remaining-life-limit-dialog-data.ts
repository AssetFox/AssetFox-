import {SelectItem} from '@/shared/models/vue/select-item';

export interface CreateRemainingLifeLimitDialogData {
    showDialog: boolean;
    numericAttributeSelectItems: SelectItem[];
}

export const emptyCreateRemainingLifeLimitDialogData: CreateRemainingLifeLimitDialogData = {
    showDialog: false,
    numericAttributeSelectItems: []
};
