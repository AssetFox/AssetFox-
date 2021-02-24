import {PerformanceCurve} from '@/shared/models/iAM/performance';
import {getBlankGuid} from '@/shared/utils/uuid-utils';

export interface CreatePerformanceCurveLibraryDialogData {
    showDialog: boolean;
    performanceCurves: PerformanceCurve[];
    scenarioId: string;
}

export const emptyCreatePerformanceLibraryDialogData: CreatePerformanceCurveLibraryDialogData = {
    showDialog: false,
    performanceCurves: [],
    scenarioId: getBlankGuid()
};
