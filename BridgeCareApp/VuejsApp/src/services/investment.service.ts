import { API, coreAxiosInstance } from '@/shared/utils/axios-instance';
import { AxiosPromise } from 'axios';
import { BudgetLibrary, InvestmentPlan } from '@/shared/models/iAM/investment';

export default class InvestmentService {
    static getInvestment(scenarioId: string): AxiosPromise {
        return coreAxiosInstance.get(
            `${API.Investment}/GetInvestment/${scenarioId}`,
        );
    }

    static upsertInvestment(
        budgetLibrary: BudgetLibrary,
        investmentPlan: InvestmentPlan,
        scenarioId: string,
    ): AxiosPromise {
        return coreAxiosInstance.post(
            `${API.Investment}/UpsertInvestment/${scenarioId}`,
            {
                budgetLibrary: budgetLibrary,
                investmentPlan: investmentPlan,
            },
        );
    }

    static deleteBudgetLibrary(libraryId: string): AxiosPromise {
        return coreAxiosInstance.delete(
            `${API.Investment}/DeleteBudgetLibrary/${libraryId}`,
        );
    }

    static getScenarioSimpleBudgetDetails(scenarioId: string) {
        return coreAxiosInstance.get(
            `${API.Investment}/GetScenarioSimpleBudgetDetails/${scenarioId}`,
        );
    }

    static exportInvestmentBudgets(
        libraryId: string,
        scenarioId: string,
    ): AxiosPromise {
        return coreAxiosInstance.get(
            `${API.Investment}/ExportInvestmentBudgetsExcelFile/${libraryId}/${scenarioId}`,
        );
    }

    static importInvestmentBudgets(
        file: File,
        overwriteBudgets: boolean,
        libraryId: string,
        scenarioId: string,
    ) {
        let formData = new FormData();

        formData.append('file', file);
        formData.append('overwriteBudgets', overwriteBudgets ? '1' : '0');
        formData.append('libraryId', libraryId);
        formData.append('simulationId', scenarioId);

        return coreAxiosInstance.post(
            `${API.Investment}/ImportInvestmentBudgetsExcelFile`,
            formData,
            { headers: { 'Content-Type': 'multipart/form-data' } },
        );
    }
}
