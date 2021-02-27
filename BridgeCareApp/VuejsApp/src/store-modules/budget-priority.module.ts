import {BudgetPriorityLibrary, emptyBudgetPriorityLibrary} from '@/shared/models/iAM/budget-priority';
import {any, append, clone, find, findIndex, propEq, reject, update} from 'ramda';
import {AxiosResponse} from 'axios';
import {hasValue} from '@/shared/utils/has-value-util';
import {http2XX} from '@/shared/utils/http-utils';
import {getBlankGuid} from '@/shared/utils/uuid-utils';
import {getAppliedLibrary, hasAppliedLibrary, unapplyLibrary} from '@/shared/utils/library-utils';
import BudgetPriorityService from '@/services/budget-priority.service';

const state = {
    budgetPriorityLibraries: [] as BudgetPriorityLibrary[],
    selectedBudgetPriorityLibrary: clone(emptyBudgetPriorityLibrary) as BudgetPriorityLibrary
};

const mutations = {
    budgetPriorityLibrariesMutator(state: any, libraries: BudgetPriorityLibrary[]) {
        state.budgetPriorityLibraries = clone(libraries);
    },
    selectedBudgetPriorityLibraryMutator(state: any, libraryId: string) {
        if (any(propEq('id', libraryId), state.budgetPriorityLibraries)) {
            state.selectedBudgetPriorityLibrary = find(propEq('id', libraryId), state.budgetPriorityLibraries);
        } else {
            state.selectedBudgetPriorityLibrary = clone(emptyBudgetPriorityLibrary);
        }
    },
    addedOrUpdatedBudgetPriorityLibraryMutator(state: any, library: BudgetPriorityLibrary) {
        state.budgetPriorityLibraries = any(propEq('id', library.id), state.budgetPriorityLibraries)
            ? update(findIndex(propEq('id', library.id), state.budgetPriorityLibraries),
                library, state.budgetPriorityLibraries)
            : append(library, state.budgetPriorityLibraries);
    },
    deletedBudgetPriorityLibraryMutator(state: any, deletedLibraryId: string) {
        if (any(propEq('id', deletedLibraryId), state.budgetPriorityLibraries)) {
            state.budgetPriorityLibraries = reject(
                (library: BudgetPriorityLibrary) => deletedLibraryId === library.id,
                state.budgetPriorityLibraries
            );
        }
    }
};

const actions = {
    selectBudgetPriorityLibrary({commit}: any, payload: any) {
        commit('selectedBudgetPriorityLibraryMutator', payload.libraryId);
    },
    async getBudgetPriorityLibraries({commit}: any) {
        await BudgetPriorityService.getBudgetPriorityLibraries()
            .then((response: AxiosResponse<any[]>) => {
                if (hasValue(response, 'data')) {
                    commit('budgetPriorityLibrariesMutator', response.data as BudgetPriorityLibrary[]);
                }
            });
    },
    async addOrUpdateBudgetPriorityLibrary({dispatch, commit}: any, payload: any) {
        await BudgetPriorityService.addOrUpdateBudgetPriorityLibrary(payload.library, payload.scenarioId)
            .then((response: AxiosResponse) => {
                if (hasValue(response, 'status') && http2XX.test(response.status.toString())) {
                    if (payload.scenarioId !== getBlankGuid() && hasAppliedLibrary(state.budgetPriorityLibraries, payload.scenarioId)) {
                        const unAppliedLibrary: BudgetPriorityLibrary = unapplyLibrary(getAppliedLibrary(
                            state.budgetPriorityLibraries, payload.scenarioId), payload.scenarioId);
                        commit('addedOrUpdatedBudgetPriorityLibraryMutator', unAppliedLibrary);
                    }

                    const library: BudgetPriorityLibrary = {
                        ...payload.library,
                        appliedScenarioIds: payload.scenarioId !== getBlankGuid() &&
                        payload.library.appliedScenarioIds.indexOf(payload.scenarioId) === -1
                            ? append(payload.scenarioId, payload.library.appliedScenarioIds)
                            : payload.library.appliedScenarioIds
                    };

                    const message: string = any(propEq('id', library.id), state.budgetPriorityLibraries)
                        ? 'Updated budget priority library'
                        : 'Added budget priority library';
                    commit('addedOrUpdatedBudgetPriorityLibraryMutator', library);
                    commit('selectedBudgetPriorityLibraryMutator', library.id);
                    dispatch('setSuccessMessage', {message: message});
                }
            });
    },
    async deleteBudgetPriorityLibrary({dispatch, commit, state}: any, payload: any) {
        await BudgetPriorityService.deleteBudgetPriorityLibrary(payload.libraryId)
            .then((response: AxiosResponse) => {
                if (hasValue(response, 'status') && http2XX.test(response.status.toString())) {
                    commit('deletedBudgetPriorityLibraryMutator', payload.libraryId);
                    dispatch('setSuccessMessage', {message: 'Deleted budget priority library'});
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
