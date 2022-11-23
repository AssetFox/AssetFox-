import { CalculatedAttribute, CalculatedAttributeLibrary, CriterionAndEquationSet, Timing } from "./calculated-attribute";
import { Budget, BudgetAmount, BudgetLibrary, Investment, InvestmentPlan } from "./investment";

//abstract
export abstract class BaseLibraryUpsertPagingRequest<T>{
    public library: T;
    public isNewLibrary: boolean;
}

export abstract class BasePagingRequest{
    public page: number;
    public rowsPerPage: number;
    public isDescending: boolean;
    public sortColumn: string;
    public search: string;
}

//General
export interface LibraryUpsertPagingRequest<T,Y> extends BaseLibraryUpsertPagingRequest<T>{
    pagingSync: PaginSync<Y>;
}

export interface PagingRequest<T> extends BasePagingRequest{
    pagingSync: PaginSync<T>;
}

export interface PagingPage<T>{
    items: T[];
    totalItems: number;
}

export interface PaginSync<T>{
    libraryId: string | null;
    rowsForDeletion: string[];
    updateRows: T[];
    addedRows: T[];
}

//Investment
export interface InvestmentLibraryUpsertPagingRequestModel extends BaseLibraryUpsertPagingRequest<BudgetLibrary>{
    pagingSync: InvestmentPagingSyncModel;    
}

export interface InvestmentPagingPage extends PagingPage<Budget>{
    lastYear: number;
    investmentPlan: InvestmentPlan
}

export interface InvestmentPagingRequestModel extends BasePagingRequest{
    pagingSync: InvestmentPagingSyncModel;
}

export interface InvestmentPagingSyncModel{
    Investment: InvestmentPlan | null;
    libraryId: string | null;
    budgetsForDeletion: string[];
    updatedBudgets: Budget[];
    addedBudgets: Budget[];
    deletionyears: number[];
    updatedBudgetAmounts: { [key: string]: BudgetAmount[]; }
    addedBudgetAmounts: { [key: string]: BudgetAmount[]; }
    firstYearAnalysisBudgetShift: number;
}

//CalculatedAttributes
export interface CalculatedAttributeLibraryUpsertPagingRequestModel extends BaseLibraryUpsertPagingRequest<CalculatedAttributeLibrary>{
    syncModel: CalculatedAttributePagingSyncModel;
}

export interface CalculatedAttributePagingRequestModel extends BasePagingRequest{
    attributeId: string;
    syncModel: CalculatedAttributePagingSyncModel;
}

export interface CalculatedAttributePagingSyncModel{
    libraryId: string | null;
    updatedCalculatedAttributes: CalculatedAttribute[];
    addedCalculatedAttributes: CalculatedAttribute[];
    addedPairs: { [key: string]: CriterionAndEquationSet[]; }
    updatedPairs: { [key: string]: CriterionAndEquationSet[]; }
    deletedPairs: { [key: string]: string[]; }
}

export interface calculcatedAttributePagingPageModel extends PagingPage<CriterionAndEquationSet>{
    calculationTiming: Timing;
}
