import {AxiosPromise} from 'axios';
import {CloneScenarioData, Scenario, QueuedWork, WorkQueueRequest} from '@/shared/models/iAM/scenario';
import {API, coreAxiosInstance} from '@/shared/utils/axios-instance';
import { PagingRequest } from '@/shared/models/iAM/paging';
import { BlobOptions } from 'buffer';

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

    static getCurrentUserOrSharedScenario(simulationId: string): AxiosPromise {
        return coreAxiosInstance.post(`${API.Scenario}/GetCurrentUserOrSharedScenario/${simulationId}`);
    }

    static getWorkQueuePage(data:PagingRequest<QueuedWork>): AxiosPromise {
        return coreAxiosInstance.post(`${API.Scenario}/GetWorkQueuePage/`, data);
    }    

    static getFastWorkQueuePage(data:PagingRequest<QueuedWork>): AxiosPromise {
        return coreAxiosInstance.post(`${API.Scenario}/GetFastWorkQueuePage/`, data);
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

    static cancelWorkQueueItem(simulationId: string): AxiosPromise {
        return coreAxiosInstance.delete(`${API.Scenario}/CancelWorkQueueItem/${simulationId}`);
    }
    
    static cancelFastQueueItem(simulationId: string): AxiosPromise {
        return coreAxiosInstance.delete(`${API.Scenario}/CancelFastQueueItem/${simulationId}`);
    }

    static getQueuedWorkByDomainIdAndWorkType(data:WorkQueueRequest): AxiosPromise {
        return coreAxiosInstance.post(`${API.Scenario}/GetQueuedWorkByDomainIdAndWorkType/`, data);
    }
    
    static getFastQueuedWorkByDomainIdAndWorkType(data:WorkQueueRequest): AxiosPromise {
        return coreAxiosInstance.post(`${API.Scenario}/GetFastQueuedWorkByDomainIdAndWorkType/`, data);
    }

    static migrateLegacySimulationData(simulationId: number): AxiosPromise {
        return coreAxiosInstance.post(`/api/LegacySimulationSynchronization/SynchronizeLegacySimulation/${simulationId}`);
    }

    static setNoTreatmentBeforeCommitted(simulationId: string): AxiosPromise {
        return coreAxiosInstance.post(`${API.Scenario}/setNoTreatmentBeforeCommitted/${simulationId}`);
    }

    static removeNoTreatmentBeforeCommitted(simulationId: string): AxiosPromise {
        return coreAxiosInstance.post(`${API.Scenario}/RemoveNoTreatmentBeforeCommitted/${simulationId}`);
    }
    
    static getNoTreatmentBeforeCommitted(simulationId: string): AxiosPromise {
        return coreAxiosInstance.get(`${API.Scenario}/GetNoTreatmentBeforeCommitted/${simulationId}`);
    }

    static ConvertSimulationOutputToRelational(simulationId: string): AxiosPromise {
        return coreAxiosInstance.post(`${API.Scenario}/ConvertSimulationOutputToRelational/${simulationId}`);
    } 

    static upsertValidateSimulation(networkId: string, simulationId: string | undefined): AxiosPromise {
        return coreAxiosInstance.post(`${API.Scenario}/ValidateSimulation/${networkId}/${simulationId}`);
    }
}
