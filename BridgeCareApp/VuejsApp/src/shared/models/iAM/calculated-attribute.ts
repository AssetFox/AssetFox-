import { emptyEquation, Equation } from '@/shared/models/iAM/equation';
import {
    CriterionLibrary,
    emptyCriterionLibrary,
} from '@/shared/models/iAM/criteria';
import { getBlankGuid } from '@/shared/utils/uuid-utils';
import { clone } from 'ramda';

export enum Timing {
    PreSimulation = 0,
    PostSimulation,
    OnDemand,
  }

export interface CalculatedAttribute {
    id: string;
    attribute: string;
    name: string;
    timing: Timing;
    criterionAndEquationSet: CriterionAndEquationSet[];
}

export interface CalculatedAttributeLibrary {
    id: string;
    name: string;
    description: string;
    calculatedAttribute: CalculatedAttribute;
    owner?: string;
    shared?: boolean;
    defaultCalculation: boolean;
}
export interface CriterionAndEquationSet {
    id: string;
    criterionLibrary: CriterionLibrary;
    equation: Equation;
}

export const emptyCalculatedAttribute: CalculatedAttribute = {
    id: getBlankGuid(),
    attribute: '',
    name: '',
    criterionAndEquationSet: [],
    timing: Timing.OnDemand
};

export const emptyCriterionAndEquationSet: CriterionAndEquationSet = {
    id: getBlankGuid(),
    equation: clone(emptyEquation),
    criterionLibrary: clone(emptyCriterionLibrary),
};

export const emptyCalculatedAttributeLibrary: CalculatedAttributeLibrary = {
    id: getBlankGuid(),
    name: '',
    description: '',
    calculatedAttribute: clone(emptyCalculatedAttribute),
    defaultCalculation: false,
};
