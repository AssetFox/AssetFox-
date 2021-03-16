import {emptyScenario, Scenario} from '@/shared/models/iAM/scenario';
import {clone} from 'ramda';

export interface ShareScenarioDialogData {
    showDialog: boolean;
    scenario: Scenario;
}

export const emptyShareScenarioDialogData: ShareScenarioDialogData = {
    showDialog: false,
    scenario: clone(emptyScenario)
};

export interface ScenarioUserGridRow {
    id: string;
    username: string;
    isShared: boolean;
    canModify: boolean;
}