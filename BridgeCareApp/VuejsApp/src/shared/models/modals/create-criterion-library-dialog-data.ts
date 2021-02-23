export interface CreateCriterionLibraryDialogData {
    showDialog: boolean;
    description: string;
    criteria: string;
}

export const emptyCreateCriterionLibraryDialogData: CreateCriterionLibraryDialogData = {
    showDialog: false,
    description: '',
    criteria: ''
};
