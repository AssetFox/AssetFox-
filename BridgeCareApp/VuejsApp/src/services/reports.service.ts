import { AxiosPromise } from 'axios';
import { axiosInstance, coreAxiosInstance } from '@/shared/utils/axios-instance';
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

    static getSummaryReportMissingAttributes(selectedScenarioId: String) {
        return bridgecareCoreAxiosInstance
            .get(`/api/SummaryReport/GetSummaryReportMissingAttributes/${selectedScenarioId}`);
    }

    static downloadSummaryReport(selectedScenarioData: Scenario): AxiosPromise {
        return axiosInstance.post('/api/DownloadSummaryReport', selectedScenarioData, { responseType: 'blob' });
    }

    static downloadTempSummaryReport(scenarioId: string, networkId: string): AxiosPromise {
          return coreAxiosInstance.request({
              method: 'POST',
              url: `/api/SummaryReport/GenerateSummaryReport/${networkId}/${scenarioId}`,
              headers: {'Content-Type': 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'},
              responseType: 'arraybuffer'
          });
    }

    static getJobList(): AxiosPromise {
        return axiosInstance.get('/api/GetJobList', {});
    }
}
