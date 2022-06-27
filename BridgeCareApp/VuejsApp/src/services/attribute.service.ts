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
    static GetAttributeAggregationRuleTypes(): AxiosPromise {
        return coreAxiosInstance.get(`${API.Attribute}/GetAggregationRuleTypes`);
    }
    static GetAttributeDataSourceTypes(): AxiosPromise {
        return coreAxiosInstance.get(`${API.Attribute}/GetAttributeDataSourceTypes`);
    }
    static CheckSqlConnection(connectionString: string): AxiosPromise {
        return coreAxiosInstance.post(`${API.Attribute}/CheckSqlConnection/${connectionString}`);
    }
}
