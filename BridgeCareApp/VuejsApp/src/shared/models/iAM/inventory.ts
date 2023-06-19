export interface KeyProperty{
    name: string;
    isNum: boolean;
    prettify: string;
}
export interface InventoryItem {
    keyProperties: any[];
}
export interface InventoryParam {
    keyProperties: Record<string, string>;
}
export interface MappedInventoryItem {
    keyProperties: any[];
}
