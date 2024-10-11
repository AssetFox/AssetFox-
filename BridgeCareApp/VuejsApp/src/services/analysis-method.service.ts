import {AxiosPromise} from 'axios';
import {API, coreAxiosInstance} from '@/shared/utils/axios-instance';
import {AnalysisMethod} from '@/shared/models/iAM/analysis-method';

export default class AnalysisMethodService {
    static getAnalysisMethod(scenarioId: string): AxiosPromise {
        return coreAxiosInstance.get(`${API.AnalysisMethod}/GetAnalysisMethod/${scenarioId}`);
    }

    static getSimulationAnalysisSetting(scenarioId: string): AxiosPromise {
        return coreAxiosInstance.get(`${API.AnalysisMethod}/GetSimulationAnalysisSetting/${scenarioId}`);
    }

    static upsetAnalysisMethod(data: AnalysisMethod, scenarioId: string): AxiosPromise {
        return coreAxiosInstance.post(`${API.AnalysisMethod}/UpsertAnalysisMethod/${scenarioId}`, data);
    }
}
