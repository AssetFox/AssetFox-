import {emptyTreatmentLibrary, TreatmentLibrary} from '@/shared/models/iAM/treatment';
import {any, append, clone, find, findIndex, propEq, reject, update} from 'ramda';
import {AxiosResponse} from 'axios';
import {hasValue} from '@/shared/utils/has-value-util';
import {http2XX} from '@/shared/utils/http-utils';
import {getBlankGuid} from '@/shared/utils/uuid-utils';
import {getAppliedLibrary, hasAppliedLibrary, unapplyLibrary} from '@/shared/utils/library-utils';
import TreatmentService from '@/services/treatment.service';

const state = {
    treatmentLibraries: [] as TreatmentLibrary[],
    selectedTreatmentLibrary: clone(emptyTreatmentLibrary) as TreatmentLibrary
};

const mutations = {
    treatmentLibrariesMutator(state: any, libraries: TreatmentLibrary[]) {
        state.treatmentLibraries = clone(libraries);
    },
    selectedTreatmentLibraryMutator(state: any, libraryId: string) {
        if (any(propEq('id', libraryId), state.treatmentLibraries)) {
            state.selectedTreatmentLibrary = find(propEq('id', libraryId), state.treatmentLibraries);
        } else {
            state.selectedTreatmentLibrary = clone(emptyTreatmentLibrary);
        }
    },
    addedOrUpdatedTreatmentLibraryMutator(state: any, library: TreatmentLibrary) {
        state.treatmentLibraries = any(propEq('id', library.id), state.treatmentLibraries)
            ? update(findIndex(propEq('id', library.id), state.treatmentLibraries),
                library, state.treatmentLibraries)
            : append(library, state.treatmentLibraries);
    },
    deletedTreatmentLibraryMutator(state: any, deletedLibraryId: string) {
        if (any(propEq('id', deletedLibraryId), state.treatmentLibraries)) {
            state.treatmentLibraries = reject(
                (library: TreatmentLibrary) => deletedLibraryId === library.id,
                state.treatmentLibraries
            );
        }
    }
};

const actions = {
    selectTreatmentLibrary({commit}: any, payload: any) {
        commit('selectedTreatmentLibraryMutator', payload.libraryId);
    },
    async getTreatmentLibraries({commit}: any) {
        await TreatmentService.getTreatmentLibraries()
            .then((response: AxiosResponse<any[]>) => {
                if (hasValue(response, 'data')) {
                    commit('treatmentLibrariesMutator', response.data as TreatmentLibrary[]);
                }
            });
    },
    async upsertTreatmentLibrary({dispatch, commit}: any, payload: any) {
        await TreatmentService.upsertTreatmentLibrary(payload.library, payload.scenarioId)
            .then((response: AxiosResponse) => {
                if (hasValue(response, 'status') && http2XX.test(response.status.toString())) {
                    if (payload.scenarioId !== getBlankGuid() && hasAppliedLibrary(state.treatmentLibraries, payload.scenarioId)) {
                        const unAppliedLibrary: TreatmentLibrary = unapplyLibrary(getAppliedLibrary(
                            state.treatmentLibraries, payload.scenarioId), payload.scenarioId);
                        commit('addedOrUpdatedTreatmentLibraryMutator', unAppliedLibrary);
                    }

                    const library: TreatmentLibrary = {
                        ...payload.library,
                        appliedScenarioIds: payload.scenarioId !== getBlankGuid() &&
                        payload.library.appliedScenarioIds.indexOf(payload.scenarioId) === -1
                            ? append(payload.scenarioId, payload.library.appliedScenarioIds)
                            : payload.library.appliedScenarioIds
                    };

                    const message: string = any(propEq('id', library.id), state.treatmentLibraries)
                        ? 'Updated treatment library'
                        : 'Added treatment library';
                    commit('addedOrUpdatedTreatmentLibraryMutator', library);
                    commit('selectedTreatmentLibraryMutator', library.id);
                    dispatch('setSuccessMessage', {message: message});
                }
            });
    },
    async deleteTreatmentLibrary({dispatch, commit, state}: any, payload: any) {
        await TreatmentService.deleteTreatmentLibrary(payload.libraryId)
            .then((response: AxiosResponse) => {
                if (hasValue(response, 'status') && http2XX.test(response.status.toString())) {
                    commit('deletedTreatmentLibraryMutator', payload.libraryId);
                    dispatch('setSuccessMessage', {message: 'Deleted treatment library'});
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
