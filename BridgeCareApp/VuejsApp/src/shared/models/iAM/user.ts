import {CriterionLibrary, emptyCriterionLibrary} from '@/shared/models/iAM/criteria';
import {getBlankGuid} from '@/shared/utils/uuid-utils';
import {clone} from 'ramda';

export interface User {
    id: string;
    username: string;
    description: string;
    criterionLibrary: CriterionLibrary;
    hasInventoryAccess: boolean;
    lastNewsAccessDate: string;
}

export interface UserNewsAccessDate {
    id: string;
    lastNewsAccessDate: string;
}

export const emptyUser: User = {
    id: getBlankGuid(),
    username: '',
    description: '',
    criterionLibrary: clone(emptyCriterionLibrary),
    hasInventoryAccess: false,
    lastNewsAccessDate: new Date().toISOString()
};
export interface LibraryUser {
    userId: string;
    userName: string;
    accessLevel: number;
}
