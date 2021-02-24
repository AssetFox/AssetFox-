import {CashFlowRuleLibrary, emptyCashFlowRuleLibrary} from '@/shared/models/iAM/cash-flow';
import {any, append, clone, find, findIndex, propEq, reject, update} from 'ramda';
import CashFlowService from '@/services/cash-flow.service';
import {AxiosResponse} from 'axios';
import {hasValue} from '@/shared/utils/has-value-util';
import {http2XX} from '@/shared/utils/http-utils';
import {getAppliedLibrary, hasAppliedLibrary, unapplyLibrary} from '@/shared/utils/library-utils';
import {PerformanceCurveLibrary} from '@/shared/models/iAM/performance';
import {getBlankGuid} from '@/shared/utils/uuid-utils';

const state = {
    cashFlowRuleLibraries: [] as CashFlowRuleLibrary[],
    selectedCashFlowRuleLibrary: clone(emptyCashFlowRuleLibrary) as CashFlowRuleLibrary
};

const mutations = {
    cashFlowRuleLibrariesMutator(state: any, libraries: CashFlowRuleLibrary[]) {
        state.cashFlowRuleLibraries = clone(libraries);
    },
    selectedCashFlowRuleLibraryMutator(state: any, libraryId: string) {
        if (any(propEq('id', libraryId), state.cashFlowRuleLibraries)) {
            state.selectedCashFlowRuleLibrary = find(propEq('id', libraryId), state.cashFlowRuleLibraries);
        } else {
            state.selectedCashFlowRuleLibrary = clone(emptyCashFlowRuleLibrary);
        }
    },
    addedOrUpdatedCashFlowRuleLibraryMutator(state: any, library: CashFlowRuleLibrary) {
        state.cashFlowRuleLibraries = any(propEq('id', library.id), state.cashFlowRuleLibraries)
            ? update(findIndex(propEq('id', library.id), state.cashFlowRuleLibraries),
                library, state.cashFlowRuleLibraries)
            : append(library, state.cashFlowRuleLibraries);
    },
    deletedCashFlowRuleLibraryMutator(state: any, deletedLibraryId: string) {
        if (any(propEq('id', deletedLibraryId), state.cashFlowRuleLibraries)) {
            state.cashFlowRuleLibraries = reject(
                (library: CashFlowRuleLibrary) => deletedLibraryId === library.id,
                state.cashFlowRuleLibraries
            );
        }
    }
};

const actions = {
    selectCashFlowRuleLibrary({commit}: any, payload: any) {
        commit('selectedCashFlowRuleLibraryMutator', payload.libraryId);
    },
    async getCashFlowRuleLibraries({commit}: any) {
        await CashFlowService.getCashFlowRuleLibraries()
            .then((response: AxiosResponse<any[]>) => {
                if (hasValue(response, 'data')) {
                    commit('cashFlowRuleLibrariesMutator', response.data as CashFlowRuleLibrary[]);
                }
            });
    },
    async addOrUpdateCashFlowRuleLibrary({dispatch, commit}: any, payload: any) {
        await CashFlowService.addOrUpdateCashFlowRuleLibrary(payload.library, payload.scenarioId)
            .then((response: AxiosResponse) => {
                if (hasValue(response, 'status') && http2XX.test(response.status.toString())) {
                    if (payload.scenarioId !== getBlankGuid() && hasAppliedLibrary(state.cashFlowRuleLibraries, payload.scenarioId)) {
                        const unAppliedLibrary: PerformanceCurveLibrary = unapplyLibrary(getAppliedLibrary(
                            state.cashFlowRuleLibraries, payload.scenarioId), payload.scenarioId);
                        commit('addedOrUpdatedCashFlowRuleLibraryMutator', unAppliedLibrary);
                    }

                    const library: CashFlowRuleLibrary = {
                        ...payload.library,
                        appliedScenarioIds: payload.scenarioId !== getBlankGuid() &&
                        payload.library.appliedScenarioIds.indexOf(payload.scenarioId) === -1
                            ? append(payload.scenarioId, payload.library.appliedScenarioIds)
                            : payload.library.appliedScenarioIds
                    };

                    const message: string = any(propEq('id', library.id), state.cashFlowRuleLibraries)
                        ? 'Updated cash flow rule library'
                        : 'Added cash flow rule library';
                    commit('addedOrUpdatedCashFlowRuleLibraryMutator', library);
                    commit('selectedCashFlowRuleLibraryMutator', library.id);
                    dispatch('setSuccessMessage', {message: message});
                }
            });
    },
    async deleteCashFlowRuleLibrary({dispatch, commit, state}: any, payload: any) {
        await CashFlowService.deleteCashFlowRuleLibrary(payload.libraryId)
            .then((response: AxiosResponse) => {
                if (hasValue(response, 'status') && http2XX.test(response.status.toString())) {
                    commit('deletedCashFlowRuleLibraryMutator', payload.libraryId);
                    dispatch('setSuccessMessage', {message: 'Deleted cash flow rule library'});
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
