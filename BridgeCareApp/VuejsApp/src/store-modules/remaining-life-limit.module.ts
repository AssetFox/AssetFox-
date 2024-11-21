import {
    emptyRemainingLifeLimitLibrary,
    RemainingLifeLimit,
    RemainingLifeLimitLibrary,
} from '@/shared/models/iAM/remaining-life-limit';
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
import { AxiosResponse } from 'axios';
import { hasValue } from '@/shared/utils/has-value-util';
import { http2XX } from '@/shared/utils/http-utils';
import { getBlankGuid } from '@/shared/utils/uuid-utils';
import {
    getAppliedLibrary,
    hasAppliedLibrary,
    unapplyLibrary,
} from '@/shared/utils/library-utils';
import RemainingLifeLimitService from '@/services/remaining-life-limit.service';

const state = {
    remainingLifeLimitLibraries: [] as RemainingLifeLimitLibrary[],
    selectedRemainingLifeLimitLibrary: clone(
        emptyRemainingLifeLimitLibrary,
    ) as RemainingLifeLimitLibrary,
    scenarioRemainingLifeLimits: [] as RemainingLifeLimit[],
    hasPermittedAccess: false,
    isSharedLibrary: false,
};

const mutations = {
    remainingLifeLimitLibrariesMutator(
        state: any,
        libraries: RemainingLifeLimitLibrary[],
    ) {
        state.remainingLifeLimitLibraries = clone(libraries);
    },
    selectedRemainingLifeLimitLibraryMutator(state: any, libraryId: string) {
        if (any(propEq('id', libraryId), state.remainingLifeLimitLibraries)) {
            state.selectedRemainingLifeLimitLibrary = find(
                propEq('id', libraryId),
                state.remainingLifeLimitLibraries,
            );
        } else {
            state.selectedRemainingLifeLimitLibrary = clone(
                emptyRemainingLifeLimitLibrary,
            );
        }
    },
    addedOrUpdatedRemainingLifeLimitLibraryMutator(
        state: any,
        library: RemainingLifeLimitLibrary,
    ) {
        state.remainingLifeLimitLibraries = any(
            propEq('id', library.id),
            state.remainingLifeLimitLibraries,
        )
            ? update(
                  findIndex(
                      propEq('id', library.id),
                      state.remainingLifeLimitLibraries,
                  ),
                  library,
                  state.remainingLifeLimitLibraries,
              )
            : append(library, state.remainingLifeLimitLibraries);
    },
    deletedRemainingLifeLimitLibraryMutator(
        state: any,
        deletedLibraryId: string,
    ) {
        if (
            any(
                propEq('id', deletedLibraryId),
                state.remainingLifeLimitLibraries,
            )
        ) {
            state.remainingLifeLimitLibraries = reject(
                (library: RemainingLifeLimitLibrary) =>
                    deletedLibraryId === library.id,
                state.remainingLifeLimitLibraries,
            );
        }
    },
    scenarioRemainingLifeLimitMutator(
        state: any,
        remainingLifeLimits: RemainingLifeLimit[],
    ) {
        state.scenarioRemainingLifeLimits = clone(remainingLifeLimits);
    },
    PermittedAccessMutator(state: any, status: boolean) {
        state.hasPermittedAccess = status;
    },
    IsSharedLibraryMutator(state: any, status: boolean) {
        state.isSharedLibrary = status;
    }
};

const actions = {
    selectRemainingLifeLimitLibrary({ commit }: any, payload: any) {
        commit('selectedRemainingLifeLimitLibraryMutator', payload.libraryId);
    },
    async getRemainingLifeLimitLibraries({ commit }: any) {
        await RemainingLifeLimitService.getRemainingLifeLimitLibraries().then(
            (response: AxiosResponse<any[]>) => {
                if (hasValue(response, 'data')) {
                    commit(
                        'remainingLifeLimitLibrariesMutator',
                        response.data as RemainingLifeLimitLibrary[],
                    );
                }
            },
        );
    },
    async upsertRemainingLifeLimitLibrary(
        { dispatch, commit }: any,
        payload: any,
    ) {
        await RemainingLifeLimitService.upsertRemainingLifeLimitLibrary(
            payload.library,
        ).then((response: AxiosResponse) => {
            if (
                hasValue(response, 'status') &&
                http2XX.test(response.status.toString())
            ) {
                const message: string = any(
                    propEq('id', payload.library.id),
                    state.remainingLifeLimitLibraries,
                )
                    ? 'Updated remaining life limit library'
                    : 'Added remaining life limit library';
                commit(
                    'addedOrUpdatedRemainingLifeLimitLibraryMutator',
                    payload.library,
                );
                commit(
                    'selectedRemainingLifeLimitLibraryMutator',
                    payload.library.id,
                );
                dispatch('addSuccessNotification', { message: message });
            }
        });
    },
    async getScenarioRemainingLifeLimits({ commit }: any, scenarioId: string) {
        await RemainingLifeLimitService.getScenarioRemainingLifeLimit(
            scenarioId,
        ).then((response: AxiosResponse) => {
            if (hasValue(response, 'data')) {
                commit(
                    'scenarioRemainingLifeLimitMutator',
                    response.data as RemainingLifeLimit[],
                );
            }
        });
    },
    async upsertScenarioRemainingLifeLimits(
        { dispatch, commit }: any,
        payload: any,
    ) {
        await RemainingLifeLimitService.upsertScenarioRemainingLifeLimits(
            payload.scenarioRemainingLifeLimits,
            payload.scenarioId,
        ).then((response: AxiosResponse) => {
            if (
                hasValue(response, 'status') &&
                http2XX.test(response.status.toString())
            ) {
                commit(
                    'scenarioRemainingLifeLimitMutator',
                    payload.scenarioRemainingLifeLimits,
                );
                dispatch('addSuccessNotification', {
                    message: 'Modified remaining life minits',
                });
            }
        });
    },
    async deleteRemainingLifeLimitLibrary(
        { dispatch, commit, state }: any,
        payload: any,
    ) {
        await RemainingLifeLimitService.deleteRemainingLifeLimitLibrary(
            payload.libraryId,
        ).then((response: AxiosResponse) => {
            if (
                hasValue(response, 'status') &&
                http2XX.test(response.status.toString())
            ) {
                commit(
                    'deletedRemainingLifeLimitLibraryMutator',
                    payload.libraryId,
                );
                dispatch('addSuccessNotification', {
                    message: 'Deleted remaining life limit library',
                });
            }
        });
    },
    async getIsSharedRemainingLifeLimitLibrary({ dispatch, commit }: any, payload: any) {
        await RemainingLifeLimitService.getIsSharedRemainingLifeLimitLibrary(payload.id).then(
            (response: AxiosResponse) => {
                if (
                hasValue(response, 'status') &&
                    http2XX.test(response.status.toString())
                ) {
                commit('IsSharedLibraryMutator', response.data as boolean);
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
