import CommittedProjectsService from "@/services/committed-projects.service";
import { SectionCommittedProject } from "@/shared/models/iAM/committed-projects";
import { hasValue } from "@/shared/utils/has-value-util";
import { http2XX } from "@/shared/utils/http-utils";
import { AxiosResponse } from "axios";
import { any, append, clone, contains, findIndex, propEq, update } from "ramda";

const state = {
    sectionCommittedProjects: [] as SectionCommittedProject[],
    committedProjectTemplate : ''
};

const mutations = {
    sectionCommittedProjectsCloneMutator(state: any, sectionCommittedProjects: SectionCommittedProject[] ) {
        state.sectionCommittedProjects = clone(sectionCommittedProjects);
    },
    committedProjectTemplateMutator(state: any, committedProjectTemplate: string){
        state.committedProjectTemplate = committedProjectTemplate;
    },
    sectionCommittedProjectsMutator(state: any, sectionCommittedProjects: SectionCommittedProject[] ) {
        sectionCommittedProjects.forEach((proj: SectionCommittedProject) => {
            state.sectionCommittedProjects = any(
                propEq('id', proj.id),
                state.sectionCommittedProjects,
            )
                ? update(
                      findIndex(
                          propEq('id', proj.id),
                          state.sectionCommittedProjects,
                      ),
                      proj,
                      state.sectionCommittedProjects,
                  )
                : append(proj, state.sectionCommittedProjects);
        })
    },
    deleteSelectedCommittedProjectMutator(state: any, ids: string[]){
        state.sectionCommittedProjects = state.sectionCommittedProjects.filter((proj: SectionCommittedProject) =>{
            !contains(proj.id, ids)
        })
    }
}

const actions = {
    async getCommittedProjects({ commit }: any, scenarioId: string) {
        await CommittedProjectsService.getCommittedProjects(scenarioId)
            .then((response: AxiosResponse) => {
                if (hasValue(response, 'data')) {
                    commit('sectionCommittedProjectsCloneMutator',response.data as SectionCommittedProject[]);
                }
        });
    },
    async getUploadedCommittedProjectTemplates({ commit }: any) {
        await CommittedProjectsService.getUploadedCommittedProjectTemplates()
            .then((response: AxiosResponse) => {
                if (hasValue(response, 'data')) {
                    commit('getCommittedProjectsCloneMutator',response.data as string[]);
                }
        });
    },
    async deleteSpecificCommittedProjects({commit, dispatch}: any, ids: string[]){
        await CommittedProjectsService.deleteSpecificCommittedProjects(ids)
            .then((response: AxiosResponse) => {
                if(hasValue(response, 'status') && http2XX.test(response.status.toString())){
                    commit('deleteSelectedCommittedProjectMutator', ids);
                    dispatch('addSuccessNotification', {
                        message: 'Deleted selected committed projects',
                    });           
                }
            })
    },
    async importCommittedProjectTemplate({commit, dispatch}: any, payload: File) {
        await CommittedProjectsService.importCommittedProjectTemplate(payload)
        .then(async (response: AxiosResponse) => {
            if (response.status >= 200 && response.status < 300) {
                const base64 = await convertFileToBase64(payload);
                commit('isSuccessfulImportMutator', true);
                dispatch('addSuccessNotification',{
                    message: 'Committed Project Template imported'
                });
            }
        });
    },
    async addCommittedProjectTemplate({commit, dispatch}: any, payload: File) {
        await CommittedProjectsService.addCommittedProjectTemplate(payload)
        .then(async (response: AxiosResponse) => {
            if (response.status >= 200 && response.status < 300) {
                const base64 = await convertFileToBase64(payload);
                commit('isSuccessfulImportMutator', true);
                dispatch('addSuccessNotification',{
                    message: 'Committed Project Template imported'
                });
            }
        });
    },
    async deleteSimulationCommittedProjects({commit, dispatch}: any, scenarioId: string){
        await CommittedProjectsService.deleteSimulationCommittedProjects(scenarioId)
            .then((response: AxiosResponse) => {
                if(hasValue(response, 'status') && http2XX.test(response.status.toString())){
                    commit('sectionCommittedProjectsCloneMutator', []);
                    dispatch('addSuccessNotification', {
                        message: 'Deleted committed projects',
                    });           
                }
            })
    },
    async importCommittedProjects( { commit, dispatch }: any,payload: any,){
        await CommittedProjectsService.importCommittedProjects(
            payload.file,
            payload.selectedScenarioId
        ).then((response: AxiosResponse) => {
            if (hasValue(response, 'data')) {
                //todo more needs to be done here
                dispatch('addSuccessNotification', {
                    message: 'Investment budgets file imported',
                });
            }
        });
    },
}

function convertFileToBase64(file: File): Promise<string> {
    return new Promise<string>(() => {
      const reader = new FileReader();
      reader.readAsDataURL(file);
    });
  }
  
export default {
    state,
    actions,
    mutations
}