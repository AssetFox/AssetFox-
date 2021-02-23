import {PerformanceCurve} from '@/shared/models/iAM/performance';

export interface CreatePerformanceCurveLibraryDialogData {
    showDialog: boolean;
    description: string;
    performanceCurves: PerformanceCurve[];
}

export const emptyCreatePerformanceLibraryDialogData: CreatePerformanceCurveLibraryDialogData = {
    showDialog: false,
    description: '',
    performanceCurves: []
};
