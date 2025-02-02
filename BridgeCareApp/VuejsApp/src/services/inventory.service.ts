import {AxiosPromise} from 'axios';
import {axiosInstance, coreAxiosInstance} from '@/shared/utils/axios-instance';
import { InventoryParam, InventoryItem } from '@/shared/models/iAM/inventory';

export default class InventoryService {
    static getInventory(keyProperties: any[]) {        
        return coreAxiosInstance.post(`/api/Inventory/GetInventory`, keyProperties);
    }
    static getKeyProperties(): AxiosPromise {
        return coreAxiosInstance.get('/api/Inventory/GetKeyProperties');
    }

    static getValuesForPrimaryKey(propertyName: string): AxiosPromise {
        return coreAxiosInstance.get(`/api/Inventory/GetValuesForPrimaryKey/${propertyName}`);
    }

    static getValuesForRawKey(propertyName: string): AxiosPromise {
        return coreAxiosInstance.get(`/api/Inventory/GetValuesForRawKey/${propertyName}`);
    }    

    static getStaticInventoryHTML(reportType: string, filterData: InventoryParam): AxiosPromise{
        return coreAxiosInstance.post(`/api/Report/GetHTML/${reportType}`, filterData);
    }

    static getQuery(querySet: InventoryParam[]): AxiosPromise{
        return coreAxiosInstance.post(`/api/Inventory/GetQuery`, querySet);
    }
}
