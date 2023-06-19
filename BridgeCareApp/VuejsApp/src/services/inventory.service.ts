import {AxiosPromise} from 'axios';
import {axiosInstance, coreAxiosInstance} from '@/shared/utils/axios-instance';
import { InventoryParam, InventoryItem } from '@/shared/models/iAM/inventory';

export default class InventoryService {
    static getInventory(keyProperties: any[]) {
        return coreAxiosInstance.get('/api/Inventory/GetInventory',
        {   
            params: {                
                keyProperties: JSON.stringify(keyProperties),
            }
        });
    }
    static getKeyProperties(): AxiosPromise {
        return coreAxiosInstance.get('/api/Inventory/GetKeyProperties');
    }

    static getValuesForKey(propertyName: string): AxiosPromise {
        return coreAxiosInstance.get(`/api/Inventory/GetValuesForKey/${propertyName}`);
    }    

    static getStaticInventoryHTML(reportType: string, filterData: InventoryParam): AxiosPromise{
        return coreAxiosInstance.post(`/api/Report/GetHTML/${reportType}`, filterData);
    }
}
