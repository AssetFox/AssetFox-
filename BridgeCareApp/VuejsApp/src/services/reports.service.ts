import {AxiosPromise} from 'axios';
import {API, coreAxiosInstance} from '@/shared/utils/axios-instance';

export default class ReportsService {
    static downloadSummaryReport(networkId: string, scenarioId: string): AxiosPromise {
        return coreAxiosInstance.request({
            method: 'POST',
            url: `${API.SummaryReport}/GenerateSummaryReport/${networkId}/${scenarioId}`,
            headers: {'Content-Type': 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'},
            responseType: 'arraybuffer'
        });
    }

    static downloadSimulationLog(networkId: string, scenarioId: string): AxiosPromise {
        console.log("Downloading log . . . ");
        return coreAxiosInstance.request({
            method: 'POST',
            url: `${API.SimulationLog}/GetSimulationLog/${networkId}/${scenarioId}`,
            headers: {'Content-Type': 'text'},
            responseType: 'arraybuffer'
        });
    }
}
