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

    /**
     * Gets an inventory item's detail by bms id
     * @param bmsId number
     */
    //static getInventoryItemDetailByBMSId(bmsId: string): AxiosPromise {
    //    return axiosInstance.get('/api/GetInventoryItemDetailByBmsId', {params: {'bmsId': bmsId}});
    // }

    /**
     * Gets an inventory item's detail by br key
     * @param brKey number
     */
    //static getInventoryItemDetailByBRKey(brKey: string): AxiosPromise {
    //    return axiosInstance.get('/api/GetInventoryItemDetailByBrKey', {params: {'brKey': brKey}});
    //}

    static getStaticInventoryHTML(reportType: string, filterData: InventoryItem): AxiosPromise{
        return coreAxiosInstance.post(`/api/Report/GetHTML/${reportType}`, filterData);
    }
}
