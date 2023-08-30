import { emptyEquation, Equation } from '@/shared/models/iAM/equation';
import {
    CriterionLibrary,
    emptyCriterionLibrary,
} from '@/shared/models/iAM/criteria';
import { clone } from 'ramda';
import { getBlankGuid } from '@/shared/utils/uuid-utils';

export interface Report {
    id: string;
    name: string;
    criterionLibrary: CriterionLibrary;
}
export const emptyReport: Report = {
    id: getBlankGuid(),
    name: '',
    criterionLibrary: clone(emptyCriterionLibrary),
};