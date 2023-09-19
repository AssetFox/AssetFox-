import { AxiosPromise } from 'axios';
import { API, coreAxiosInstance } from '@/shared/utils/axios-instance';
import {CommittedProjectFillTreatmentValues, SectionCommittedProject } from '@/shared/models/iAM/committed-projects';
import { Network } from '@/shared/models/iAM/network';
import { PagingRequest, PaginSync } from '@/shared/models/iAM/paging';

export default class CommittedProjectsService { 
    static getCommittedProjectTemplate(networkId: string): AxiosPromise {
        return coreAxiosInstance.get(
            `${API.CommittedProject}/CommittedProjectTemplate/${networkId}`,
        );
    }
    static getUploadedCommittedProjectTemplate(): AxiosPromise {
        return coreAxiosInstance.get(
            `${API.CommittedProject}/DownloadCommittedProjectTemplate`,
        );
    }
    static getUploadedCommittedProjectTemplates(): AxiosPromise {
        return coreAxiosInstance.get(
            `${API.CommittedProject}/getUploadedCommittedProjectTemplates`,
        );
    }
    static addCommittedProjectTemplate(file: File): AxiosPromise {
        var reader = new FileReader();
        reader.readAsDataURL(file);
        let formData = new FormData();
        formData.append('file', file);
        return coreAxiosInstance.post(
            `${API.CommittedProject}/AddCommittedProjectTemplate`,
            formData,
            {headers: {'Content-Type': 'multipart/form-data'}},
        );
    }
    static getSelectedCommittedProjectTemplate(filename: string): AxiosPromise {
        return coreAxiosInstance.get(
            `${API.CommittedProject}/DownloadSelectedCommittedProjectTemplate/${filename}`,
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
    static importCommittedProjectTemplate(file: File): AxiosPromise {
        var reader = new FileReader();
        reader.readAsDataURL(file);
        let formData = new FormData();
        formData.append('file', file);
        return coreAxiosInstance.post(
            `${API.CommittedProject}/SetCommittedProjectTemplate`,
            formData,
            {headers: {'Content-Type': 'multipart/form-data'}},
        );
    }
    static getCommittedProjectsPage(scenarioId: string, data:PagingRequest<SectionCommittedProject>): AxiosPromise {
        return coreAxiosInstance.post(
            `${API.CommittedProject}/GetSectionCommittedProjectsPage/${scenarioId}`, data
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

    static upsertCommittedProjects(scenarioId: string, data: PaginSync<SectionCommittedProject>): AxiosPromise {
        return coreAxiosInstance.post(
            `${API.CommittedProject}/UpsertSectionCommittedProjects/${scenarioId}`, data
        );
    }

    static validateAssetExistence(data: Network, brkey: string){
        return coreAxiosInstance.post(
            `${API.CommittedProject}/ValidateAssetExistence/${brkey}`, data
        );
    }

    static validateExistenceOfAssets(data: string[], networkId: string){
        return coreAxiosInstance.post(
            `${API.CommittedProject}/ValidateExistenceOfAssets/${networkId}`, data
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

    static getProjectSources(): AxiosPromise {
        return coreAxiosInstance.get(`${API.CommittedProject}/projectsources`);
    }
    
    static FillTreatmentValues(data: CommittedProjectFillTreatmentValues){
        return coreAxiosInstance.post(
            `${API.CommittedProject}/FillTreatmentValues`, data
        );
    }
}
