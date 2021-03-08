import {AxiosPromise} from 'axios';
import {API, axiosInstance, coreAxiosInstance} from '@/shared/utils/axios-instance';
import {CriteriaValidation} from '@/shared/models/iAM/criteria-validation';
import {CriterionLibrary} from '@/shared/models/iAM/criteria';

export default class CriterionLibraryService {
    static checkCriteriaValidity(criteriaValidation: CriteriaValidation): AxiosPromise {
        return axiosInstance.post('/api/ValidateCriteria', criteriaValidation);
    }

    static getCriterionLibraries(): AxiosPromise {
        return coreAxiosInstance.get(`${API.CriterionLibrary}/GetCriterionLibraries`);
    }

    static upsertCriterionLibrary(data: CriterionLibrary): AxiosPromise {
        return coreAxiosInstance.post(`${API.CriterionLibrary}/UpsertCriterionLibrary`, data);
    }

    static deleteCriterionLibrary(libraryId: string): AxiosPromise {
        return coreAxiosInstance.delete(`${API.CriterionLibrary}/DeleteCriterionLibrary/${libraryId}`);
    }
}
