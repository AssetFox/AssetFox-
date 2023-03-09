import { emptyEquation, Equation } from '@/shared/models/iAM/equation';
import {
    CriterionLibrary,
    emptyCriterionLibrary,
} from '@/shared/models/iAM/criteria';
import { getBlankGuid } from '@/shared/utils/uuid-utils';
import { clone } from 'ramda';

export enum Timing {
    NotSpecified,
    OnDemand,
    PreDeterioration,
    PostDeterioration,
}
export const TimingMap: Record<string, Timing> = {
    'Pre Deterioration': Timing.PreDeterioration,
    'Post Deterioration': Timing.PostDeterioration,
    'On Demand': Timing.OnDemand,
};

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
    users: CalculatedAttributeLibraryUser[];
    calculatedAttributes: CalculatedAttribute[];
    isDefault: boolean;
    owner?: string;
}

export interface CriterionAndEquationSet {
    id: string;
    criteriaLibrary: CriterionLibrary;
    equation: Equation;
}

export interface CalculatedAttributeGridModel{
    id: string
    criteriaExpression: string;
    equation: string;
}

export const emptyCalculatedAttributeGridModel: CalculatedAttributeGridModel = {
    id: getBlankGuid(),
    criteriaExpression: "",
    equation: ""
}

export const emptyCalculatedAttribute: CalculatedAttribute = {
    id: getBlankGuid(),
    attribute: 'CONDITIONINDEX',
    name: 'CONDITIONINDEX',
    equations: [],
    calculationTiming: Timing.OnDemand,
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
    users: [],
    calculatedAttributes: [] as CalculatedAttribute[],
    isDefault: false,
};
export interface CalculatedAttributeLibraryUser {
    userId: string;
    username: string;
    canModify: boolean;
    isOwner: boolean;
}
export const emptyCalculatedAttributeLibraryUsers: CalculatedAttributeLibraryUser[] = [{
    userId: '',
    username: '',
    canModify: false,
    isOwner: false
}];