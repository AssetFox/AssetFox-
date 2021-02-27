import {emptyPerformanceCurveLibrary, PerformanceCurveLibrary} from '@/shared/models/iAM/performance';
import {any, append, clone, find, findIndex, propEq, reject, update} from 'ramda';
import PerformanceCurveService from '@/services/performance-curve.service';
import {AxiosResponse} from 'axios';
import {hasValue} from '@/shared/utils/has-value-util';
import {http2XX} from '@/shared/utils/http-utils';
import {getBlankGuid} from '@/shared/utils/uuid-utils';
import {getAppliedLibrary, hasAppliedLibrary, unapplyLibrary} from '@/shared/utils/library-utils';

const state = {
    performanceCurveLibraries: [] as PerformanceCurveLibrary[],
    selectedPerformanceCurveLibrary: clone(emptyPerformanceCurveLibrary) as PerformanceCurveLibrary
};

const mutations = {
    performanceCurveLibrariesMutator(state: any, libraries: PerformanceCurveLibrary[]) {
        state.performanceCurveLibraries = clone(libraries);
    },
    selectedPerformanceCurveLibraryMutator(state: any, libraryId: string) {
        if (any(propEq('id', libraryId), state.performanceCurveLibraries)) {
            state.selectedPerformanceCurveLibrary = find(propEq('id', libraryId), state.performanceCurveLibraries);
        } else {
            state.selectedPerformanceCurveLibrary = clone(emptyPerformanceCurveLibrary);
        }
    },
    addedOrUpdatedPerformanceCurveLibraryMutator(state: any, library: PerformanceCurveLibrary) {
        state.performanceCurveLibraries = any(propEq('id', library.id), state.performanceCurveLibraries)
            ? update(findIndex(propEq('id', library.id), state.performanceCurveLibraries),
                library, state.performanceCurveLibraries)
            : append(library, state.performanceCurveLibraries);
    },
    deletedPerformanceCurveLibraryMutator(state: any, deletedLibraryId: string) {
        if (any(propEq('id', deletedLibraryId), state.performanceCurveLibraries)) {
            state.performanceCurveLibraries = reject(
                (library: PerformanceCurveLibrary) => library.id === deletedLibraryId,
                state.performanceCurveLibraries
            );
        }
    }
};

const actions = {
    selectPerformanceCurveLibrary({commit}: any, payload: any) {
        commit('selectedPerformanceCurveLibraryMutator', payload.libraryId);
    },
    async getPerformanceCurveLibraries({commit}: any) {
        await PerformanceCurveService.getPerformanceCurveLibraries()
            .then((response: AxiosResponse<any[]>) => {
                if (hasValue(response, 'data')) {
                    commit('performanceCurveLibrariesMutator', response.data as PerformanceCurveLibrary[]);
                }
            });
    },
    async addOrUpdatePerformanceCurveLibrary({dispatch, commit}: any, payload: any) {
        await PerformanceCurveService.addOrUpdatePerformanceCurveLibrary(payload.library, payload.scenarioId)
            .then((response: AxiosResponse) => {
                if (hasValue(response, 'status') && http2XX.test(response.status.toString())) {
                    if (payload.scenarioId !== getBlankGuid() && hasAppliedLibrary(state.performanceCurveLibraries, payload.scenarioId)) {
                        const unAppliedLibrary: PerformanceCurveLibrary = unapplyLibrary(getAppliedLibrary(
                            state.performanceCurveLibraries, payload.scenarioId), payload.scenarioId);
                        commit('addedOrUpdatedPerformanceCurveLibraryMutator', unAppliedLibrary);
                    }

                    const library: PerformanceCurveLibrary = {
                        ...payload.library,
                        appliedScenarioIds: payload.scenarioId !== getBlankGuid() &&
                                            payload.library.appliedScenarioIds.indexOf(payload.scenarioId) === -1
                            ? append(payload.scenarioId, payload.library.appliedScenarioIds)
                            : payload.library.appliedScenarioIds
                    };

                    const message: string = any(propEq('id', library.id), state.performanceCurveLibraries)
                        ? 'Updated performance curve library'
                        : 'Added performance curve library';
                    commit('addedOrUpdatedPerformanceCurveLibraryMutator', library);
                    commit('selectedPerformanceCurveLibraryMutator', library.id);
                    dispatch('setSuccessMessage', {message: message});
                }
            });
    },
    async deletePerformanceCurveLibrary({dispatch, commit}: any, payload: any) {
        await PerformanceCurveService.deletePerformanceCurveLibrary(payload.libraryId)
            .then((response: AxiosResponse) => {
                if (hasValue(response, 'status') && http2XX.test(response.status.toString())) {
                    commit('deletedPerformanceCurveLibraryMutator', payload.libraryId);
                    dispatch('setSuccessMessage', {message: 'Deleted performance curve library'});
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
