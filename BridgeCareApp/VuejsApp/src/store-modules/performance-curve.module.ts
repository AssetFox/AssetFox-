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
    async upsertPerformanceCurveLibrary(
        { dispatch, commit }: any,
        library: PerformanceCurveLibrary,
    ) {
        await PerformanceCurveService.upsertPerformanceCurveLibrary(
            library,
        ).then((response: AxiosResponse) => {
            if (
                hasValue(response, 'status') &&
                http2XX.test(response.status.toString())
            ) {
                const message: string = any(
                    propEq('id', library.id),
                    state.performanceCurveLibraries,
                )
                    ? 'Updated performance curve library'
                    : 'Added performance curve library';

                commit('performanceCurveLibraryMutator', library);
                commit('selectedPerformanceCurveLibraryMutator', library.id);

                dispatch('addSuccessNotification', { message: message });
            }
        });
    },
    async getScenarioPerformanceCurves({ commit }: any, scenarioId: string) {
        await PerformanceCurveService.getScenarioPerformanceCurves(
            scenarioId,
        ).then((response: AxiosResponse) => {
            if (hasValue(response, 'data')) {
                commit(
                    'scenarioPerformanceCurvesMutator',
                    response.data as PerformanceCurve[],
                );
            }
        });
    },
    async upsertScenarioPerformanceCurves(
        { dispatch, commit }: any,
        payload: any,
    ) {
        await PerformanceCurveService.upsertScenarioPerformanceCurves(
            payload.scenarioPerformanceCurves,
            payload.scenarioId,
        ).then((response: AxiosResponse) => {
            if (
                hasValue(response, 'status') &&
                http2XX.test(response.status.toString())
            ) {
                commit(
                    'scenarioPerformanceCurvesMutator',
                    payload.scenarioPerformanceCurves,
                );
                dispatch('addSuccessNotification', {
                    message: "Modified scenario's performance curves",
                });
            }
        });
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
                    message: 'Deleted performance curve library',
                });
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
