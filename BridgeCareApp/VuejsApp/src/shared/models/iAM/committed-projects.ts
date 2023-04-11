import { getBlankGuid } from "@/shared/utils/uuid-utils";
import { TreatmentCategory } from "./treatment";

export interface BaseCommittedProject {
    id: string;
    simulationId: string;
    scenarioBudgetId: string | null;
    year: number;
    treatment: string;
    cost: number;
    shadowForAnyTreatment: number;
    shadowForSameTreatment: number;
    consequences: CommittedProjectConsequence[];
    locationKeys: { [key: string]: string; }; 
    category: TreatmentCategory
}
export interface SectionCommittedProjectTableData {
    id: string;
    brkey: string;
    year: number;
    factor: number;
    treatment: string;
    treatmentId: string;
    scenarioBudgetId: string;
    budget: string;
    cost: number;
    errors: string[];
    yearErrors: string[];
    category: string;
}
export interface SectionCommittedProject extends BaseCommittedProject{
    name: string;
    performanceFactor: number;
}

export interface CommittedProjectConsequence {
    id: string;
    committedProjectId: string;
    attribute: string;
    changeValue: string;
}


export const emptySectionCommittedProject = {
    id: getBlankGuid(),
    simulationId: getBlankGuid(),
    scenarioBudgetId: null,
    year: 0,
    treatment: '',
    cost: 0,
    performanceFactor: 1,
    shadowForAnyTreatment: 0,
    shadowForSameTreatment: 0,
    consequences: [],
    locationKeys: {},
    name: '',
    category: TreatmentCategory.other
}

export const emptyCommittedProjectConsequence ={
    id: getBlankGuid(),
    committedProjectId: getBlankGuid(),
    attribute: '',
    changeValue: ''
}

export interface CommittedProjectFillTreatmentReturnValues {
    validTreatmentConsequences: CommittedProjectConsequence[];
    treatmentCost: number;
    treatmentCategory: TreatmentCategory;
}

export interface CommittedProjectFillTreatmentValues {
    committedProjectId: string;
    treatmentLibraryId: string;
    treatmentName: string;
    brkey_Value: string; 
    networkId: string;
}

