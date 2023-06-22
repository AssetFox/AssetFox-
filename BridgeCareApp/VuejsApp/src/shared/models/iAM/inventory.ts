export interface KeyProperty{
    name: string;
    isNum: boolean;
    prettify: string;
}
export interface InventoryItem {
    keyProperties: any[];
}
export interface inventoryParam {
    keyProperties: Record<string, string>;
}
export const emptyInventoryParam: inventoryParam = {
    keyProperties: {}
};
export interface MappedInventoryItem {
    keyProperties: any[];
}
