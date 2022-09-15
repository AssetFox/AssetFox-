import { Budget, BudgetAmount, BudgetLibrary, Investment, InvestmentPlan } from "./investment";

export interface PagingPage<T>{
    items: T[];
    totalItems: number;
}

export interface InvestmentPagingPage{
    items: Budget[];
    totalItems: number;
    lastYear: number;
    investmentPlan: InvestmentPlan
}

export interface PagingRequest<T>{
    page: number;
    rowsPerPage: number;
    sortColumn: string;
    search: string;
    isDescending: boolean;
    pagingSync: PaginSync<T>;
}

export interface PaginSync<T>{
    libraryId: string | null;
    rowsForDeletion: string[];
    updateRows: T[];
    addedRows: T[];
}

export interface LibraryUpsertPagingRequest<T,Y>{
    library: T;
    pagingSync: PaginSync<Y>;
}

export interface InvestmentLibraryUpsertPagingRequestModel{
    library: BudgetLibrary;
    pagingSync: InvestmentPagingSyncModel;
}

export interface InvestmentPagingRequestModel{
    page: number;
    rowsPerPage: number;
    sortColumn: string;
    search: string;
    isDescending: boolean;
    pagingSync: InvestmentPagingSyncModel;
}

export interface InvestmentPagingSyncModel{
    Investment: InvestmentPlan;
    libraryId: string | null;
    budgetsForDeletion: string[];
    updatedBudgets: Budget[];
    addedBudgets: Budget[];
    deletionyears: number[];
    updatedBudgetAmounts: { [key: string]: BudgetAmount[]; }
    addedBudgetAmounts: { [key: string]: BudgetAmount[]; }
}