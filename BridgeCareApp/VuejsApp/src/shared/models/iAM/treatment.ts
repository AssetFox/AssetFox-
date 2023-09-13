import {
    CriterionLibrary,
    emptyCriterionLibrary,
} from '@/shared/models/iAM/criteria';
import { getBlankGuid } from '@/shared/utils/uuid-utils';
import { clone } from 'ramda';
import { emptyEquation, Equation } from '@/shared/models/iAM/equation';

// order is important
export enum TreatmentCategory {
    preservation,
    capacityAdding,
    rehabilitation,
    replacement,
    maintenance,
    other,
}
export enum AssetType {
    bridge,
    culvert,
}

export interface TreatmentAttributeFactor {
    attribute: string;
    factor: number;
}
export interface TreatmentCost {
    id: string;
    equation: Equation;
    criterionLibrary: CriterionLibrary;
}

export interface TreatmentPerformanceFactor {
    id: string;
    attribute: string;
    performanceFactor: number;
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
    performanceFactors: TreatmentPerformanceFactor[];
    budgetIds: string[];
    addTreatment: boolean;
    category: TreatmentCategory;
    assetType: AssetType;
    isModified: boolean;
    libraryId: string;
    isUnselectable: boolean;
}

export interface SimpleTreatment {
    id: string;
    name: string;
}

export interface TreatmentLibraryUser {
    userId: string;
    username: string;
    canModify: boolean;
    isOwner: boolean;
}
export interface TreatmentLibrary {
    id: string;
    name: string;
    description: string;
    treatments: Treatment[];
    owner?: string;
    isShared: boolean;
    isModified: boolean;
    users: TreatmentLibraryUser[];
}

export interface TreatmentDetails {
    description: string;
    shadowForAnyTreatment: number;
    shadowForSameTreatment: number;
    criterionLibrary: CriterionLibrary;
    category: TreatmentCategory;
    assetType: AssetType;
    isUnselectable: boolean;
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
    category: TreatmentCategory.preservation,
    assetType: AssetType.bridge,
    performanceFactors: [],
    isModified: false,
    libraryId:  getBlankGuid(),
    isUnselectable: false,
};

export const emptyTreatmentLibrary: TreatmentLibrary = {
    id: getBlankGuid(),
    name: '',
    description: '',
    treatments: [],
    isShared: false,
    isModified: false,
    users: []
};

export const emptyTreatmentLibraryUser: TreatmentLibraryUser = {
    userId: '',
    username: '',
    canModify: false,
    isOwner: false
}

export const emptyTreatmentDetails: TreatmentDetails = {
    description: '',
    shadowForAnyTreatment: 0,
    shadowForSameTreatment: 0,
    criterionLibrary: clone(emptyCriterionLibrary),
    category: TreatmentCategory.preservation,
    assetType: AssetType.bridge,
    isUnselectable: false,
};

export const assetTypeMap: Map<string, AssetType> = new Map([
    ['Bridge', 0],
    ['Culvert', 1],
]);
export const assetTypeReverseMap: Map<AssetType, string> = new Map([
    [0, 'Bridge'],
    [1, 'Culvert'],
]);

// the maps are to convert data to be used in the UI to the data needed by Vue and backend
export const treatmentCategoryMap: Map<string, TreatmentCategory> = new Map([
    ['Preservation', 0],
    ['Capacity Adding', 1],
    ['Rehabilitation', 2],
    ['Replacement', 3],
    ['Maintenance', 4],
    ['Other', 5],
]);
export const treatmentCategoryReverseMap: Map<TreatmentCategory, string> = new Map([
    [0, 'Preservation'],
    [1, 'Capacity Adding'],
    [2, 'Rehabilitation'],
    [3, 'Replacement'],
    [4, 'Maintenance'],
    [5, 'Other'],
]);

export interface TreatmentsFileImport {
    file: File;
}
