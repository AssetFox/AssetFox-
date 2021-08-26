import {
    CriterionLibrary,
    emptyCriterionLibrary,
} from '@/shared/models/iAM/criteria';
import { getBlankGuid } from '@/shared/utils/uuid-utils';
import { clone } from 'ramda';
import { emptyEquation, Equation } from '@/shared/models/iAM/equation';

export interface TreatmentCost {
    id: string;
    equation: Equation;
    criterionLibrary: CriterionLibrary;
}

export interface TreatmentConsequence {
    id: string;
    attribute: string;
    changeValue: string;
    equation: Equation;
    criterionLibrary: CriterionLibrary;
}

export interface Treatment {
    id: string;
    name: string;
    description: string;
    shadowForAnyTreatment: number;
    shadowForSameTreatment: number;
    criterionLibrary: CriterionLibrary;
    costs: TreatmentCost[];
    consequences: TreatmentConsequence[];
    budgetIds: string[];
    addTreatment: boolean;
}

export interface TreatmentLibrary {
    id: string;
    name: string;
    description: string;
    treatments: Treatment[];
    owner?: string;
    shared?: boolean;
}

export interface TreatmentDetails {
    description: string;
    shadowForAnyTreatment: number;
    shadowForSameTreatment: number;
    criterionLibrary: CriterionLibrary;
}

export interface BudgetGridRow {
    budget: string;
}

export const emptyCost: TreatmentCost = {
    id: getBlankGuid(),
    equation: clone(emptyEquation),
    criterionLibrary: clone(emptyCriterionLibrary),
};

export const emptyConsequence: TreatmentConsequence = {
    id: getBlankGuid(),
    attribute: '',
    changeValue: '',
    equation: clone(emptyEquation),
    criterionLibrary: clone(emptyCriterionLibrary),
};

export const emptyTreatment: Treatment = {
    id: getBlankGuid(),
    name: '',
    description: '',
    shadowForAnyTreatment: 0,
    shadowForSameTreatment: 0,
    criterionLibrary: clone(emptyCriterionLibrary),
    costs: [],
    consequences: [],
    budgetIds: [],
    addTreatment: false,
};

export const emptyTreatmentLibrary: TreatmentLibrary = {
    id: getBlankGuid(),
    name: '',
    description: '',
    treatments: [],
};

export const emptyTreatmentDetails: TreatmentDetails = {
    description: '',
    shadowForAnyTreatment: 0,
    shadowForSameTreatment: 0,
    criterionLibrary: clone(emptyCriterionLibrary),
};
