import {emptyRemainingLifeLimitLibrary, RemainingLifeLimitLibrary} from '@/shared/models/iAM/remaining-life-limit';
import {any, append, clone, find, findIndex, propEq, reject, update} from 'ramda';
import {AxiosResponse} from 'axios';
import {hasValue} from '@/shared/utils/has-value-util';
import {http2XX} from '@/shared/utils/http-utils';
import {getBlankGuid} from '@/shared/utils/uuid-utils';
import {getAppliedLibrary, hasAppliedLibrary, unapplyLibrary} from '@/shared/utils/library-utils';
import RemainingLifeLimitService from '@/services/remaining-life-limit.service';

const state = {
    remainingLifeLimitLibraries: [] as RemainingLifeLimitLibrary[],
    selectedRemainingLifeLimitLibrary: clone(emptyRemainingLifeLimitLibrary) as RemainingLifeLimitLibrary
};

const mutations = {
    remainingLifeLimitLibrariesMutator(state: any, libraries: RemainingLifeLimitLibrary[]) {
        state.remainingLifeLimitLibraries = clone(libraries);
    },
    selectedRemainingLifeLimitLibraryMutator(state: any, libraryId: string) {
        if (any(propEq('id', libraryId), state.remainingLifeLimitLibraries)) {
            state.selectedRemainingLifeLimitLibrary = find(propEq('id', libraryId), state.remainingLifeLimitLibraries);
        } else {
            state.selectedRemainingLifeLimitLibrary = clone(emptyRemainingLifeLimitLibrary);
        }
    },
    addedOrUpdatedRemainingLifeLimitLibraryMutator(state: any, library: RemainingLifeLimitLibrary) {
        state.remainingLifeLimitLibraries = any(propEq('id', library.id), state.remainingLifeLimitLibraries)
            ? update(findIndex(propEq('id', library.id), state.remainingLifeLimitLibraries),
                library, state.remainingLifeLimitLibraries)
            : append(library, state.remainingLifeLimitLibraries);
    },
    deletedRemainingLifeLimitLibraryMutator(state: any, deletedLibraryId: string) {
        if (any(propEq('id', deletedLibraryId), state.remainingLifeLimitLibraries)) {
            state.remainingLifeLimitLibraries = reject(
                (library: RemainingLifeLimitLibrary) => deletedLibraryId === library.id,
                state.remainingLifeLimitLibraries
            );
        }
    }
};

const actions = {
    selectRemainingLifeLimitLibrary({commit}: any, payload: any) {
        commit('selectedRemainingLifeLimitLibraryMutator', payload.libraryId);
    },
    async getRemainingLifeLimitLibraries({commit}: any) {
        await RemainingLifeLimitService.getRemainingLifeLimitLibraries()
            .then((response: AxiosResponse<any[]>) => {
                if (hasValue(response, 'data')) {
                    commit('remainingLifeLimitLibrariesMutator', response.data as RemainingLifeLimitLibrary[]);
                }
            });
    },
    async upsertRemainingLifeLimitLibrary({dispatch, commit}: any, payload: any) {
        await RemainingLifeLimitService.upsertRemainingLifeLimitLibrary(payload.library, payload.scenarioId)
            .then((response: AxiosResponse) => {
                if (hasValue(response, 'status') && http2XX.test(response.status.toString())) {
                    if (payload.scenarioId !== getBlankGuid() && hasAppliedLibrary(state.remainingLifeLimitLibraries, payload.scenarioId)) {
                        const unAppliedLibrary: RemainingLifeLimitLibrary = unapplyLibrary(getAppliedLibrary(
                            state.remainingLifeLimitLibraries, payload.scenarioId), payload.scenarioId);
                        commit('addedOrUpdatedRemainingLifeLimitLibraryMutator', unAppliedLibrary);
                    }

                    const library: RemainingLifeLimitLibrary = {
                        ...payload.library,
                        appliedScenarioIds: payload.scenarioId !== getBlankGuid() &&
                        payload.library.appliedScenarioIds.indexOf(payload.scenarioId) === -1
                            ? append(payload.scenarioId, payload.library.appliedScenarioIds)
                            : payload.library.appliedScenarioIds
                    };

                    const message: string = any(propEq('id', library.id), state.remainingLifeLimitLibraries)
                        ? 'Updated remaining life limit library'
                        : 'Added remaining life limit library';
                    commit('addedOrUpdatedRemainingLifeLimitLibraryMutator', library);
                    commit('selectedRemainingLifeLimitLibraryMutator', library.id);
                    dispatch('setSuccessMessage', {message: message});
                }
            });
    },
    async deleteRemainingLifeLimitLibrary({dispatch, commit, state}: any, payload: any) {
        await RemainingLifeLimitService.deleteRemainingLifeLimitLibrary(payload.libraryId)
            .then((response: AxiosResponse) => {
                if (hasValue(response, 'status') && http2XX.test(response.status.toString())) {
                    commit('deletedRemainingLifeLimitLibraryMutator', payload.libraryId);
                    dispatch('setSuccessMessage', {message: 'Deleted remaining life limit library'});
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
