import {CriterionLibrary, emptyCriterionLibrary} from '@/shared/models/iAM/criteria';
import {any, append, clone, find, findIndex, propEq, reject, update} from 'ramda';
import CriteriaEditorService from '@/services/criteria-editor.service';
import {AxiosResponse} from 'axios';
import {hasValue} from '@/shared/utils/has-value-util';
import {http2XX} from '@/shared/utils/http-utils';

const state = {
    criterionLibraries: [] as CriterionLibrary[],
    selectedCriterionLibrary: clone(emptyCriterionLibrary) as CriterionLibrary
};

const mutations = {
    criterionLibrariesMutator(state: any, criteriaLibraries: CriterionLibrary[]) {
        state.criterionLibraries = clone(criteriaLibraries);
    },
    selectedCriterionLibraryMutator(state: any, libraryId: string) {
        if (any(propEq('id', libraryId), state.criterionLibraries)) {
            state.selectedCriterionLibrary = find(propEq('id', libraryId), state.criterionLibraries);
        } else {
            state.selectedCriterionLibrary = clone(emptyCriterionLibrary);
        }
    },
    addedOrUpdatedCriterionLibraryMutator(state: any, library: CriterionLibrary) {
        state.criterionLibraries = any(propEq('id', library.id), state.criterionLibraries)
            ? update(findIndex(propEq('id', library.id), state.criterionLibraries),
                library, state.criterionLibraries)
            : append(library, state.criterionLibraries);
    },
    deletedCriterionLibraryMutator(state: any, deletedCriteriaLibraryId: string) {
        state.criterionLibraries = reject(propEq('id', deletedCriteriaLibraryId), state.criterionLibraries);
    }
};

const actions = {
    selectCriterionLibrary({commit}: any, payload: any) {
        commit('selectedCriterionLibraryMutator', payload.libraryId);
    },
    async getCriterionLibraries({commit}: any) {
        await CriteriaEditorService.getCriterionLibraries()
            .then((response: AxiosResponse) => {
                if (hasValue(response, 'data')) {
                    commit('criterionLibrariesMutator', response.data as CriterionLibrary[]);
                }
            });
    },
    async addOrUpdateCriterionLibrary({commit, dispatch}: any, payload: any) {
        await CriteriaEditorService.addOrUpdateCriterionLibrary(payload.library)
            .then((response: AxiosResponse) => {
                if (hasValue(response, 'status') && http2XX.test(response.status.toString())) {
                    const library: CriterionLibrary = payload.library;
                    const message: string = any(propEq('id', library.id), state.criterionLibraries)
                        ? 'Updated criterion library'
                        : 'Added criterion library';
                    commit('addedOrUpdatedCriterionLibraryMutator', library);
                    commit('selectedCriterionLibraryMutator', library.id);
                    dispatch('setSuccessMessage', {message: message});
                }
            });
    },
    async deleteCriterionLibrary({commit, dispatch}: any, payload: any) {
        await CriteriaEditorService.deleteCriterionLibrary(payload.libraryId)
            .then((response: AxiosResponse) => {
                if (hasValue(response, 'status') && http2XX.test(response.status.toString())) {
                    commit('deletedCriterionLibraryMutator', payload.libraryId);
                    dispatch('setSuccessMessage', {message: 'Deleted criterion library'});
                }
            });
    }
};

const getters = {};

export default {
    state,
    getters,
    actions,
    mutations
};
