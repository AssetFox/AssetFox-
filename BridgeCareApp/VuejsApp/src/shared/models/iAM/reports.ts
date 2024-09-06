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
    mergedExpression: string;
    isGenerated: boolean;
}
export const emptyReport: Report = {
    id: getBlankGuid(),
    name: '',
    mergedExpression: '',
    isGenerated: false
};

export interface ReportDetails {
    simulationId: string;
    reportName: string;
    isGenerated: boolean;
}

export const reportDetails: ReportDetails = {
    simulationId: getBlankGuid(),
    reportName: '',
    isGenerated: false
};