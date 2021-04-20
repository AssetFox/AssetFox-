import {AxiosPromise} from 'axios';
import { API, axiosInstance, coreAxiosInstance } from '@/shared/utils/axios-instance';
import {Scenario} from '@/shared/models/iAM/scenario';

export default class CommittedProjectsService {
    static importCommittedProjects(files: File[], applyNoTreatment: boolean, selectedScenarioId: string): AxiosPromise {
        let formData = new FormData();

        for (let i = 0; i < files.length; i++) {
            let file = files[i];
            formData.append('files[' + i + ']', file);
        }
        formData.append('applyNoTreatment', applyNoTreatment ? '1' : '0');
        formData.append('simulationId', selectedScenarioId);

        return coreAxiosInstance.post(`${API.CommittedProject}/ImportCommittedProjects`, formData,
          {headers: {'Content-Type': 'multipart/form-data'}});
    }

    static exportCommittedProjects(scenarioId: string): AxiosPromise {
        return coreAxiosInstance.get(`${API.CommittedProject}/ExportCommittedProjects/${scenarioId}`);
    }

    static deleteCommittedProjects(scenarioId: string): AxiosPromise {
        return coreAxiosInstance.delete(`${API.CommittedProject}/DeleteCommittedProjects/${scenarioId}`);
    }
}
    

