import {emptyPerformanceCurveLibrary, PerformanceCurveLibrary} from '@/shared/models/iAM/performance';
import {clone} from 'ramda';

export interface SharePerformanceCurveLibraryDialogData {
    showDialog: boolean;
    performanceCurveLibrary: PerformanceCurveLibrary;
}

export const emptySharePerformanceCurveLibraryDialogData: SharePerformanceCurveLibraryDialogData = {
    showDialog: false,
    performanceCurveLibrary: clone(emptyPerformanceCurveLibrary)
};

export interface PerformanceCurveLibraryUserGridRow {
    id: string;
    username: string;
    isShared: boolean;
    canModify: boolean;
}