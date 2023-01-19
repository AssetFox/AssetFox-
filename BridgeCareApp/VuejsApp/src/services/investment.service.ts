import { API, coreAxiosInstance } from '@/shared/utils/axios-instance';
import { AxiosPromise } from 'axios';
import { BudgetLibrary, BudgetLibraryUser, Investment, LibraryUser } from '@/shared/models/iAM/investment';
import { UserCriteriaFilter } from '@/shared/models/iAM/user-criteria-filter';
import { InvestmentLibraryUpsertPagingRequestModel, InvestmentPagingRequestModel, InvestmentPagingSyncModel } from '@/shared/models/iAM/paging';

export default class InvestmentService {
    static getInvestment(scenarioId: string): AxiosPromise {
        return coreAxiosInstance.get(
            `${API.Investment}/GetInvestment/${scenarioId}`,
        );
    }

    static upsertInvestment(
        data: InvestmentPagingSyncModel,
        scenarioId: string,
    ): AxiosPromise {
        return coreAxiosInstance.post(
            `${API.Investment}/UpsertInvestment/${scenarioId}`,
            data,
        );
    }

    static getBudgetLibraries(): AxiosPromise {
        return coreAxiosInstance.get(`${API.Investment}/GetBudgetLibraries/`);
    }

    static getScenarioInvestmentPage(scenarioId: string, data: InvestmentPagingRequestModel): AxiosPromise {
        return coreAxiosInstance.post(`${API.Investment}/GetScenarioInvestmentPage/${scenarioId}`, data)
    }

    static getLibraryInvestmentPage(scenarioId: string, data: InvestmentPagingRequestModel): AxiosPromise {
        return coreAxiosInstance.post(`${API.Investment}/GetLibraryInvestmentPage/${scenarioId}`, data)
    }

    static upsertBudgetLibrary(data: InvestmentLibraryUpsertPagingRequestModel): AxiosPromise {
        return coreAxiosInstance.post(
            `${API.Investment}/UpsertBudgetLibrary`,
            data,
        );
    }

    static deleteBudgetLibrary(libraryId: string): AxiosPromise {
        return coreAxiosInstance.delete(
            `${API.Investment}/DeleteBudgetLibrary/${libraryId}`,
        );
    }

    static getBudgetLibraryUsers(libraryId: string): AxiosPromise {
        return coreAxiosInstance.get(`${API.Investment}/GetBudgetLibraryUsers/${libraryId}`)
    }

    static upsertOrDeleteBudgetLibraryUsers(libraryId: string, proposedUsers: LibraryUser[]): AxiosPromise {
        return coreAxiosInstance.post(
            `${API.Investment}/UpsertOrDeleteBudgetLibraryUsers/${libraryId}`
            , proposedUsers
        );
    }

    static getScenarioSimpleBudgetDetails(scenarioId: string) {
        return coreAxiosInstance.get(
            `${API.Investment}/GetScenarioSimpleBudgetDetails/${scenarioId}`,
        );
    }

    static GetInvestmentPlan(scenarioId: string) {
        return coreAxiosInstance.get(
            `${API.Investment}/GetInvestmentPlan/${scenarioId}`,
        );
    }

    static getScenarioBudgetYears(scenarioId: string){
        return coreAxiosInstance.get(
            `${API.Investment}/GetScenarioBudgetYears/${scenarioId}`,
        );
    }

    static exportInvestmentBudgets(
        id: string,
        forScenario: boolean = false,
    ): AxiosPromise {
        return forScenario
            ? coreAxiosInstance.get(
                  `${API.Investment}/ExportScenarioInvestmentBudgetsExcelFile/${id}`,
              )
            : coreAxiosInstance.get(
                  `${API.Investment}/ExportLibraryInvestmentBudgetsExcelFile/${id}`,
              );
    }

    static importInvestmentBudgets(
        file: File,
        overwriteBudgets: boolean,
        id: string,
        forScenario: boolean,
        currentUserCriteriaFilter: UserCriteriaFilter,
    ) {
        let formData = new FormData();

        formData.append('file', file);
        formData.append('overwriteBudgets', overwriteBudgets ? '1' : '0');
        formData.append(forScenario ? 'simulationId' : 'libraryId', id);
        formData.append(
            'currentUserCriteriaFilter',
            JSON.stringify(currentUserCriteriaFilter),
        );

        return forScenario
            ? coreAxiosInstance.post(
                  `${API.Investment}/ImportScenarioInvestmentBudgetsExcelFile`,
                  formData,
                  { headers: { 'Content-Type': 'multipart/form-data' } },
              )
            : coreAxiosInstance.post(
                  `${API.Investment}/ImportLibraryInvestmentBudgetsExcelFile`,
                  formData,
                  { headers: { 'Content-Type': 'multipart/form-data' } },
              );
    }

    static downloadInvestmentBudgetsTemplate(
    ): AxiosPromise {
            return coreAxiosInstance.get(               
                  `${API.Investment}/DownloadInvestmentBudgetsTemplate`,
              );
    }

    static getHasPermittedAccess(): AxiosPromise {
        return coreAxiosInstance.get(
            `${API.Investment}/GetHasPermittedAccess`,
        );
    }
}
