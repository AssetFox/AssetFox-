import { clone } from 'ramda';
import { CalculatedAttribute, emptyCalculatedAttribute } from '../iAM/calculated-attribute';
import { SelectItem } from '../vue/select-item';

export interface CreateCalculatedAttributeLibraryDialogData {
    showDialog: boolean;
    calculatedAttributes: CalculatedAttribute[];
    attributeSelectItems: SelectItem[];
}

export const emptyCreateCalculatedAttributeLibraryDialogData: CreateCalculatedAttributeLibraryDialogData = {
    showDialog: false,
    calculatedAttributes: [] as CalculatedAttribute[],
    attributeSelectItems: [] as SelectItem[],
};
