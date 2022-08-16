export interface PagingPage<T>{
    items: T[];
    totalItems: number;
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