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
  export const TimingMap: Record<string,  Timing> = {
      'Pre Simulation': Timing.PreSimulation,
      'Post Simulation': Timing.PostSimulation,
      'On Demand': Timing.OnDemand
  }

export interface CalculatedAttribute {
    id: string;
    attribute: string;
    name: string;
    calculationTiming: Timing;
    equations: CriterionAndEquationSet[];
}

export interface CalculatedAttributeLibrary {
    id: string;
    name: string;
    description: string;
    calculatedAttributes: CalculatedAttribute[];
    isDefault: boolean;
}
export interface CriterionAndEquationSet {
    id: string;
    criteriaLibrary: CriterionLibrary;
    equation: Equation;
}

export const emptyCalculatedAttribute: CalculatedAttribute = {
    id: getBlankGuid(),
    attribute: 'CONDITIONINDEX',
    name: 'CONDITIONINDEX',
    equations: [],
    calculationTiming: Timing.OnDemand
};

export const emptyCriterionAndEquationSet: CriterionAndEquationSet = {
    id: getBlankGuid(),
    equation: clone(emptyEquation),
    criteriaLibrary: clone(emptyCriterionLibrary),
};

export const emptyCalculatedAttributeLibrary: CalculatedAttributeLibrary = {
    id: getBlankGuid(),
    name: '',
    description: '',
    calculatedAttributes: [] as CalculatedAttribute[],
    isDefault: false,
};