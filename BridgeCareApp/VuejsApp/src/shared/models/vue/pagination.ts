export interface Pagination {
    descending: boolean;
    page: number;
    rowsPerPage: number;
    totalItems: number;
    sort: any[];
}

export const emptyPagination: Pagination = {
    descending: false,
    page: 1,
    rowsPerPage: 5,
    totalItems: 0,
    sort: []
};
