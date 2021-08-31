import { CalculatedAttribute, CalculatedAttributeLibrary } from "@/shared/models/iAM/calculated-attribute";
import { API, coreAxiosInstance } from "@/shared/utils/axios-instance";
import { AxiosPromise } from "axios";

export default class CalculatedAttributeService {
    
    static getCalculatedAttributeLibraries(): AxiosPromise {
        return coreAxiosInstance.get(
            `${API.CalculatedAttribute}/GetCalculatedAttributeLibraries`,
        );
    }
    static getScenarioCalculatedAttribute(scenarioId: string): AxiosPromise {
        return coreAxiosInstance.get(
            `${API.CalculatedAttribute}/GetScenarioCalculatedAttribute/${scenarioId}`,
        );
    }

    static upsertCalculatedAttributeLibrary(
        data: CalculatedAttributeLibrary,
    ): AxiosPromise {
        return coreAxiosInstance.post(
            `${API.CalculatedAttribute}/UpsertCalculatedAttributeLibrary`,
            data,
        );
    }

    static upsertScenarioCalculatedAttribute(
        data: CalculatedAttribute[],
        scenarioId: string,
    ): AxiosPromise {
        return coreAxiosInstance.post(
            `${API.CalculatedAttribute}/UpsertScenarioCalculatedAttribute/${scenarioId}`,
            data,
        );
    }
    static deleteCalculatedAttributeLibrary(libraryId: string): AxiosPromise {
        return coreAxiosInstance.delete(
            `${API.CalculatedAttribute}/DeleteCalculatedAttributeLibrary/${libraryId}`,
        );
    }
}
