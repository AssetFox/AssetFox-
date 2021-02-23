import {AxiosPromise} from 'axios';
import {API, axiosInstance, coreAxiosInstance, nodejsAxiosInstance} from '@/shared/utils/axios-instance';
import {CriteriaValidation} from '@/shared/models/iAM/criteria-validation';
import {CriterionLibrary} from '@/shared/models/iAM/criteria';
import {convertFromVueToMongo} from '@/shared/utils/mongo-model-conversion-utils';

export default class CriteriaEditorService {
    /**
     * Checks a criteria's validity
     * @param mergedCriteriaExpression Criteria string to validate
     */
    static checkCriteriaValidity(criteriaValidation: CriteriaValidation): AxiosPromise {
        return axiosInstance.post('/api/ValidateCriteria', criteriaValidation);
    }

    static getCriterionLibraries(): AxiosPromise {
        return coreAxiosInstance.get(`${API.CriterionLibraryController}/GetCriterionLibraries`);
    }

    static addOrUpdateCriterionLibrary(data: CriterionLibrary): AxiosPromise {
        return coreAxiosInstance.post(`${API.CriterionLibraryController}/AddOrUpdateCriterionLibrary`, data);
    }

    static deleteCriterionLibrary(libraryId: string): AxiosPromise {
        return coreAxiosInstance.delete(`${API.CriterionLibraryController}/DeleteCriterionLibrary/${libraryId}`);
    }
}
