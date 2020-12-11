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
              url: '/api/SummaryReport/GenerateSummaryReport/EAE31810-2687-4951-8786-51AC3D02F378/E707CEF3-41E5-4DA3-845D-CBEDCDBA0D6A',
              headers: {'Content-Type': 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'},
              responseType: 'arraybuffer'
          });
    }

    static getJobList(): AxiosPromise {
        return axiosInstance.get('/api/GetJobList', {});
    }
}
