import { clone } from 'ramda';
import { CalculatedAttribute, emptyCalculatedAttribute } from '../iAM/calculated-attribute';

export interface CreateCalculatedAttributeLibraryDialogData {
    showDialog: boolean;
    calculatedAttribute: CalculatedAttribute;
}

export const emptyCreateCalculatedAttributeLibraryDialogData: CreateCalculatedAttributeLibraryDialogData = {
    showDialog: false,
    calculatedAttribute: clone(emptyCalculatedAttribute),
};
