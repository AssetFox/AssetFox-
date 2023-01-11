import {
    CashFlowRule,
    CashFlowRuleLibrary,
    emptyCashFlowRuleLibrary,
} from '@/shared/models/iAM/cash-flow';
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
import CashFlowService from '@/services/cash-flow.service';
import { AxiosResponse } from 'axios';
import { hasValue } from '@/shared/utils/has-value-util';
import { http2XX } from '@/shared/utils/http-utils';

const state = {
    cashFlowRuleLibraries: [] as CashFlowRuleLibrary[],
    selectedCashFlowRuleLibrary: clone(
        emptyCashFlowRuleLibrary,
    ) as CashFlowRuleLibrary,
    scenarioCashFlowRules: [] as CashFlowRule[],
    hasPermittedAccess: false,
};

const mutations = {
    cashFlowRuleLibrariesMutator(state: any, libraries: CashFlowRuleLibrary[]) {
        state.cashFlowRuleLibraries = clone(libraries);
    },
    selectedCashFlowRuleLibraryMutator(state: any, libraryId: string) {
        if (any(propEq('id', libraryId), state.cashFlowRuleLibraries)) {
            state.selectedCashFlowRuleLibrary = find(
                propEq('id', libraryId),
                state.cashFlowRuleLibraries,
            );
        } else {
            state.selectedCashFlowRuleLibrary = clone(emptyCashFlowRuleLibrary);
        }
    },
    cashFlowRuleLibraryMutator(state: any, library: CashFlowRuleLibrary) {
        state.cashFlowRuleLibraries = any(
            propEq('id', library.id),
            state.cashFlowRuleLibraries,
        )
            ? update(
                  findIndex(
                      propEq('id', library.id),
                      state.cashFlowRuleLibraries,
                  ),
                  library,
                  state.cashFlowRuleLibraries,
              )
            : append(library, state.cashFlowRuleLibraries);
    },
    scenarioCashFlowRulesMutator(state: any, cashFlowRules: CashFlowRule[]) {
        state.scenarioCashFlowRules = clone(cashFlowRules);
    },
    PermittedAccessMutator(state: any, status: boolean) {
        state.hasPermittedAccess = status;
    },
};

const actions = {
    selectCashFlowRuleLibrary({ commit }: any, libraryId: string) {
        commit('selectedCashFlowRuleLibraryMutator', libraryId);
    },
    async getCashFlowRuleLibraries({ commit }: any) {
        await CashFlowService.getCashFlowRuleLibraries().then(
            (response: AxiosResponse<any[]>) => {
                if (hasValue(response, 'data')) {
                    commit(
                        'cashFlowRuleLibrariesMutator',
                        response.data as CashFlowRuleLibrary[],
                    );
                }
            },
        );
    },
    /*async upsertCashFlowRuleLibrary(
        { dispatch, commit }: any,
        library: CashFlowRuleLibrary,
    ) {
        await CashFlowService.upsertCashFlowRuleLibrary(library).then(
            (response: AxiosResponse) => {
                if (
                    hasValue(response, 'status') &&
                    http2XX.test(response.status.toString())
                ) {
                    const message: string = any(
                        propEq('id', library.id),
                        state.cashFlowRuleLibraries,
                    )
                        ? 'Updated cash flow rule library'
                        : 'Added cash flow rule library';

                    commit('cashFlowRuleLibraryMutator', library);
                    commit('selectedCashFlowRuleLibraryMutator', library.id);

                    dispatch('addSuccessNotification', { message: message });
                }
            },
        );
    },*/
    async deleteCashFlowRuleLibrary(
        { dispatch, commit, state }: any,
        libraryId: string,
    ) {
        await CashFlowService.deleteCashFlowRuleLibrary(libraryId).then(
            (response: AxiosResponse) => {
                if (
                    hasValue(response, 'status') &&
                    http2XX.test(response.status.toString())
                ) {
                    const cashFlowRuleLibraries: CashFlowRuleLibrary[] = reject(
                        propEq('id', libraryId),
                        state.cashFlowRuleLibraries,
                    );

                    commit(
                        'cashFlowRuleLibrariesMutator',
                        cashFlowRuleLibraries,
                    );

                    dispatch('addSuccessNotification', {
                        message: 'Deleted cash flow rule library',
                    });
                }
            },
        );
    },
    async getScenarioCashFlowRules({ commit }: any, scenarioId: string) {
        await CashFlowService.getScenarioCashFlowRules(scenarioId).then(
            (response: AxiosResponse) => {
                if (hasValue(response, 'data')) {
                    commit(
                        'scenarioCashFlowRulesMutator',
                        response.data as CashFlowRule[],
                    );
                }
            },
        );
    },
    async upsertScenarioCashFlowRules({ dispatch, commit }: any, payload: any) {
        await CashFlowService.upsertScenarioCashFlowRules(
            payload.scenarioCashFlowRules,
            payload.scenarioId,
        ).then((response: AxiosResponse) => {
            if (
                hasValue(response, 'status') &&
                http2XX.test(response.status.toString())
            ) {
                commit(
                    'scenarioCashFlowRulesMutator',
                    payload.scenarioCashFlowRules,
                );
                dispatch('addSuccessNotification', {
                    message: 'Modified scenario cash flow rules',
                });
            }
        });
    },
    async getHasPermittedAccess({ commit }: any)
    {
        await CashFlowService.getHasPermittedAccess()
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
