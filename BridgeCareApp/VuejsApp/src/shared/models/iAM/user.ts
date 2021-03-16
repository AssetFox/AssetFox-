import {CriterionLibrary, emptyCriterionLibrary} from '@/shared/models/iAM/criteria';
import {getBlankGuid} from '@/shared/utils/uuid-utils';
import {clone} from 'ramda';

export interface User {
    id: string;
    username: string;
    criterionLibrary: CriterionLibrary;
    hasInventoryAccess: boolean;
}

export const emptyUser: User = {
    id: getBlankGuid(),
    username: '',
    criterionLibrary: clone(emptyCriterionLibrary),
    hasInventoryAccess: false
};
