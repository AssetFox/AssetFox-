import {AxiosPromise} from 'axios';
import {TreatmentLibrary} from '@/shared/models/iAM/treatment';
import {API, coreAxiosInstance} from '@/shared/utils/axios-instance';

export default class TreatmentService {
    static getTreatmentLibraries(): AxiosPromise {
        return coreAxiosInstance.get(`${API.TreatmentController}/GetTreatmentLibraries`);
    }

    static upsertTreatmentLibrary(data: TreatmentLibrary, scenarioId: string): AxiosPromise {
        return coreAxiosInstance.post(`${API.TreatmentController}/UpsertTreatmentLibrary/${scenarioId}`, data);
    }

    static deleteTreatmentLibrary(libraryId: string): AxiosPromise {
        return coreAxiosInstance.delete(`${API.TreatmentController}/DeleteTreatmentLibrary/${libraryId}`);
    }
}
