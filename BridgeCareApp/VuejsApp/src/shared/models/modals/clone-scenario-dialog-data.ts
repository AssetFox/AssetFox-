import { emptyScenario, Scenario } from '../iAM/scenario';

export interface CloneScenarioDialogData {
    showDialog: boolean;
    scenario: Scenario;
}

export const emptyCloneScenarioDialogData: CloneScenarioDialogData = {
    showDialog: false,
    scenario: emptyScenario
};
