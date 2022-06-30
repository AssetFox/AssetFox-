import CommittedProjectsService from "@/services/committed-projects.service";
import { SectionCommittedProject } from "@/shared/models/iAM/committed-projects";
import { hasValue } from "@/shared/utils/has-value-util";
import { AxiosResponse } from "axios";
import { any, append, clone, contains, findIndex, propEq, update } from "ramda";

const state = {
    sectionCommittedProjects: [] as SectionCommittedProject[]
};

const mutations = {
    sectionCommittedProjectsCloneMutator(state: any, sectionCommittedProjects: SectionCommittedProject[] ) {
        state.sectionCommittedProjects = clone(sectionCommittedProjects);
    },
    sectionCommittedProjectsMutator(state: any, sectionCommittedProjects: SectionCommittedProject[] ) {
        sectionCommittedProjects.forEach((proj: SectionCommittedProject) => {
            state.sectionCommittedProjects = any(
                propEq('id', proj.id),
                state.budgetPriorityLibraries,
            )
                ? update(
                      findIndex(
                          propEq('id', proj.id),
                          state.budgetPriorityLibraries,
                      ),
                      proj,
                      state.budgetPriorityLibraries,
                  )
                : append(proj, state.budgetPriorityLibraries);
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
                    commit('sectionCommittedProjectsCloneMutator',response.data as SectionCommittedProject[],);
                }
        });
    },
    async deleteSpecificCommittedProjects({commit, dispatch}: any, ids: string[]){
        await CommittedProjectsService.deleteSpecificCommittedProjects(ids)
            .then((response: AxiosResponse) => {
                if(hasValue(response, 'data')){
                    commit('deleteSelectedCommittedProjectMutator', ids);
                    dispatch('addSuccessNotification', {
                        message: 'Deleted selected committed projects',
                    });           
                }
            })
    },
    async deleteSimulationCommittedProjects({commit, dispatch}: any, scenarioId: string){
        await CommittedProjectsService.deleteSimulationCommittedProjects(scenarioId)
            .then((response: AxiosResponse) => {
                if(hasValue(response, 'data')){
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
            payload.applyNoTreatment,
            payload.selectedScenarioId,
        ).then((response: AxiosResponse) => {
            if (hasValue(response, 'data')) {
                //todo more needs to be done here
                dispatch('addSuccessNotification', {
                    message: 'Investment budgets file imported',
                });
            }
        });
    },
    async upsertCommittedProjects({commit, dispatch}: any, sectionCommittedProjects:SectionCommittedProject[]){
        await CommittedProjectsService.upsertCommittedProjects(sectionCommittedProjects)
            .then((response: AxiosResponse) => {

            });
    }
}