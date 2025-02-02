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
import { LibraryUser } from '@/shared/models/iAM/user';
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
    libraryUsers: [] as LibraryUser[],
    hasPermittedAccess: false,
    isSharedLibrary: false,
    isDeteriorationModelApiRunning: false,
    distinctPerformanceFactorAttributes: [] as string[]
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
    performanceCurveLibraryUserMutator(
        state: any,
        libraries: LibraryUser[]
    ) {
        state.libraryUsers = clone(libraries);
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
    distinctPerformanceFactorAttributesMutator(
        state: any,
        distinctAttributes: string[],
    ) {
        state.distinctPerformanceFactorAttributes = clone(distinctAttributes);
    },
    PermittedAccessMutator(state: any, status: boolean) {
        state.hasPermittedAccess = status;
    },
    IsSharedPerformanceCurveLibraryMutator(state: any, status: boolean) {
        state.isSharedLibrary = status;
    },
    IsDeteriorationModelApiRunningMutator(state: any, status: boolean) {
        state.isDeteriorationModelApiRunning = status;
    }
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
    async getDistinctScenarioPerformanceFactorAttributeNames(
        { commit }: any
    ) {
        await PerformanceCurveService.GetDistinctScenarioPerformanceFactorAttributeNames().then((response: AxiosResponse) => {
            if (hasValue(response, 'status') && http2XX.test(response.status.toString())) {
                const distinctAttributes: string[] = response.data as string[];
                commit('distinctPerformanceFactorAttributesMutator', distinctAttributes);
            }
        });
    },
    async getScenarioPerformanceCurves(
        { commit }: any,
        scenarioId: string,
    ) {
        await PerformanceCurveService.GetPerformanceCurves(scenarioId).then((response: AxiosResponse) => {
            if (hasValue(response, 'status') && http2XX.test(response.status.toString())) {
                const performanceCurves: PerformanceCurve[] = response.data as PerformanceCurve[];
                commit('scenarioPerformanceCurvesMutator', performanceCurves);
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
            dispatch('setAlertMessage', "A performance curve import has been added to the work queue");
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
            dispatch('setAlertMessage', "A performance curve import has been added to the work queue");
        });
    },
    async getPerformanceCurveLibraryUsers({ commit }: any, libraryId: string) {
        await PerformanceCurveService.GetPerformanceCurveLibraryUsers(libraryId)
        .then((response: AxiosResponse) => {
            if (
                hasValue(response, 'status') && http2XX.test(response.status.toString())
            ) {
                commit('performanceCurveLibraryUserMutator', response.data);                
            }
        });
    },
    async getIsSharedPerformanceCurveLibrary({ dispatch, commit }: any, payload: any) {
        await PerformanceCurveService.getIsSharedLibrary(payload.id).then(
            (response: AxiosResponse) => {
                if (
                hasValue(response, 'status') &&
                    http2XX.test(response.status.toString())
                ) {
                commit('IsSharedPerformanceCurveLibraryMutator', response.data as boolean);
            }
        });
    },
    async getIsDeteriorationModelApiRunning({ commit }: any, payload: any) {
        commit('IsDeteriorationModelApiRunningMutator', payload);
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
