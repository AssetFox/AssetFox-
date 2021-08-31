import { emptyEquation, Equation } from '@/shared/models/iAM/equation';
import {
    CriterionLibrary,
    emptyCriterionLibrary,
} from '@/shared/models/iAM/criteria';
import { getBlankGuid } from '@/shared/utils/uuid-utils';
import { clone } from 'ramda';

export interface CalculatedAttribute {
    id: string;
    attribute: string;
    name: string;
    shift: boolean;
    criterionLibrary: CriterionLibrary;
    equation: Equation;
}

export interface CalculatedAttributeLibrary {
    id: string;
    name: string;
    description: string;
    calculatedAttributes: CalculatedAttribute[];
    owner?: string;
    shared?: boolean;
}
export const emptyCalculatedAttribute: CalculatedAttribute = {
    id: getBlankGuid(),
    attribute: '',
    name: '',
    shift: false,
    equation: clone(emptyEquation),
    criterionLibrary: clone(emptyCriterionLibrary),
};

export const emptyCalculatedAttributeLibrary: CalculatedAttributeLibrary = {
    id: getBlankGuid(),
    name: '',
    description: '',
    calculatedAttributes: [],
};