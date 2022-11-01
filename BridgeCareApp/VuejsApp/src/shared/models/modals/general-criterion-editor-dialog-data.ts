import { CriterionLibrary, emptyCriteria, emptyCriterionLibrary } from "../iAM/criteria";

export interface GeneralCriterionEditorDialogData{
    CriteriaExpression: string | null;
    showDialog: boolean;
}

export const emptyGeneralCriterionEditorDialogData: GeneralCriterionEditorDialogData = {
    CriteriaExpression: "",
    showDialog: false
}