import {
    CalculatedAttribute,
    CalculatedAttributeLibrary,
} from '@/shared/models/iAM/calculated-attribute';
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

    static upsertCalculatedAttributeLibrary(
        data: CalculatedAttributeLibrary,
    ): AxiosPromise {
        return coreAxiosInstance.post(
            `${API.CalculatedAttributes}/UpsertLibrary`,
            data,
        );
    }

    static upsertScenarioCalculatedAttribute(
        data: CalculatedAttribute[],
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
