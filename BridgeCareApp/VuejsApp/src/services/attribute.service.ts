import {AxiosPromise} from 'axios';
import {API, coreAxiosInstance} from '@/shared/utils/axios-instance';
import { Attribute } from '@/shared/models/iAM/attribute';
import { TestStringData } from '@/shared/models/iAM/test-string';

export default class AttributeService {
    static getAttributes(): AxiosPromise {
        return coreAxiosInstance.get(`${API.Attribute}/GetAttributes`);
    }

    static getAttributeSelectValues(attributeNames: string[]): AxiosPromise {
        return coreAxiosInstance.post(`${API.Attribute}/GetAttributesSelectValues`, attributeNames);
    }

    static upsertAttribute(data: { attribute: Attribute, setForAllAttributes: boolean }) {
        return coreAxiosInstance.post(
            `${API.Attribute}/CreateAttribute`,
            data 
        );
    }

    static upsertAttributes(data: Attribute[]){
        return coreAxiosInstance.post(
            `${API.Attribute}/CreateAttributes`,
            data
        );
    }

    static GetAttributeAggregationRules(): AxiosPromise {
        return coreAxiosInstance.get(`${API.Attribute}/GetAggregationRules`);
    }

    static GetAttributeDataSourceTypes(): AxiosPromise {
        return coreAxiosInstance.get(`${API.Attribute}/GetAttributeDataSourceTypes`);
    }
    static CheckCommand(sqlCommand: TestStringData): AxiosPromise {
        return coreAxiosInstance.post(`${API.Attribute}/CheckCommand/`, sqlCommand);
    }
}
