import { AxiosPromise } from 'axios';
import { API, coreAxiosInstance } from '@/shared/utils/axios-instance';
import {SectionCommittedProject } from '@/shared/models/iAM/committed-projects';
import { Network } from '@/shared/models/iAM/network';

export default class CommittedProjectsService { 
    static getCommittedProjectTemplate(): AxiosPromise {
        return coreAxiosInstance.get(
            `${API.CommittedProject}/CommittedProjectTemplate`,
        );
    }
    static exportCommittedProjects(scenarioId: string): AxiosPromise {
        return coreAxiosInstance.get(
            `${API.CommittedProject}/ExportCommittedProjects/${scenarioId}`,
        );
    }
    static getCommittedProjects(scenarioId: string): AxiosPromise {
        return coreAxiosInstance.get(
            `${API.CommittedProject}/GetSectionCommittedProjects/${scenarioId}`,
        );
    }
    static deleteSimulationCommittedProjects(scenarioId: string): AxiosPromise {
        return coreAxiosInstance.delete(
            `${API.CommittedProject}/DeleteSimulationCommittedProjects/${scenarioId}`,
        );
    }
    static deleteSpecificCommittedProjects(data: string[]): AxiosPromise {
        return coreAxiosInstance.post(
            `${API.CommittedProject}/DeleteSpecificCommittedProjects`, data
        );
    }
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

    static upsertCommittedProjects(data: SectionCommittedProject[]): AxiosPromise {
        return coreAxiosInstance.post(
            `${API.CommittedProject}/UpsertSectionCommittedProjects`, data
        );
    }

    static ValidateBRKEY(data: Network, brkey: string){
        return coreAxiosInstance.post(
            `${API.CommittedProject}/ValidateAssetExistence/${brkey}`, data
        );
    }

    static GetTreatmetCost(data: SectionCommittedProject, brkeyValue: string){
        return coreAxiosInstance.post(
            `${API.CommittedProject}/GetTreatmetCost/${brkeyValue}`, data
        );
    }

    static GetValidConsequences(data: SectionCommittedProject, brkeyValue: string){
        return coreAxiosInstance.post(
            `${API.CommittedProject}/GetValidConsequences/${brkeyValue}`, data
        );
    }
}
