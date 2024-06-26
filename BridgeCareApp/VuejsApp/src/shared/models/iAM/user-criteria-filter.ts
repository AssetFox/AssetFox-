import {getBlankGuid} from '@/shared/utils/uuid-utils';

export interface UserCriteriaFilter {
    criteriaId: string;
    userId: string;
    userName: string;
    description: string; 
    name: string;
    hasCriteria: boolean;
    hasAccess: boolean;
    activeStatus: boolean;
    criteria: string | null;
}

export const emptyUserCriteriaFilter: UserCriteriaFilter = {
    criteriaId: getBlankGuid(),
    userId: getBlankGuid(),
    criteria: '',
    name: '',
    userName: '',
    description: '',
    hasAccess: false,
    activeStatus: true,
    hasCriteria: true
};
