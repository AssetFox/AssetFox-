import {
    emptyPerformanceCurveLibrary,
    PerformanceCurve,
    PerformanceCurveLibrary,
} from '@/shared/models/iAM/performance';
import {
    any,
    append,
    clone,
    find,
    findIndex,
    propEq,
    reject,
    update,
} from 'ramda';
import PerformanceCurveService from '@/services/performance-curve.service';
import { AxiosResponse } from 'axios';
import { hasValue } from '@/shared/utils/has-value-util';
import { http2XX } from '@/shared/utils/http-utils';

const state = {
    performanceCurveLibraries: [] as PerformanceCurveLibrary[],
    selectedPerformanceCurveLibrary: clone(
        emptyPerformanceCurveLibrary,
    ) as PerformanceCurveLibrary,
    scenarioPerformanceCurves: [] as PerformanceCurve[],
    hasPermittedAccess: false,
};

const mutations = {
    performanceCurveLibrariesMutator(
        state: any,
        libraries: PerformanceCurveLibrary[],
    ) {
        state.performanceCurveLibraries = clone(libraries);
    },
    selectedPerformanceCurveLibraryMutator(state: any, libraryId: string) {
        if (any(propEq('id', libraryId), state.performanceCurveLibraries)) {
            state.selectedPerformanceCurveLibrary = find(
                propEq('id', libraryId),
                state.performanceCurveLibraries,
            );
        } else {
            state.selectedPerformanceCurveLibrary = clone(
                emptyPerformanceCurveLibrary,
            );
        }
    },
    performanceCurveLibraryMutator(
        state: any,
        library: PerformanceCurveLibrary,
    ) {
        state.performanceCurveLibraries = any(
            propEq('id', library.id),
            state.performanceCurveLibraries,
        )
            ? update(
                  findIndex(
                      propEq('id', library.id),
                      state.performanceCurveLibraries,
                  ),
                  library,
                  state.performanceCurveLibraries,
              )
            : append(library, state.performanceCurveLibraries);
    },
    scenarioPerformanceCurvesMutator(
        state: any,
        performanceCurves: PerformanceCurve[],
    ) {
        state.scenarioPerformanceCurves = clone(performanceCurves);
    },
    PermittedAccessMutator(state: any, status: boolean) {
        state.hasPermittedAccess = status;
    },
};

const actions = {
    selectPerformanceCurveLibrary({ commit }: any, libraryId: string) {
        commit('selectedPerformanceCurveLibraryMutator', libraryId);
    },
    async getPerformanceCurveLibraries({ commit }: any) {
        await PerformanceCurveService.getPerformanceCurveLibraries().then(
            (response: AxiosResponse<any[]>) => {
                if (hasValue(response, 'data')) {
                    commit(
                        'performanceCurveLibrariesMutator',
                        response.data as PerformanceCurveLibrary[],
                    );
                }
            },
        );
    },

    async deletePerformanceCurveLibrary(
        { dispatch, commit }: any,
        libraryId: string,
    ) {
        await PerformanceCurveService.deletePerformanceCurveLibrary(
            libraryId,
        ).then((response: AxiosResponse) => {
            if (
                hasValue(response, 'status') &&
                http2XX.test(response.status.toString())
            ) {
                const performanceCurveLibraries: PerformanceCurveLibrary[] = reject(
                    propEq('id', libraryId),
                    state.performanceCurveLibraries,
                );

                commit(
                    'performanceCurveLibrariesMutator',
                    performanceCurveLibraries,
                );

                dispatch('addSuccessNotification', {
                    message: 'Deleted deterioration model library',
                });
            }
        });
    },
    async importScenarioPerformanceCurvesFile(
        { commit, dispatch }: any,
        payload: any,
    ) {
        await PerformanceCurveService.importPerformanceCurves(
            payload.file,
            payload.id,
            true,
            payload.currentUserCriteriaFilter,
        ).then((response: AxiosResponse) => {
            if (hasValue(response, 'data')) {
                const performanceCurves: PerformanceCurve[] = response.data as PerformanceCurve[];
                commit('scenarioPerformanceCurvesMutator', performanceCurves);
                dispatch('addSuccessNotification', {
                    message: 'Deterioration models file imported',
                });
            }
        });
    },
    async importLibraryPerformanceCurvesFile(
        { commit, dispatch }: any,
        payload: any,
    ) {
        await PerformanceCurveService.importPerformanceCurves(
            payload.file,
            payload.id,
            false,
            payload.currentUserCriteriaFilter,
        ).then((response: AxiosResponse) => {
            if (hasValue(response, 'data')) {
                const library: PerformanceCurveLibrary = response.data as PerformanceCurveLibrary;
                commit('performanceCurveLibraryMutator', library);
                commit('selectedPerformanceCurveLibraryMutator', library.id);               
                dispatch('addSuccessNotification', {
                    message: 'Deterioration Models file imported',
                });
            }
        });
    },
    async getHasPermittedAccess({ commit }: any)
    {
        await PerformanceCurveService.getHasPermittedAccess()
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
    mutations,
};
