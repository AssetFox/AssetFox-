import {API, coreAxiosInstance} from '@/shared/utils/axios-instance';
import {AxiosPromise} from 'axios';
import {BudgetLibrary, InvestmentPlan} from '@/shared/models/iAM/investment';

export default class InvestmentService {
    static getInvestment(scenarioId: string): AxiosPromise {
        return coreAxiosInstance.get(`${API.InvestmentController}/GetInvestment/${scenarioId}`);
    }

    static addOrUpdateInvestment(budgetLibrary: BudgetLibrary, investmentPlan: InvestmentPlan, scenarioId: string): AxiosPromise {
        return coreAxiosInstance.post(`${API.InvestmentController}/AddOrUpdateInvestment/${scenarioId}`, {
            budgetLibrary: budgetLibrary,
            investmentPlan: investmentPlan
        });
    }

    static deleteBudgetLibrary(libraryId: string): AxiosPromise {
        return coreAxiosInstance.delete(`${API.InvestmentController}/DeleteBudgetLibrary/${libraryId}`);
    }

    static getScenarioSimpleBudgetDetails(scenarioId: string) {
        return coreAxiosInstance.get(`${API.InvestmentController}/GetScenarioSimpleBudgetDetails/${scenarioId}`);
    }
}
