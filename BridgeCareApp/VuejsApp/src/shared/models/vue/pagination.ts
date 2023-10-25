export interface Pagination {
    descending: boolean;
    page: number;
    rowsPerPage: number;
    sort: any[];
    totalItems: number;
}

export const emptyPagination: Pagination = {
    descending: false,
    page: 1,
    rowsPerPage: 5,
    totalItems: 0,
    sort: []
};
