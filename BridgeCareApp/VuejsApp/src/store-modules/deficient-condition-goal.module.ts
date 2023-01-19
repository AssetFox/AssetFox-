import {
    DeficientConditionGoal,
    DeficientConditionGoalLibrary,
    emptyDeficientConditionGoalLibrary,
} from '@/shared/models/iAM/deficient-condition-goal';
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
import DeficientConditionGoalService from '@/services/deficient-condition-goal.service';

const state = {
    deficientConditionGoalLibraries: [] as DeficientConditionGoalLibrary[],
    selectedDeficientConditionGoalLibrary: clone(
        emptyDeficientConditionGoalLibrary,
    ) as DeficientConditionGoalLibrary,
    scenarioDeficientConditionGoals: [] as DeficientConditionGoal[],
    hasPermittedAccess: false,
    isSharedLibrary: false,
};

const mutations = {
    deficientConditionGoalLibrariesMutator(
        state: any,
        libraries: DeficientConditionGoalLibrary[],
    ) {
        state.deficientConditionGoalLibraries = clone(libraries);
    },
    selectedDeficientConditionGoalLibraryMutator(
        state: any,
        libraryId: string,
    ) {
        if (
            any(propEq('id', libraryId), state.deficientConditionGoalLibraries)
        ) {
            state.selectedDeficientConditionGoalLibrary = find(
                propEq('id', libraryId),
                state.deficientConditionGoalLibraries,
            );
        } else {
            state.selectedDeficientConditionGoalLibrary = clone(
                emptyDeficientConditionGoalLibrary,
            );
        }
    },
    addedOrUpdatedDeficientConditionGoalLibraryMutator(
        state: any,
        library: DeficientConditionGoalLibrary,
    ) {
        state.deficientConditionGoalLibraries = any(
            propEq('id', library.id),
            state.deficientConditionGoalLibraries,
        )
            ? update(
                  findIndex(
                      propEq('id', library.id),
                      state.deficientConditionGoalLibraries,
                  ),
                  library,
                  state.deficientConditionGoalLibraries,
              )
            : append(library, state.deficientConditionGoalLibraries);
    },
    deletedDeficientConditionGoalLibraryMutator(
        state: any,
        deletedLibraryId: string,
    ) {
        if (
            any(
                propEq('id', deletedLibraryId),
                state.deficientConditionGoalLibraries,
            )
        ) {
            state.deficientConditionGoalLibraries = reject(
                (library: DeficientConditionGoalLibrary) =>
                    deletedLibraryId === library.id,
                state.deficientConditionGoalLibraries,
            );
        }
    },
    scenarioDeficientConditionGoalsMutator(
        state: any,
        deficientConditionGoals: DeficientConditionGoal[],
    ) {
        state.scenarioDeficientConditionGoals = clone(deficientConditionGoals);
    },
    PermittedAccessMutator(state: any, status: boolean) {
        state.hasPermittedAccess = status;
    },
    IsSharedLibraryMutator(state: any, status: boolean) {
        state.isSharedLibrary = status;
    }
};

const actions = {
    selectDeficientConditionGoalLibrary({ commit }: any, payload: any) {
        commit(
            'selectedDeficientConditionGoalLibraryMutator',
            payload.libraryId,
        );
    },
    async getDeficientConditionGoalLibraries({ commit }: any) {
        await DeficientConditionGoalService.getDeficientConditionGoalLibraries().then(
            (response: AxiosResponse<any[]>) => {
                if (hasValue(response, 'data')) {
                    commit(
                        'deficientConditionGoalLibrariesMutator',
                        response.data as DeficientConditionGoalLibrary[],
                    );
                }
            },
        );
    },
    async getScenarioDeficientConditionGoals(
        { commit }: any,
        scenarioId: string,
    ) {
        await DeficientConditionGoalService.getScenarioDeficientConditionGoals(
            scenarioId,
        ).then((response: AxiosResponse) => {
            if (hasValue(response, 'data')) {
                commit(
                    'scenarioDeficientConditionGoalsMutator',
                    response.data as DeficientConditionGoal[],
                );
            }
        });
    },
    async deleteDeficientConditionGoalLibrary(
        { dispatch, commit, state }: any,
        payload: any,
    ) {
        await DeficientConditionGoalService.deleteDeficientConditionGoalLibrary(
            payload.libraryId,
        ).then((response: AxiosResponse) => {
            if (
                hasValue(response, 'status') &&
                http2XX.test(response.status.toString())
            ) {
                commit(
                    'deletedDeficientConditionGoalLibraryMutator',
                    payload.libraryId,
                );
                dispatch('addSuccessNotification', {
                    message: 'Deleted target condition goal library',
                });
            }
        });
    },
    async getHasPermittedAccess({ commit }: any)
    {
        await DeficientConditionGoalService.getHasPermittedAccess()
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
    async getIsSharedDeficientConditionGoalLibrary({ dispatch, commit }: any, payload: any) {
        await DeficientConditionGoalService.getIsSharedDeficientConditionGoalLibrary(payload.id).then(
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
