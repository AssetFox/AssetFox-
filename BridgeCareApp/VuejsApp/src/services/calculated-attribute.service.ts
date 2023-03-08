import { CalculatedAttributeLibraryUpsertPagingRequestModel, CalculatedAttributePagingRequestModel, CalculatedAttributePagingSyncModel } from '@/shared/models/iAM/paging';
import { API, coreAxiosInstance } from '@/shared/utils/axios-instance';
import { AxiosPromise } from 'axios';

export default class CalculatedAttributeService {
    static getCalculatedAttributes(): AxiosPromise {
        return coreAxiosInstance.get(
            `${API.CalculatedAttributes}/CalculatedAttributes`,
        );
    }
    static getCalculatedAttributeLibraries(): AxiosPromise {
        return coreAxiosInstance.get(
            `${API.CalculatedAttributes}/CalculatedAttrbiuteLibraries`,
        );
    }
    static getScenarioCalculatedAttribute(scenarioId: string): AxiosPromise {
        return coreAxiosInstance.get(
            `${API.CalculatedAttributes}/ScenarioAttributes/${scenarioId}`,
        );
    }

    static getScenarioCalculatedAttrbiutetPage(scenarioId: string, data: CalculatedAttributePagingRequestModel): AxiosPromise{
        return coreAxiosInstance.post(
            `${API.CalculatedAttributes}/GetScenarioCalculatedAttrbiutetPage/${scenarioId}`,
            data,
        );
    }

    static getLibraryCalculatedAttributePage(libraryId: string, data: CalculatedAttributePagingRequestModel): AxiosPromise{
        return coreAxiosInstance.post(
            `${API.CalculatedAttributes}/GetLibraryCalculatedAttrbiutePage/${libraryId}`,
            data,
        );
    }

    static getEmptyCalculatedAttributesByLibraryId(libraryId: string): AxiosPromise{
        return coreAxiosInstance.get(`${API.CalculatedAttributes}/GetEmptyCalculatedAttributesByLibraryId/${libraryId}`)
    }

    static getEmptyCalculatedAttributesByScenarioId(scenarioId: string): AxiosPromise{
        return coreAxiosInstance.get(`${API.CalculatedAttributes}/GetEmptyCalculatedAttributesByScenarioId/${scenarioId}`)
    }

    static upsertCalculatedAttributeLibrary(
        data: CalculatedAttributeLibraryUpsertPagingRequestModel,
    ): AxiosPromise {
        return coreAxiosInstance.post(
            `${API.CalculatedAttributes}/UpsertLibrary`,
            data,
        );
    }

    static upsertScenarioCalculatedAttribute(
        data: CalculatedAttributePagingSyncModel,
        scenarioId: string,
    ): AxiosPromise {
        return coreAxiosInstance.post(
            `${API.CalculatedAttributes}/UpsertScenarioAttributes/${scenarioId}`,
            data,
        );
    }
    static deleteCalculatedAttributeLibrary(libraryId: string): AxiosPromise {
        return coreAxiosInstance.delete(
            `${API.CalculatedAttributes}/DeleteLibrary/${libraryId}`,
        );
    }
}
