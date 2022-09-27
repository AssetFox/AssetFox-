import {AxiosPromise} from 'axios';
import {API, coreAxiosInstance} from '@/shared/utils/axios-instance';
import {CriterionLibrary} from '@/shared/models/iAM/criteria';

export default class CriterionLibraryService {
    static getCriterionLibraries(): AxiosPromise {
        return coreAxiosInstance.get(`${API.CriterionLibrary}/GetCriterionLibraries`);
    }

    static upsertCriterionLibrary(data: CriterionLibrary): AxiosPromise {
        return coreAxiosInstance.post(`${API.CriterionLibrary}/UpsertCriterionLibrary`, data);
    }

    static deleteCriterionLibrary(libraryId: string): AxiosPromise {
        return coreAxiosInstance.delete(`${API.CriterionLibrary}/DeleteCriterionLibrary/${libraryId}`);
    }

    static getSelectedCriterionLibrary(libraryId: string): AxiosPromise{
        return coreAxiosInstance.get(`${API.CriterionLibrary}/GetSpecificCriteria/${libraryId}`);
    }

    static getHasPermittedAccess(): AxiosPromise {
        return coreAxiosInstance.get(
            `${API.CriterionLibrary}/GetHasPermittedAccess`,
        );
    }
}
