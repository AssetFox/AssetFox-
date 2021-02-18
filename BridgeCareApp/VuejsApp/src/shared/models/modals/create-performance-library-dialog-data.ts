import {PerformanceCurve} from '@/shared/models/iAM/performance';

export interface CreatePerformanceLibraryDialogData {
    showDialog: boolean;
    description: string;
    equations: PerformanceCurve[];
}

export const emptyCreatePerformanceLibraryDialogData: CreatePerformanceLibraryDialogData = {
    showDialog: false,
    description: '',
    equations: []
};
