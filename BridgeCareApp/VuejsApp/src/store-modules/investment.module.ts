import {
    Budget,
    BudgetLibrary,
    emptyBudgetLibrary,
    emptyInvestmentPlan,
    Investment,
    InvestmentPlan,
    SimpleBudgetDetail
} from '@/shared/models/iAM/investment';
import InvestmentService from '@/services/investment.service';
import {any, append, clone, find, findIndex, propEq, reject, update} from 'ramda';
import {AxiosResponse} from 'axios';
import {hasValue} from '@/shared/utils/has-value-util';
import {http2XX} from '@/shared/utils/http-utils';
import {getBlankGuid} from '@/shared/utils/uuid-utils';
import {getAppliedLibrary, hasAppliedLibrary, unapplyLibrary} from '@/shared/utils/library-utils';
import {CriterionLibrary} from '@/shared/models/iAM/criteria';

const state = {
    budgetLibraries: [] as BudgetLibrary[],
    selectedBudgetLibrary: clone(emptyBudgetLibrary) as BudgetLibrary,
    investmentPlan: clone(emptyInvestmentPlan) as InvestmentPlan,
    scenarioSimpleBudgetDetails: [] as SimpleBudgetDetail[]
};

const mutations = {
    budgetLibrariesMutator(state: any, libraries: BudgetLibrary[]) {
        state.budgetLibraries = clone(libraries);
    },
    selectedBudgetLibraryMutator(state: any, libraryId: string) {
        if (any(propEq('id', libraryId), state.budgetLibraries)) {
            state.selectedBudgetLibrary = find(propEq('id', libraryId), state.budgetLibraries);
        } else {
            state.selectedBudgetLibrary = clone(emptyBudgetLibrary);
        }
    },
    addedOrUpdatedBudgetLibraryMutator(state: any, library: BudgetLibrary) {
        state.budgetLibraries = any(propEq('id', library.id), state.budgetLibraries)
            ? update(findIndex(propEq('id', library.id), state.budgetLibraries),
                library, state.budgetLibraries)
            : append(library, state.budgetLibraries);
    },
    deletedBudgetLibraryMutator(state: any, deletedLibraryId: string) {
        if (any(propEq('id', deletedLibraryId), state.budgetLibraries)) {
            state.budgetLibraries = reject(
                (library: BudgetLibrary) => deletedLibraryId === library.id,
                state.budgetLibraries
            );
        }
    },
    investmentPlanMutator(state: any, investmentPlan: InvestmentPlan) {
        state.investmentPlan = clone(investmentPlan);
    },
    scenarioSimpleBudgetDetailsMutator(state: any, simpleBudgetDetails: SimpleBudgetDetail[]) {
        state.scenarioSimpleBudgetDetails = clone(simpleBudgetDetails);
    },
    updatedBudgetsCriterionLibrariesMutator(state: any, criterionLibrary: CriterionLibrary) {
        state.budgetLibraries = state.budgetLibraries.map((library: BudgetLibrary) => ({
            ...library,
            budgets: library.budgets.map((budget: Budget) => ({
                ...budget,
                criterionLibrary: budget.criterionLibrary.id == criterionLibrary.id
                    ? clone(criterionLibrary)
                    : budget.criterionLibrary
            }))
        }));
    }
};

const actions = {
    selectBudgetLibrary({commit}: any, payload: any) {
        commit('selectedBudgetLibraryMutator', payload.libraryId);
    },
    updateBudgetsCriterionLibraries({commit}: any, payload: any) {
        commit('updatedBudgetsCriterionLibrariesMutator', payload.criterionLibrary);
    },
    async getInvestment({commit}: any, payload: any) {
        await InvestmentService.getInvestment(payload.scenarioId)
            .then((response: AxiosResponse) => {
                if (hasValue(response, 'data')) {
                    const investmentData: Investment = response.data as Investment;
                    commit('budgetLibrariesMutator', investmentData.budgetLibraries);
                    commit('investmentPlanMutator', investmentData.investmentPlan);
                }
            });
    },
    async upsertInvestment({dispatch, commit}: any, payload: any) {
        await InvestmentService.upsertInvestment(payload.library, payload.investmentPlan, payload.scenarioId)
            .then((response: AxiosResponse) => {
                if (hasValue(response, 'status') && http2XX.test(response.status.toString())) {
                    if (payload.scenarioId !== getBlankGuid() && hasAppliedLibrary(state.budgetLibraries, payload.scenarioId)) {
                        const unAppliedLibrary: BudgetLibrary = unapplyLibrary(getAppliedLibrary(
                            state.budgetLibraries, payload.scenarioId), payload.scenarioId);
                        commit('addedOrUpdatedBudgetLibraryMutator', unAppliedLibrary);
                    }

                    const library: BudgetLibrary = {
                        ...payload.library,
                        appliedScenarioIds: payload.scenarioId !== getBlankGuid() &&
                        payload.library.appliedScenarioIds.indexOf(payload.scenarioId) === -1
                            ? append(payload.scenarioId, payload.library.appliedScenarioIds)
                            : payload.library.appliedScenarioIds
                    };

                    commit('addedOrUpdatedBudgetLibraryMutator', library);
                    commit('selectedBudgetLibraryMutator', library.id);
                    commit('investmentPlanMutator', payload.investmentPlan);

                    dispatch('setSuccessMessage', {message: 'Upsertted investment data'});
                }
            });
    },
    async deleteBudgetLibrary({dispatch, commit, state}: any, payload: any) {
        await InvestmentService.deleteBudgetLibrary(payload.libraryId)
            .then((response: AxiosResponse) => {
                if (hasValue(response, 'status') && http2XX.test(response.status.toString())) {
                    commit('deletedBudgetLibraryMutator', payload.libraryId);
                    dispatch('setSuccessMessage', {message: 'Deleted budget library'});
                }
            });
    },
    async getScenarioSimpleBudgetDetails({commit}: any, payload: any) {
        await InvestmentService.getScenarioSimpleBudgetDetails(payload.scenarioId)
            .then((response: AxiosResponse<any[]>) => {
                if (hasValue(response, 'data')) {
                    commit('scenarioSimpleBudgetDetailsMutator', response.data as SimpleBudgetDetail[]);
                }
            });
    },
};

const getters = {};

export default {
    state,
    getters,
    actions,
    mutations
};
