import { AxiosPromise } from 'axios';
import { API, coreAxiosInstance } from '@/shared/utils/axios-instance';

export default class CommittedProjectsService { 
    static GetCommittedProjectTemplate(): AxiosPromise {
        return coreAxiosInstance.get(
            `${API.CommittedProject}/CommittedProjectTemplate`,
        );
    }
    static exportCommittedProjects(scenarioId: string): AxiosPromise {
        return coreAxiosInstance.get(
            `${API.CommittedProject}/ExportCommittedProjects/${scenarioId}`,
        );
    }
    static GetCommittedProjects(scenarioId: string): AxiosPromise {
        return coreAxiosInstance.get(
            `${API.CommittedProject}/GetSectionCommittedProjects/${scenarioId}`,
        );
    }
    static DeleteSimulationCommittedProjects(scenarioId: string): AxiosPromise {
        return coreAxiosInstance.delete(
            `${API.CommittedProject}/DeleteSimulationCommittedProjects/${scenarioId}`,
        );
    }
    // static DeleteSpecificCommittedProjects(data: string[]): AxiosPromise {
    //     return coreAxiosInstance.delete(
    //         `${API.CommittedProject}/DeleteSimulationCommittedProjects`, data
    //     );
    // }
    static importCommittedProjects(
        file: File,
        applyNoTreatment: boolean,
        selectedScenarioId: string,
    ): AxiosPromise {
        let formData = new FormData();

        formData.append('file', file);
        formData.append('applyNoTreatment', applyNoTreatment ? '1' : '0');
        formData.append('simulationId', selectedScenarioId);

        return coreAxiosInstance.post(
            `${API.CommittedProject}/ImportCommittedProjects`,
            formData,
            { headers: { 'Content-Type': 'multipart/form-data' } },
        );
    }   

    static UpsertCommittedProjects(data: string): AxiosPromise {
        return coreAxiosInstance.post(
            `${API.CommittedProject}/UpsertSectionCommittedProjects/${scenarioId}`,
        );
    }
}
