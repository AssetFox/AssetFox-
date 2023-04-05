import { CalculatedAttribute, CalculatedAttributeLibrary, CriterionAndEquationSet, Timing } from "./calculated-attribute";
import { Budget, BudgetAmount, BudgetLibrary, BudgetLibraryUser, Investment, InvestmentPlan } from "./investment";

//abstract
export abstract class BaseLibraryUpsertPagingRequest<T>{
    public library: T;
    public isNewLibrary: boolean;
    scenarioId: string | null;
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
    syncModel: PaginSync<Y>; 
}

export interface PagingRequest<T> extends BasePagingRequest{
    syncModel: PaginSync<T>;
}

export interface PagingPage<T>{
    items: T[];
    totalItems: number;
}

export interface PaginSync<T>{
    libraryId: string | null;
    isModified: boolean;
    rowsForDeletion: string[];
    updateRows: T[];
    addedRows: T[];
}

//Investment
export interface InvestmentLibraryUpsertPagingRequestModel extends BaseLibraryUpsertPagingRequest<BudgetLibrary>{
    syncModel: InvestmentPagingSyncModel;    
}

export interface InvestmentPagingPage extends PagingPage<Budget>{
    lastYear: number;
    firstYear: number;
    investmentPlan: InvestmentPlan
}

export interface InvestmentPagingRequestModel extends BasePagingRequest{
    syncModel: InvestmentPagingSyncModel;
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
    isModified: boolean;
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
    isModified: boolean;
    updatedCalculatedAttributes: CalculatedAttribute[];
    addedCalculatedAttributes: CalculatedAttribute[];
    addedPairs: { [key: string]: CriterionAndEquationSet[]; }
    updatedPairs: { [key: string]: CriterionAndEquationSet[]; }
    deletedPairs: { [key: string]: string[]; }
    defaultEquations: { [key: string]: CriterionAndEquationSet; }
}

export interface calculcatedAttributePagingPageModel extends PagingPage<CriterionAndEquationSet>{
    calculationTiming: Timing;
    defaultEquation: CriterionAndEquationSet;
    libraryId: string;
    isModified: boolean;
}
