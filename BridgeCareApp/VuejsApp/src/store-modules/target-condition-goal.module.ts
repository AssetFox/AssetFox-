import {
    emptyTargetConditionGoalLibrary,
    TargetConditionGoal,
    TargetConditionGoalLibrary,
} from '@/shared/models/iAM/target-condition-goal';
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
import TargetConditionGoalService from '@/services/target-condition-goal.service';
import { AxiosResponse } from 'axios';
import { hasValue } from '@/shared/utils/has-value-util';
import { http2XX } from '@/shared/utils/http-utils';
import { getBlankGuid } from '@/shared/utils/uuid-utils';
import {
    getAppliedLibrary,
    hasAppliedLibrary,
    unapplyLibrary,
} from '@/shared/utils/library-utils';

const state = {
    targetConditionGoalLibraries: [] as TargetConditionGoalLibrary[],
    selectedTargetConditionGoalLibrary: clone(
        emptyTargetConditionGoalLibrary,
    ) as TargetConditionGoalLibrary,
    scenarioTargetConditionGoals: [] as TargetConditionGoal[],
};

const mutations = {
    targetConditionGoalLibrariesMutator(
        state: any,
        libraries: TargetConditionGoalLibrary[],
    ) {
        state.targetConditionGoalLibraries = clone(libraries);
    },
    selectedTargetConditionGoalLibraryMutator(state: any, libraryId: string) {
        if (any(propEq('id', libraryId), state.targetConditionGoalLibraries)) {
            state.selectedTargetConditionGoalLibrary = find(
                propEq('id', libraryId),
                state.targetConditionGoalLibraries,
            );
        } else {
            state.selectedTargetConditionGoalLibrary = clone(
                emptyTargetConditionGoalLibrary,
            );
        }
    },
    addedOrUpdatedTargetConditionGoalLibraryMutator(
        state: any,
        library: TargetConditionGoalLibrary,
    ) {
        state.targetConditionGoalLibraries = any(
            propEq('id', library.id),
            state.targetConditionGoalLibraries,
        )
            ? update(
                  findIndex(
                      propEq('id', library.id),
                      state.targetConditionGoalLibraries,
                  ),
                  library,
                  state.targetConditionGoalLibraries,
              )
            : append(library, state.targetConditionGoalLibraries);
    },
    deletedTargetConditionGoalLibraryMutator(
        state: any,
        deletedLibraryId: string,
    ) {
        if (
            any(
                propEq('id', deletedLibraryId),
                state.targetConditionGoalLibraries,
            )
        ) {
            state.targetConditionGoalLibraries = reject(
                (library: TargetConditionGoalLibrary) =>
                    deletedLibraryId === library.id,
                state.targetConditionGoalLibraries,
            );
        }
    },
    scenarioTargetConditionGoalsMutator(
        state: any,
        targetConditionGoals: TargetConditionGoal[],
    ) {
        state.scenarioTargetConditionGoals = clone(targetConditionGoals);
    },
};

const actions = {
    selectTargetConditionGoalLibrary({ commit }: any, payload: any) {
        commit('selectedTargetConditionGoalLibraryMutator', payload.libraryId);
    },
    async getTargetConditionGoalLibraries({ commit }: any) {
        await TargetConditionGoalService.getTargetConditionGoalLibraries().then(
            (response: AxiosResponse<any[]>) => {
                if (hasValue(response, 'data')) {
                    commit(
                        'targetConditionGoalLibrariesMutator',
                        response.data as TargetConditionGoalLibrary[],
                    );
                }
            },
        );
    },
    async upsertTargetConditionGoalLibrary(
        { dispatch, commit }: any,
        payload: any,
    ) {
        await TargetConditionGoalService.upsertTargetConditionGoalLibrary(
            payload.library,
            payload.scenarioId,
        ).then((response: AxiosResponse) => {
            if (
                hasValue(response, 'status') &&
                http2XX.test(response.status.toString())
            ) {
                // if (
                //     payload.scenarioId !== getBlankGuid() &&
                //     hasAppliedLibrary(
                //         state.targetConditionGoalLibraries,
                //         payload.scenarioId,
                //     )
                // ) {
                //     const unAppliedLibrary: TargetConditionGoalLibrary = unapplyLibrary(
                //         getAppliedLibrary(
                //             state.targetConditionGoalLibraries,
                //             payload.scenarioId,
                //         ),
                //         payload.scenarioId,
                //     );
                //     commit(
                //         'addedOrUpdatedTargetConditionGoalLibraryMutator',
                //         unAppliedLibrary,
                //     );
                // }

                // const library: TargetConditionGoalLibrary = {
                //     ...payload.library,
                //     appliedScenarioIds:
                //         payload.scenarioId !== getBlankGuid() &&
                //         payload.library.appliedScenarioIds.indexOf(
                //             payload.scenarioId,
                //         ) === -1
                //             ? append(
                //                   payload.scenarioId,
                //                   payload.library.appliedScenarioIds,
                //               )
                //             : payload.library.appliedScenarioIds,
                // };

                const message: string = any(
                    propEq('id', payload.library.id),
                    state.targetConditionGoalLibraries,
                )
                    ? 'Updated target condition goal library'
                    : 'Added target condition goal library';
                commit(
                    'addedOrUpdatedTargetConditionGoalLibraryMutator',
                    payload.library,
                );
                commit('selectedTargetConditionGoalLibraryMutator', payload.library.id);
                dispatch('setSuccessMessage', { message: message });
            }
        });
    },
    async getScenarioTargetConditionGoals(
        {commit} : any,
        scenarioId: string,
    ){
        await TargetConditionGoalService.getScenarioTargetConditionGoals(scenarioId).then(
            (response: AxiosResponse) => {
                if(hasValue(response, 'data')){
                    commit(
                        'scenarioTargetConditionGoalsMutator',
                        response.data as TargetConditionGoal[],
                    )
                }
            }
        )
    },
    async deleteTargetConditionGoalLibrary(
        { dispatch, commit, state }: any,
        payload: any,
    ) {
        await TargetConditionGoalService.deleteTargetConditionGoalLibrary(
            payload.libraryId,
        ).then((response: AxiosResponse) => {
            if (
                hasValue(response, 'status') &&
                http2XX.test(response.status.toString())
            ) {
                commit(
                    'deletedTargetConditionGoalLibraryMutator',
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
