import {AxiosPromise} from 'axios';
import {axiosInstance, coreAxiosInstance} from '@/shared/utils/axios-instance';
import { InventoryItem } from '@/shared/models/iAM/inventory';

export default class InventoryService {
    static getPennDOTInventory(): AxiosPromise {
        return coreAxiosInstance.get('/api/Inventory/GetPennDOTInventory');
    }
    static getInventory(keyProperties: string[]) {
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

    static getStaticInventoryHTML(reportType: string, filterData: InventoryItem): AxiosPromise{
        return coreAxiosInstance.post(`/api/Report/GetHTML/${reportType}`, filterData);
    }
}
