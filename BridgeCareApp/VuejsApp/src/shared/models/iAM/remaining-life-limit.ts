import {getBlankGuid} from '@/shared/utils/uuid-utils';
import {CriterionLibrary, emptyCriterionLibrary} from '@/shared/models/iAM/criteria';
import {clone} from 'ramda';

export interface RemainingLifeLimit {
    id: string;
    attribute: string;
    value: number;
    criterionLibrary: CriterionLibrary;
}

export const emptyRemainingLifeLimit: RemainingLifeLimit = {
    id: getBlankGuid(),
    attribute: '',
    value: 0,
    criterionLibrary: clone(emptyCriterionLibrary)
};

export interface RemainingLifeLimitLibrary {
    id: string;
    name: string;
    description: string;
    remainingLifeLimits: RemainingLifeLimit[];
    appliedScenarioIds: string[];
    owner?: string;
    users: RemainingLifeLimitLibraryUser[];
    shared?: boolean;
}

export const emptyRemainingLifeLimitLibrary: RemainingLifeLimitLibrary = {
    id: getBlankGuid(),
    name: '',
    description: '',
    remainingLifeLimits: [],
    appliedScenarioIds: [],
    users: []
};

export interface RemainingLifeLimitLibraryUser {
    userId: string;
    username: string;
    canModify: boolean;
    isOwner: boolean;
}
export const emptyRemainingLifeLimitLibraryUsers: RemainingLifeLimitLibraryUser[] = [{
    userId: '',
    username: '',
    canModify: false,
    isOwner: false
}];
