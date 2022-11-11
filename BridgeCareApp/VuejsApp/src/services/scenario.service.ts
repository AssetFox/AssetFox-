import {AxiosPromise} from 'axios';
import {CloneScenarioData, Scenario, QueuedSimulation} from '@/shared/models/iAM/scenario';
import {API, coreAxiosInstance} from '@/shared/utils/axios-instance';
import { PagingRequest } from '@/shared/models/iAM/paging';

export default class ScenarioService {
    static getScenarios(): AxiosPromise {
        return coreAxiosInstance.get(`${API.Scenario}/GetScenarios/`);
    }

    static getUserScenariosPage(data:PagingRequest<Scenario>): AxiosPromise {
        return coreAxiosInstance.post(`${API.Scenario}/GetUserScenariosPage/`, data);
    }

    static getSharedScenariosPage(data:PagingRequest<Scenario>): AxiosPromise {
        return coreAxiosInstance.post(`${API.Scenario}/GetSharedScenariosPage/`, data);
    }

    static getSimulationQueuePage(data:PagingRequest<QueuedSimulation>): AxiosPromise {
        return coreAxiosInstance.post(`${API.Scenario}/GetSimulationQueuePage/`, data);
    }    

    static createScenario(data: Scenario, networkId: string): AxiosPromise {
        return coreAxiosInstance.post(`${API.Scenario}/CreateScenario/${networkId}`, data);
    }

    static updateScenario(data: Scenario): AxiosPromise {
        return coreAxiosInstance.put(`${API.Scenario}/UpdateScenario`, data);
    }

    static cloneScenario(data: CloneScenarioData): AxiosPromise {
        return coreAxiosInstance.post(`${API.Scenario}/CloneScenario/`, data);
    }

    static deleteScenario(scenarioId: number): AxiosPromise {
        return coreAxiosInstance.delete(`${API.Scenario}/DeleteScenario/${scenarioId}`);
    }

    static runSimulation(networkId: string, scenarioId: string | undefined): AxiosPromise {
        return coreAxiosInstance.post(`${API.Scenario}/RunSimulation/${networkId}/${scenarioId}`);
    }

    static cancelSimulation(simulationId: string): AxiosPromise {
        return coreAxiosInstance.delete(`${API.Scenario}/CancelSimulation/${simulationId}`);
    }    

    static migrateLegacySimulationData(simulationId: number): AxiosPromise {
        return coreAxiosInstance.post(`/api/LegacySimulationSynchronization/SynchronizeLegacySimulation/${simulationId}`);
    }
}
