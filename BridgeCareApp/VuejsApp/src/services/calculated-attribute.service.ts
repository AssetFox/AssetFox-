import {
    CalculatedAttribute,
    CalculatedAttributeLibrary,
} from '@/shared/models/iAM/calculated-attribute';
import { API, coreAxiosInstance } from '@/shared/utils/axios-instance';
import { AxiosPromise } from 'axios';

export default class CalculatedAttributeService {
    static getCalculatedAttributeLibraries(): AxiosPromise {
        return coreAxiosInstance.get(
            `${API.CalculatedAttribute}/CalculatedAttrbiuteLibraries`,
        );
    }
    static getScenarioCalculatedAttribute(scenarioId: string): AxiosPromise {
        return coreAxiosInstance.get(
            `${API.CalculatedAttribute}/ScenarioAttributes/${scenarioId}`,
        );
    }

    static upsertCalculatedAttributeLibrary(
        data: CalculatedAttributeLibrary,
    ): AxiosPromise {
        return coreAxiosInstance.post(
            `${API.CalculatedAttribute}/UpsertLibrary`,
            data,
        );
    }

    static upsertScenarioCalculatedAttribute(
        data: CalculatedAttribute[],
        scenarioId: string,
    ): AxiosPromise {
        return coreAxiosInstance.post(
            `${API.CalculatedAttribute}/UpsertScenarioAttribute/${scenarioId}`,
            data,
        );
    }
    static deleteCalculatedAttributeLibrary(libraryId: string): AxiosPromise {
        return coreAxiosInstance.delete(
            `${API.CalculatedAttribute}/DeleteLibrary/${libraryId}`,
        );
    }
}
