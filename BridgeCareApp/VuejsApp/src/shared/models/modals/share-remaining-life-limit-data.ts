import {emptyRemainingLifeLimitLibrary, RemainingLifeLimitLibrary} from '@/shared/models/iAM/remaining-life-limit';
import {clone} from 'ramda';

export interface ShareRemainingLifeLimitLibraryDialogData {
    showDialog: boolean;
    remainingLifeLimitLibrary: RemainingLifeLimitLibrary;
}

export const emptyShareRemainingLifeLimitLibraryDialogData: ShareRemainingLifeLimitLibraryDialogData = {
    showDialog: false,
    remainingLifeLimitLibrary: clone(emptyRemainingLifeLimitLibrary)
};

export interface RemainingLifeLimitLibraryUserGridRow {
    id: string;
    username: string;
    isShared: boolean;
    canModify: boolean;
}