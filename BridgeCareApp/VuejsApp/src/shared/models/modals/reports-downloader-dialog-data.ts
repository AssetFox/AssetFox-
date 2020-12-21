import {emptyScenario, Scenario} from '@/shared/models/iAM/scenario';

export interface ReportsDownloaderDialogData {
    showModal: boolean;
    scenario: Scenario;
    newNetworkId: string;
}

export const emptyReportsDownloadDialogData: ReportsDownloaderDialogData = {
    showModal: false,
    scenario: emptyScenario,
    newNetworkId: ''
};
