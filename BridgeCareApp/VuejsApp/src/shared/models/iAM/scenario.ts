import {getBlankGuid} from '@/shared/utils/uuid-utils';
export interface ScenarioUser {
    userId: string;
    username: string;
    canModify: boolean;
    isOwner: boolean;
}

export interface Scenario {
    id: string;
    name: string;
    networkId: string;
    networkName: string;
    users: ScenarioUser[];
    owner?: string;
    creator?: string;
    createdDate?: Date;
    lastModifiedDate?: Date;
    lastRun?: Date;
    status?: string;
    reportStatus?: string;
    runTime?: string;
}

export interface CloneScenario {
    id: string;
    name: string;
    destinationNetworkId: string;
    networkId: string;
    networkName: string;
    scenarioId: string;
    users: ScenarioUser[];
    owner?: string;
    creator?: string;
    createdDate?: Date;
    lastModifiedDate?: Date;
    lastRun?: Date;
    status?: string;
    reportStatus?: string;
    runTime?: string;
}

export interface QueuedWork {
    id: string;
    name: string;
    status?: string;
    queueEntryTimestamp: Date;
    workStartedTimestamp?: Date;
    queueingUser: string;
    currentRunTime?: string;
    previousRunTime?: string;
    queuePosition: number;
    workDescription: string;
    workType: WorkType;
    domainType: DomainType;
    domainId: string;
}

export interface WorkQueueRequest {
    domainId: string;
    workType: WorkType;
}

export enum WorkType {
    SimulationAnalysis,
    DeleteNetwork,
    Aggregation,
    SimulationOutputConversion,
    DeleteSimulation,
    ReportGeneration,
    ImportLibraryInvestment,
    ImportLibraryPerformanceCurve,
    ImportLibraryTreatment,
    ImportScenarioInvestment,
    ImportScenarioPerformanceCurve,
    ImportScenarioTreatment,
    ImportScenarioTreatmentSupersedeRule,
    ImportLibraryTreatmentSupersedeRule,
    ImportCommittedProject,
}

export enum DomainType {
    Simulation,
    Network,
    Investment,
    PerformanceCurve,
    Treatment,
    CommittedProject
}

export interface ScenarioActions {
    title: string;
    action: string;
    icon: string;
    isCustomIcon: boolean;
}
export interface TabItems {
    name: string;
    icon: string;
    count: number;
}

export interface CloneScenarioData {
    scenarioId: string;
    destinationNetworkId: string;
    networkId: string;
    scenarioName: string;
}

export const emptyScenario: Scenario = {
    id: getBlankGuid(),
    name: '',
    networkId: getBlankGuid(),
    networkName: '',
    users: [],
    createdDate: new Date(),
    lastModifiedDate: new Date(),
};

export const emptyCloneScenario: CloneScenario = {
    id: getBlankGuid(),
    name: '',
    destinationNetworkId: getBlankGuid(),
    scenarioId: getBlankGuid(),
    networkId: getBlankGuid(),
    networkName: '',
    users: [],
    createdDate: new Date(),
    lastModifiedDate: new Date(),
};

export const emptyQueuedWork: QueuedWork = {
    id: '',
    name: '',
    queueEntryTimestamp: new Date(),
    workStartedTimestamp: new Date(),
    queueingUser: '',
    queuePosition: 0,
    workDescription: '',
    workType: WorkType.SimulationAnalysis,
    domainType: DomainType.Simulation,
    domainId: getBlankGuid()
};