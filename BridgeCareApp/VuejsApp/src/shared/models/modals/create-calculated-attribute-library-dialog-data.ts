import { CalculatedAttribute } from '../iAM/calculated-attribute';

export interface CreateCalculatedAttributeLibraryDialogData {
    showDialog: boolean;
    calculatedAttributes: CalculatedAttribute[];
}

export const emptyCreateCalculatedAttributeLibraryDialogData: CreateCalculatedAttributeLibraryDialogData = {
    showDialog: false,
    calculatedAttributes: [],
};
