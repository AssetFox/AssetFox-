export interface PageingModel<T>{
    items: T[];
    totalItems: number;
    page: number;
    rowsPerPage: number;
}

export interface PagingRequestModel<T>{
    page: number;
    rowsPerPage: number;
    sortColumn: string;
    isDescending: boolean;
    rowsForDeletion: string[];
    updateRows: T[];
    addedRows: T[];
}