import {CriterionLibrary, emptyCriterionLibrary} from '@/shared/models/iAM/criteria';
import {any, append, clone, find, findIndex, isNil, propEq, reject, update} from 'ramda';
import CriterionLibraryService from '@/services/criterion-library.service';
import {AxiosResponse} from 'axios';
import {hasValue} from '@/shared/utils/has-value-util';
import {http2XX} from '@/shared/utils/http-utils';
import { getBlankGuid } from '@/shared/utils/uuid-utils';

const state = {
    criterionLibraries: [] as CriterionLibrary[],
    selectedCriterionLibrary: clone(emptyCriterionLibrary) as CriterionLibrary,
    selectedCriterionIsValid: false as boolean,
    scenarioRelatedCriteria: clone(emptyCriterionLibrary) as CriterionLibrary,
    hasPermittedAccess: false,
};

const mutations = {
    criterionLibrariesMutator(state: any, criteriaLibraries: CriterionLibrary[]) {
        state.criterionLibraries = clone(criteriaLibraries);
    },
    selectedCriterionLibraryMutator(state: any, criterionLibrary: CriterionLibrary) {
        state.selectedCriterionLibrary = clone(criterionLibrary);
    },
    addedOrUpdatedCriterionLibraryMutator(state: any, library: CriterionLibrary) {
        state.criterionLibraries = any(propEq('id', library.id), state.criterionLibraries)
            ? update(findIndex(propEq('id', library.id), state.criterionLibraries),
                library, state.criterionLibraries)
            : append(library, state.criterionLibraries);
    },
    deletedCriterionLibraryMutator(state: any, deletedCriteriaLibraryId: string) {
        state.criterionLibraries = reject(propEq('id', deletedCriteriaLibraryId), state.criterionLibraries);
    },
    selectedCriterionIsValidMutator(state: any, isValid: boolean) {
        state.selectedCriterionIsValid = isValid;
    },
    scenarioRelatedCriterionMutator(state: any, library: CriterionLibrary){
        state.scenarioRelatedCriteria = clone(library);
    },
    upsertScenarioRelatedCriteriaMutator(state: any, library: CriterionLibrary){
        state.scenarioRelatedCriteria = clone(library);
    },
    PermittedAccessMutator(state: any, status: boolean) {
        state.hasPermittedAccess = status;
    },
};

const actions = {
    selectCriterionLibrary({commit}: any, payload: any) {
        commit('selectedCriterionLibraryMutator', payload.libraryId);
    },
    async getSelectedCriterionLibrary({commit}: any, payload: any) {
        if(!isNil(payload.libraryId) && payload.libraryId != getBlankGuid()){
            await CriterionLibraryService.getSelectedCriterionLibrary(payload.libraryId)
            .then((response: AxiosResponse) => {
                if (hasValue(response, 'data')) {
                    commit('selectedCriterionLibraryMutator', response.data as CriterionLibrary);
                }
            });
        }
        else{
            var emptyData = clone(emptyCriterionLibrary) as CriterionLibrary;
            commit('selectedCriterionLibraryMutator', emptyData);
        }
    },
    async selectScenarioRelatedCriterion({commit}: any, payload: any) {
        if(payload.libraryId != getBlankGuid()){
            await CriterionLibraryService.getSelectedCriterionLibrary(payload.libraryId)
            .then((response: AxiosResponse) => {
                if (hasValue(response, 'data')) {
                    commit('scenarioRelatedCriterionMutator', response.data as CriterionLibrary);
                }
            });
        }
        else{
            var emptyData = clone(emptyCriterionLibrary) as CriterionLibrary;
            commit('scenarioRelatedCriterionMutator', emptyData);
        }
    },
    setSelectedCriterionIsValid({commit}: any, payload: any) {
        commit('selectedCriterionIsValidMutator', payload.isValid);
    },
    async getCriterionLibraries({commit}: any) {
        await CriterionLibraryService.getCriterionLibraries()
            .then((response: AxiosResponse) => {
                if (hasValue(response, 'data')) {
                    commit('criterionLibrariesMutator', response.data as CriterionLibrary[]);
                }
            });
    },
    async upsertCriterionLibrary({commit, dispatch}: any, payload: any) : Promise<string> {
        var returningId = getBlankGuid();
        await CriterionLibraryService.upsertCriterionLibrary(payload.library)
            .then((response: AxiosResponse) => {
                if (hasValue(response, 'status') && http2XX.test(response.status.toString())) {
                    const library: CriterionLibrary = payload.library;

                    const isExistingCriteria: boolean = any(propEq('id', library.id), state.criterionLibraries);
                    const message: string = isExistingCriteria
                        ? 'Updated criterion library'
                        : 'Added criterion library';
                    commit('addedOrUpdatedCriterionLibraryMutator', library);
                    if(!library.isSingleUse){
                        commit('selectedCriterionLibraryMutator', library);
                    }
                    else{
                            commit('scenarioRelatedCriterionMutator', library);
                    }
                dispatch('addSuccessNotification', { message: message });
                returningId = response.data;
            }
            return returningId;
        });
        return returningId;
},
async deleteCriterionLibrary({commit, dispatch}: any, payload: any) {
    await CriterionLibraryService.deleteCriterionLibrary(payload.libraryId)
        .then((response: AxiosResponse) => {
            if (hasValue(response, 'status') && http2XX.test(response.status.toString())) {
                commit('deletedCriterionLibraryMutator', payload.libraryId);
                dispatch('setSuccessMessage', {message: 'Deleted criterion library'});
                commit('selectedCriterionIsValidMutator', false);
            }
        });
},
upsertSelectedScenarioRelatedCriterion({commit, dispatch}: any, payload: any){
    commit('upsertScenarioRelatedCriteriaMutator', payload.library);
},
async getHasPermittedAccess({ commit }: any)
    {
        await CriterionLibraryService.getHasPermittedAccess()
        .then((response: AxiosResponse) => {
            if (
                hasValue(response, 'status') &&
                http2XX.test(response.status.toString())
            ) {
                const hasPermittedAccess: boolean = response.data as boolean;
                commit('PermittedAccessMutator', hasPermittedAccess);
            }
        });
    },
};

const getters = {};

export default {
state,
getters,
actions,
mutations
};
