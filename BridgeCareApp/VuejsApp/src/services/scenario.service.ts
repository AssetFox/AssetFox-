import {AxiosPromise} from 'axios';
import {Scenario} from '@/shared/models/iAM/scenario';
import {API, coreAxiosInstance} from '@/shared/utils/axios-instance';

export default class ScenarioService {
    static getScenarios(): AxiosPromise {
        return coreAxiosInstance.get(`${API.Scenario}/GetScenarios/`);
    }

    static createScenario(data: Scenario, networkId: string): AxiosPromise {
        return coreAxiosInstance.post(`${API.Scenario}/CreateScenario/${networkId}`, data);
    }

    static updateScenario(data: Scenario): AxiosPromise {
        return coreAxiosInstance.put(`${API.Scenario}/UpdateScenario`, data);
    }

    static cloneScenario(scenarioId: number): AxiosPromise {
        return coreAxiosInstance.post(`${API.Scenario}/CloneScenario/${scenarioId}`);
    }

    static deleteScenario(scenarioId: number): AxiosPromise {
        return coreAxiosInstance.delete(`${API.Scenario}/DeleteScenario/${scenarioId}`);
    }

    static runSimulation(networkId: string, scenarioId: string | undefined): AxiosPromise {
        return coreAxiosInstance.post(`${API.Scenario}/RunSimulation/${networkId}/${scenarioId}`);
    }

    static migrateLegacySimulationData(simulationId: number): AxiosPromise {
        return coreAxiosInstance.post(`/api/LegacySimulationSynchronization/SynchronizeLegacySimulation/${simulationId}`);
    }
}
