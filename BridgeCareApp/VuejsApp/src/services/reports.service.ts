import {AxiosPromise} from 'axios';
import {API, coreAxiosInstance} from '@/shared/utils/axios-instance';
import { ReportDetails } from '@/shared/models/iAM/reports';

export default class ReportsService {
    static generateReport(scenarioId: string, reportType: string): AxiosPromise {
        return coreAxiosInstance.request({
            method: 'POST',
            url: `${API.Report}/GetFile/${reportType}`,
            headers: {'Content-Type': 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'},
            data: scenarioId,
            responseType: 'text'
        });
    }
    static generateReportWithCriteria(scenarioId: string, expression: string, reportType:string): AxiosPromise {
        return coreAxiosInstance.request({
            method: 'POST',
            url: `${API.Report}/GetFile/${reportType}`,
            headers: {'Content-Type': 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'},
            data: {'scenarioId':scenarioId, 'expression':expression},
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

    static downloadReport(scenarioId: string, reportName: string): AxiosPromise {
        return coreAxiosInstance.get(               
            `${API.Report}/DownloadReport/${scenarioId}/${reportName}`,
        );
    }

    static getReportGenerationStatus(reportDetails: ReportDetails[]): AxiosPromise {
        return coreAxiosInstance.post(`${API.Report}/GetReportGenerationStatus`, reportDetails);
    }
    
    static deleteReport(scenarioId: string, reportName: string): AxiosPromise {
        return coreAxiosInstance.get(               
            `${API.Report}/DeleteReport/${scenarioId}/${reportName}`,
        );
    }

    static deleteAllGeneratedReports(scenarioId: string): AxiosPromise {
        return coreAxiosInstance.get(               
            `${API.Report}/DeleteReport/${scenarioId}`,
        );
    }
}
