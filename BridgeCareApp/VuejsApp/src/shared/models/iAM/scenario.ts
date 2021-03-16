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

export const emptyScenario: Scenario = {
    id: getBlankGuid(),
    name: '',
    users: [],
    createdDate: new Date(),
    lastModifiedDate: new Date(),
};
