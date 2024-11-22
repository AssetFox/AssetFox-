export interface KeyProperty{
    name: string;
    isNum: boolean;
    prettify: string;
}
export interface InventoryItem {
    keyProperties: any[];
}
export interface InventoryParam {
values(values: any): unknown;
    keyProperties: Record<string, string>;
}
export const emptyInventoryParam: InventoryParam = {
    keyProperties: {},
    values: function (values: any): unknown {
        throw new Error("Function not implemented.");
    }
};
export interface QueryResponse {
    attribute: string;
    values: string[];
}
export const emptyQueryResponse: QueryResponse = {
    attribute: '',
    values: [],
}

export interface MappedInventoryItem {
    keyProperties: any[];
}
