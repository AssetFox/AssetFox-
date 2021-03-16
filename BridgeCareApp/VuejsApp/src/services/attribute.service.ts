import {AxiosPromise} from 'axios';
import {API, coreAxiosInstance} from '@/shared/utils/axios-instance';

export default class AttributeService {
    static getAttributes(): AxiosPromise {
        return coreAxiosInstance.get(`${API.Attribute}/GetAttributes`);
    }

    static getAttributeSelectValues(attributeNames: string[]): AxiosPromise {
        return coreAxiosInstance.post(`${API.Attribute}/GetAttributesSelectValues`, attributeNames);
    }
}
