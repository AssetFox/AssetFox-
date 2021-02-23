import {emptyEquation, Equation} from '@/shared/models/iAM/equation';
import {clone} from 'ramda';

export interface EquationEditorDialogData {
    showDialog: boolean;
    equation: Equation;
}

export const emptyEquationEditorDialogData: EquationEditorDialogData = {
    showDialog: false,
    equation: clone(emptyEquation)
};
