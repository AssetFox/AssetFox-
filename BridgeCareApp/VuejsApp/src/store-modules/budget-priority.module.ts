import {
    BudgetPriority,
    BudgetPriorityLibrary,
    emptyBudgetPriorityLibrary,
} from '@/shared/models/iAM/budget-priority';
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
import { AxiosResponse } from 'axios';
import { hasValue } from '@/shared/utils/has-value-util';
import { http2XX } from '@/shared/utils/http-utils';
import BudgetPriorityService from '@/services/budget-priority.service';

const state = {
    budgetPriorityLibraries: [] as BudgetPriorityLibrary[],
    selectedBudgetPriorityLibrary: clone(
        emptyBudgetPriorityLibrary,
    ) as BudgetPriorityLibrary,
    scenarioBudgetPriorities: [] as BudgetPriority[],
    scenarioBudgetPriorityLibrary: [] as BudgetPriorityLibrary[],
    hasPermittedAccess: false,
    libraryUsers: [] as LibraryUser[],
    isSharedLibrary: false
};

const mutations = {
    budgetPriorityLibrariesMutator(
        state: any,
        libraries: BudgetPriorityLibrary[],
    ) {
        state.budgetPriorityLibraries = clone(libraries);
    },
    scenarioBudgetPriorityLibraryMutator(
        start: any,
        libraries: BudgetPriorityLibrary[],
    ) {
        state.scenarioBudgetPriorityLibrary = clone(libraries);
    },
    selectedBudgetPriorityLibraryMutator(state: any, libraryId: string) {
        if (any(propEq('id', libraryId), state.budgetPriorityLibraries)) {
            state.selectedBudgetPriorityLibrary = find(
                propEq('id', libraryId),
                state.budgetPriorityLibraries,
            );
        } else {
            state.selectedBudgetPriorityLibrary = clone(
                emptyBudgetPriorityLibrary,
            );
        }
    },
    budgetPriorityLibraryMutator(state: any, library: BudgetPriorityLibrary) {
        state.budgetPriorityLibraries = any(
            propEq('id', library.id),
            state.budgetPriorityLibraries,
        )
            ? update(
                  findIndex(
                      propEq('id', library.id),
                      state.budgetPriorityLibraries,
                  ),
                  library,
                  state.budgetPriorityLibraries,
              )
            : append(library, state.budgetPriorityLibraries);
    },
    budgetPriorityLibraryUserMutator(
        state: any,
        libraries: LibraryUser[]
    ) {
        state.libraryUsers = clone(libraries);
    },
    scenarioBudgetPrioritiesMutator(
        state: any,
        budgetPriorities: BudgetPriority[],
    ) {
        state.scenarioBudgetPriorities = clone(budgetPriorities);
    },
    PermittedAccessMutator(state: any, status: boolean) {
        state.hasPermittedAccess = status;
    },
    IsSharedLibraryMutator(state: any, status: boolean) {
        state.isSharedLibrary = status;
    }
};

const actions = {
    selectBudgetPriorityLibrary({ commit }: any, payload: any) {
        commit('selectedBudgetPriorityLibraryMutator', payload.libraryId);
    },
    async getBudgetPriorityLibraries({ commit }: any) {
        await BudgetPriorityService.getBudgetPriorityLibraries().then(
            (response: AxiosResponse<any[]>) => {
                if (hasValue(response, 'data')) {
                    commit(
                        'budgetPriorityLibrariesMutator',
                        response.data as BudgetPriorityLibrary[],
                    );
                }
            },
        );
    },
    async deleteBudgetPriorityLibrary(
        { dispatch, commit, state }: any,
        libraryId: string,
    ) {
        await BudgetPriorityService.deleteBudgetPriorityLibrary(libraryId).then(
            (response: AxiosResponse) => {
                if (
                    hasValue(response, 'status') &&
                    http2XX.test(response.status.toString())
                ) {
                    const budgetPriorityLibraries: BudgetPriorityLibrary[] = reject(
                        propEq('id', libraryId),
                        state.budgetPriorityLibraries,
                    );

                    commit(
                        'budgetPriorityLibrariesMutator',
                        budgetPriorityLibraries,
                    );

                    dispatch('addSuccessNotification', {
                        message: 'Deleted budget priority library',
                    });
                }
            },
        );
    },
    async getScenarioBudgetPriorities({ commit }: any, scenarioId: string) {
        await BudgetPriorityService.getScenarioBudgetPriorities(
            scenarioId,
        ).then((response: AxiosResponse) => {
            if (hasValue(response, 'data')) {
                commit(
                    'scenarioBudgetPrioritiesMutator',
                    response.data as BudgetPriority[],
                );
            }
        });
    },
    async getHasPermittedAccess({ commit }: any)
    {
        await BudgetPriorityService.getHasPermittedAccess()
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
    async getBudgetPriorityLibraryUsers({ commit }: any, libraryId: string) {
        await BudgetPriorityService.GetBudgetPriorityLibraryUsers(libraryId)
        .then((response: AxiosResponse) => {
            if (
                hasValue(response, 'status') && http2XX.test(response.status.toString())
            ) {
                commit('budgetPriorityLibraryUserMutator', response.data);                
            }
        });
    },
    async getScenarioBudgetPriorityLibrary({ commit }: any, scenarioId: string) {
        await BudgetPriorityService.getScenarioBudgetPriorityLibrary(scenarioId)
        .then((response: AxiosResponse) => {
            if (
                hasValue(response, 'status') && http2XX.test(response.status.toString())
            ) {
                commit('scenarioBudgetPriorityLibraryMutator', response.data);                
            }
        });
    },
    async getIsSharedBudgetPriorityLibrary({ dispatch, commit }: any, payload: any) {
        await BudgetPriorityService.getIsSharedBudgetPriorityLibrary(payload.id).then(
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
