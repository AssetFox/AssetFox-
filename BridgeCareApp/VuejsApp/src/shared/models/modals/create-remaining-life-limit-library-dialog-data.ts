import { RemainingLifeLimit } from '@/shared/models/iAM/remaining-life-limit';
import { getBlankGuid } from '@/shared/utils/uuid-utils';

export interface CreateRemainingLifeLimitLibraryDialogData {
    showDialog: boolean;
    remainingLifeLimits: RemainingLifeLimit[];
}

export const emptyCreateRemainingLifeLimitLibraryDialogData: CreateRemainingLifeLimitLibraryDialogData = {
    showDialog: false,
    remainingLifeLimits: [],
};
