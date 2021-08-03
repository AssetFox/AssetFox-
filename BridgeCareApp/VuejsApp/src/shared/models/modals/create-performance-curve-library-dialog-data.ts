import { PerformanceCurve } from '@/shared/models/iAM/performance';
import { getBlankGuid } from '@/shared/utils/uuid-utils';

export interface CreatePerformanceCurveLibraryDialogData {
    showDialog: boolean;
    performanceCurves: PerformanceCurve[];
}

export const emptyCreatePerformanceLibraryDialogData: CreatePerformanceCurveLibraryDialogData = {
    showDialog: false,
    performanceCurves: [],
};
