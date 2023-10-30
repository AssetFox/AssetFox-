import { emptyScenario, Scenario,CloneScenario, emptyCloneScenario } from '../iAM/scenario';

export interface CloneScenarioDialogData {
    showDialog: boolean;
    scenario: Scenario;
}
export interface CloneSimulationDialogData {
    showDialog: boolean;
    scenario: CloneScenario;
}

export const emptyCloneSimulationDialogData: CloneSimulationDialogData = {
    showDialog: false,
    scenario: emptyCloneScenario
}
export const emptyCloneScenarioDialogData: CloneScenarioDialogData = {
    showDialog: false,
    scenario: emptyScenario
};
