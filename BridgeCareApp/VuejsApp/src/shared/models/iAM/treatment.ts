import {CriterionLibrary, emptyCriterionLibrary} from '@/shared/models/iAM/criteria';
import {getBlankGuid} from '@/shared/utils/uuid-utils';
import {clone} from 'ramda';
import {emptyEquation, Equation} from '@/shared/models/iAM/equation';
import {SimpleBudgetDetail} from '@/shared/models/iAM/investment';

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
    isNew: boolean;
}

export interface TreatmentLibrary {
    id: string;
    name: string;
    description: string;
    treatments: Treatment[];
    appliedScenarioIds: string[];
    owner?: string;
    shared?: boolean;
}

export interface TreatmentDetails {
    description: string;
    shadowForAnyTreatment: number;
    shadowForSameTreatment: number;
    criterionLibrary: CriterionLibrary;
    isCallFromScenario: boolean;
}

export interface BudgetGridRow {
    budget: string;
}

export const emptyCost: TreatmentCost = {
    id: getBlankGuid(),
    equation: clone(emptyEquation),
    criterionLibrary: clone(emptyCriterionLibrary)
};

export const emptyConsequence: TreatmentConsequence = {
    id: getBlankGuid(),
    attribute: '',
    changeValue: '',
    equation: clone(emptyEquation),
    criterionLibrary: clone(emptyCriterionLibrary)
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
    isNew: false
};

export const emptyTreatmentLibrary: TreatmentLibrary = {
    id: getBlankGuid(),
    name: '',
    description: '',
    treatments: [],
    appliedScenarioIds: []
};

export const emptyTreatmentDetails: TreatmentDetails = {
    description: '',
    shadowForAnyTreatment: 0,
    shadowForSameTreatment: 0,
    criterionLibrary: clone(emptyCriterionLibrary),
    isCallFromScenario: false
};