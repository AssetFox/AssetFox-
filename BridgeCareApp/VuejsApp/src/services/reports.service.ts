import { AxiosPromise } from 'axios';
import { axiosInstance, bridgecareCoreAxiosInstance } from '@/shared/utils/axios-instance';
import { Scenario } from '@/shared/models/iAM/scenario';

export default class ReportsService {
    /**
     * Gets a scenario's detailed report
     * @param selectedScenarioData Scenario data to use in generating the report
     */
    static getDetailedReport(selectedScenarioData: Scenario): AxiosPromise {
        return axiosInstance.post('/api/GetDetailedReport', selectedScenarioData, { responseType: 'blob' });
    }

    /**
     * Gets a scenario's summary report
     * @param selectedScenarioData Scenario data to use in generating the report
     */
    static getSummaryReport(selectedScenarioData: Scenario): AxiosPromise {
        return axiosInstance.post('/api/GenerateSummaryReport', selectedScenarioData, {});
    }

    static getSummaryReportMissingAttributes(selectedScenarioId: number, selectedNetworkId: number) {
        return axiosInstance
            .get(`/api/GetSummaryReportMissingAttributes?simulationId=${selectedScenarioId}&networkId=${selectedNetworkId}`);
    }

    static downloadSummaryReport(selectedScenarioData: Scenario): AxiosPromise {
        return axiosInstance.post('/api/DownloadSummaryReport', selectedScenarioData, { responseType: 'blob' });
    }

    static downloadTempSummaryReport(selectedScenarioData: Scenario): AxiosPromise {
          return bridgecareCoreAxiosInstance.request({
              method: 'POST',
              url: '/api/SummaryReport/GenerateSummaryReport/E3E02FD0-6251-45AE-A5A4-7BE3E51F3B40/E3E02FD0-6251-45AE-A5A4-7BE3E51F3B40',
              headers: {'Content-Type': 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'},
              responseType: 'arraybuffer'
          });
    }

    static getJobList(): AxiosPromise {
        return axiosInstance.get('/api/GetJobList', {});
    }
}
