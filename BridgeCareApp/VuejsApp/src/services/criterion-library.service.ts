import {AxiosPromise} from 'axios';
import {API, axiosInstance, coreAxiosInstance} from '@/shared/utils/axios-instance';
import {CriteriaValidation} from '@/shared/models/iAM/criteria-validation';
import {CriterionLibrary} from '@/shared/models/iAM/criteria';

export default class CriterionLibraryService {
    static checkCriteriaValidity(criteriaValidation: CriteriaValidation): AxiosPromise {
        return axiosInstance.post('/api/ValidateCriteria', criteriaValidation);
    }

    static getCriterionLibraries(): AxiosPromise {
        return coreAxiosInstance.get(`${API.CriterionLibraryController}/GetCriterionLibraries`);
    }

    static upsertCriterionLibrary(data: CriterionLibrary): AxiosPromise {
        return coreAxiosInstance.post(`${API.CriterionLibraryController}/UpsertCriterionLibrary`, data);
    }

    static deleteCriterionLibrary(libraryId: string): AxiosPromise {
        return coreAxiosInstance.delete(`${API.CriterionLibraryController}/DeleteCriterionLibrary/${libraryId}`);
    }
}
