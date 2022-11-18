import {
    Budget,
    BudgetLibrary,
    emptyBudgetLibrary,
    emptyInvestmentPlan,
    Investment,
    InvestmentPlan,
    LibraryInvestmentBudgetFileImport,
    ScenarioInvestmentBudgetFileImport,
    SimpleBudgetDetail,
} from '@/shared/models/iAM/investment';
import InvestmentService from '@/services/investment.service';
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
import { CriterionLibrary } from '@/shared/models/iAM/criteria';

const state = {
    budgetLibraries: [] as BudgetLibrary[],
    selectedBudgetLibrary: clone(emptyBudgetLibrary) as BudgetLibrary,
    investmentPlan: clone(emptyInvestmentPlan) as InvestmentPlan,
    scenarioSimpleBudgetDetails: [] as SimpleBudgetDetail[],
    scenarioBudgets: [] as Budget[],
    isSuccessfulImport: false,
    hasPermittedAccess: false,
};

const mutations = {
    budgetLibrariesMutator(state: any, libraries: BudgetLibrary[]) {
        state.budgetLibraries = clone(libraries);
    },
    selectedBudgetLibraryMutator(state: any, libraryId: string) {
        if (any(propEq('id', libraryId), state.budgetLibraries)) {
            state.selectedBudgetLibrary = find(
                propEq('id', libraryId),
                state.budgetLibraries,
            );
        } else {
            state.selectedBudgetLibrary = clone(emptyBudgetLibrary);
        }
    },
    budgetLibraryMutator(state: any, library: BudgetLibrary) {
        state.budgetLibraries = any(
            propEq('id', library.id),
            state.budgetLibraries,
        )
            ? update(
                  findIndex(propEq('id', library.id), state.budgetLibraries),
                  library,
                  state.budgetLibraries,
              )
            : append(library, state.budgetLibraries);
    },
    investmentPlanMutator(state: any, investmentPlan: InvestmentPlan) {
        state.investmentPlan = clone(investmentPlan);
    },
    scenarioSimpleBudgetDetailsMutator(
        state: any,
        simpleBudgetDetails: SimpleBudgetDetail[],
    ) {
        state.scenarioSimpleBudgetDetails = clone(simpleBudgetDetails);
    },
    scenarioBudgetsMutator(state: any, budgets: Budget[]) {
        state.scenarioBudgets = clone(budgets);
    },
    isSuccessfulImportMutator(State: any, isSuccessful: boolean){
        state.isSuccessfulImport = isSuccessful;
    },
    PermittedAccessMutator(state: any, status: boolean) {
        state.hasPermittedAccess = status;
    },
};

const actions = {
    selectBudgetLibrary({ commit }: any, libraryId: string) {
        commit('selectedBudgetLibraryMutator', libraryId);
    },
    async getInvestment({ commit }: any, scenarioId: string) {
        await InvestmentService.getInvestment(scenarioId).then(
            (response: AxiosResponse) => {
                if (hasValue(response, 'data')) {
                    const investmentData: Investment = response.data as Investment;
                    commit(
                        'scenarioBudgetsMutator',
                        investmentData.scenarioBudgets,
                    );
                    commit(
                        'investmentPlanMutator',
                        investmentData.investmentPlan,
                    );
                }
            },
        );
    },
    async upsertInvestment({ dispatch, commit }: any, payload: any) {
        await InvestmentService.upsertInvestment(
            payload.investment,
            payload.scenarioId,
        ).then((response: AxiosResponse) => {
            if (
                hasValue(response, 'status') &&
                http2XX.test(response.status.toString())
            ) {
                commit(
                    'scenarioBudgetsMutator',
                    payload.investment.scenarioBudgets,
                );

                commit(
                    'investmentPlanMutator',
                    payload.investment.investmentPlan,
                );

                dispatch('addSuccessNotification', {
                    message: 'Modified investment',
                });
            }
        });
    },
    async getBudgetLibraries({ commit }: any) {
        await InvestmentService.getBudgetLibraries().then(
            (response: AxiosResponse) => {
                if (hasValue(response, 'data')) {
                    commit(
                        'budgetLibrariesMutator',
                        response.data as BudgetLibrary[],
                    );
                }
            },
        );
    },
    async deleteBudgetLibrary(
        { dispatch, commit, state }: any,
        libraryId: string,
    ) {
        await InvestmentService.deleteBudgetLibrary(libraryId).then(
            (response: AxiosResponse) => {
                if (
                    hasValue(response, 'status') &&
                    http2XX.test(response.status.toString())
                ) {
                    const budgetLibraries: BudgetLibrary[] = reject(
                        propEq('id', libraryId),
                        state.budgetLibraries,
                    ) as BudgetLibrary[];
                    commit('budgetLibrariesMutator', budgetLibraries);

                    dispatch('addSuccessNotification', {
                        message: 'Deleted budget library',
                    });
                }
            },
        );
    },
    async getScenarioSimpleBudgetDetails({ commit }: any, payload: any) {
        await InvestmentService.getScenarioSimpleBudgetDetails(
            payload.scenarioId,
        ).then((response: AxiosResponse<any[]>) => {
            if (hasValue(response, 'data')) {
                commit(
                    'scenarioSimpleBudgetDetailsMutator',
                    response.data as SimpleBudgetDetail[],
                );
            }
        });
    },
    async getInvestmentPlan({ commit }: any, payload: any) {
        await InvestmentService.GetInvestmentPlan(
            payload.scenarioId,
        ).then((response: AxiosResponse<any>) => {
            if (hasValue(response, 'data')) {
                commit(
                    'investmentPlanMutator',
                    response.data as InvestmentPlan,
                );
            }
        });
    },
    async importScenarioInvestmentBudgetsFile(
        { commit, dispatch }: any,
        payload: any,
    ) {
        await InvestmentService.importInvestmentBudgets(
            payload.file,
            payload.overwriteBudgets,
            payload.id,
            true,
            payload.currentUserCriteriaFilter,
        ).then((response: AxiosResponse) => {
            if (hasValue(response, 'data')) {
                const budgets: Budget[] = response.data as Budget[];
                commit('scenarioBudgetsMutator', budgets);
                commit('isSuccessfulImportMutator', true);
                dispatch('addSuccessNotification', {
                    message: 'Investment budgets file imported',
                });
            }
            else {
                commit('isSuccessfulImportMutator', false);
            }
        });
    },
    async importLibraryInvestmentBudgetsFile(
        { commit, dispatch }: any,
        payload: any,
    ) {
        await InvestmentService.importInvestmentBudgets(
            payload.file,
            payload.overwriteBudgets,
            payload.id,
            false,
            payload.currentUserCriteriaFilter,
        ).then((response: AxiosResponse) => {
            if (hasValue(response, 'data')) {
                const library: BudgetLibrary = response.data as BudgetLibrary;
                commit('budgetLibraryMutator', library);
                commit('selectedBudgetLibraryMutator', library.id);
                commit('isSuccessfulImportMutator', true);
                dispatch('addSuccessNotification', {
                    message: 'Investment budgets file imported',
                });
            }
            else {
                commit('isSuccessfulImportMutator', false);
            }
        });
    },
    async getHasPermittedAccess({ commit }: any)
    {
        await InvestmentService.getHasPermittedAccess()
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
