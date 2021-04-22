import {getBlankGuid} from '@/shared/utils/uuid-utils';

export interface ReportsDownloaderDialogData {
    showModal: boolean;
    scenarioId: string;
    networkId: string;
    name: string;
}

export const emptyReportsDownloadDialogData: ReportsDownloaderDialogData = {
    showModal: false,
    scenarioId: getBlankGuid(),
    networkId: getBlankGuid(),
    name: ''
};
