import { getBlankGuid } from "@/shared/utils/uuid-utils";

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
    locationKeys: { [key: string]: string; }  
}
export interface SectionCommittedProjectTableData {
    id: string;
    brkey: string;
    year: number;
    treatment: string;
    budget: string;
    cost: number;
}
export interface SectionCommittedProject extends BaseCommittedProject{
    name: string;
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
    shadowForAnyTreatment: 0,
    shadowForSameTreatment: 0,
    consequences: [],
    locationKeys: {},
    name: '',

}

export const emptyCommittedProjectConsequence ={
    id: getBlankGuid(),
    committedProjectId: getBlankGuid(),
    attribute: '',
    changeValue: ''
}