import {AxiosPromise} from 'axios';
import {API, coreAxiosInstance} from '@/shared/utils/axios-instance';

export default class ReportsService {
    static generateReport(scenarioId: string): AxiosPromise {
        return coreAxiosInstance.request({
            method: 'POST',
            url: `${API.Report}/GetFile/BAMSSummaryReport`,
            headers: {'Content-Type': 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'},
            data: scenarioId,
            responseType: 'text'
        });
    }

    static downloadSimulationLog(networkId: string, scenarioId: string): AxiosPromise {
        return coreAxiosInstance.request({
            method: 'POST',
            url: `${API.SimulationLog}/GetSimulationLog/${networkId}/${scenarioId}`,
            headers: {'Content-Type': 'text'},
            responseType: 'arraybuffer'
        });
    }

    static downloadReport(reportIndexID: string): AxiosPromise {
        return coreAxiosInstance.get(               
            `${API.Report}/DownloadReport/${reportIndexID}`,
        );
    }
}
