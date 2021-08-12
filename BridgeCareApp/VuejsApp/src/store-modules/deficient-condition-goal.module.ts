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
import { getBlankGuid } from '@/shared/utils/uuid-utils';
import {
    getAppliedLibrary,
    hasAppliedLibrary,
    unapplyLibrary,
} from '@/shared/utils/library-utils';
import DeficientConditionGoalService from '@/services/deficient-condition-goal.service';

const state = {
    deficientConditionGoalLibraries: [] as DeficientConditionGoalLibrary[],
    selectedDeficientConditionGoalLibrary: clone(
        emptyDeficientConditionGoalLibrary,
    ) as DeficientConditionGoalLibrary,
    scenarioDeficientConditionGoals: [] as DeficientConditionGoal[],
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
    async upsertDeficientConditionGoalLibrary(
        { dispatch, commit }: any,
        payload: any,
    ) {
        await DeficientConditionGoalService.upsertDeficientConditionGoalLibrary(
            payload.library,
        ).then((response: AxiosResponse) => {
            if (
                hasValue(response, 'status') &&
                http2XX.test(response.status.toString())
            ) {
                // if (payload.scenarioId !== getBlankGuid() && hasAppliedLibrary(state.deficientConditionGoalLibraries, payload.scenarioId)) {
                //     const unAppliedLibrary: DeficientConditionGoalLibrary = unapplyLibrary(getAppliedLibrary(
                //         state.deficientConditionGoalLibraries, payload.scenarioId), payload.scenarioId);
                //     commit('addedOrUpdatedDeficientConditionGoalLibraryMutator', unAppliedLibrary);
                // }

                // const library: DeficientConditionGoalLibrary = {
                //     ...payload.library,
                //     appliedScenarioIds: payload.scenarioId !== getBlankGuid() &&
                //     payload.library.appliedScenarioIds.indexOf(payload.scenarioId) === -1
                //         ? append(payload.scenarioId, payload.library.appliedScenarioIds)
                //         : payload.library.appliedScenarioIds
                // };

                const message: string = any(
                    propEq('id', payload.library.id),
                    state.deficientConditionGoalLibraries,
                )
                    ? 'Updated deficient condition goal library'
                    : 'Added deficient condition goal library';
                commit(
                    'addedOrUpdatedDeficientConditionGoalLibraryMutator',
                    payload.library,
                );
                commit(
                    'selectedDeficientConditionGoalLibraryMutator',
                    payload.library.id,
                );
                dispatch('setSuccessMessage', { message: message });
            }
        });
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
    async upsertScenarioDeficientConditionGoals(
        { dispatch, commit }: any,
        payload: any,
    ) {
        await DeficientConditionGoalService.upsertScenarioDeficientConditionGoals(
            payload.scenarioDeficientConditionGoals,
            payload.scenarioId,
        ).then((response: AxiosResponse) => {
            if (
                hasValue(response, 'status') &&
                http2XX.test(response.status.toString())
            ) {
                commit(
                    'scenarioDeficientConditionGoalsMutator',
                    payload.scenarioDeficientConditionGoals,
                );
                dispatch('setSuccessMessage', {
                    message: 'Modified deficient condition goals',
                });
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
                dispatch('setSuccessMessage', {
                    message: 'Deleted target condition goal library',
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
