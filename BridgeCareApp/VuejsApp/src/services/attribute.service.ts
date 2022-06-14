import {AxiosPromise} from 'axios';
import {API, coreAxiosInstance} from '@/shared/utils/axios-instance';
import { Attribute } from '@/shared/models/iAM/attribute';

export default class AttributeService {
    static getAttributes(): AxiosPromise {
        return coreAxiosInstance.get(`${API.Attribute}/GetAttributes`);
    }

    static getAttributeSelectValues(attributeNames: string[]): AxiosPromise {
        return coreAxiosInstance.post(`${API.Attribute}/GetAttributesSelectValues`, attributeNames);
    }

    static upsertAttribute(data: Attribute){
        return coreAxiosInstance.post(
            `${API.BudgetPriority}/CreateAttribute`,
            data
        );
    }

    static upsertAttributes(data: Attribute[]){
        return coreAxiosInstance.post(
            `${API.BudgetPriority}/CreateAttributes`,
            data
        );
    }
}
