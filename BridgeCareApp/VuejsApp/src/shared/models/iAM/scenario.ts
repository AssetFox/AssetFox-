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

export interface QueuedScenario {
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
    queueEntryTimestamp: Date;
    workStartedTimestamp?: Date;
    queueingUser: string;
}

export interface ScenarioActions {
    title: string;
    action: string;
    icon: string;
}
export interface TabItems {
    name: string;
    icon: string;
    count: number;
}

export interface CloneScenarioData {
    scenarioId: string;
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