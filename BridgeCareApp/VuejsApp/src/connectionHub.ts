import { HubConnectionBuilder, LogLevel, HttpTransportType } from '@microsoft/signalr';
import { coreAxiosInstance } from './shared/utils/axios-instance';
import {SimulationAnalysisDetail} from '@/shared/models/iAM/simulation-analysis-detail';

export default {
    install(Vue: any) {
        const connection = new HubConnectionBuilder()
            .withUrl(`${process.env.VUE_APP_BRIDGECARE_CORE_URL}/bridgecarehub/`, {
                //skipNegotiation: false,
                transport: HttpTransportType.LongPolling
            })
            .configureLogging(LogLevel.Information)
            .build();

            const statusHub = new Vue();
            Vue.prototype.$statusHub = statusHub;

            connection.on('BroadcastAssignDataStatus', (status, percentage) => {
                statusHub.$emit('assignedData-status-event', {status, percentage});
            });

            connection.on('BroadcastSummaryReportGenerationStatus', (status, scenarioId) => {
                statusHub.$emit('summaryReportGeneration-status-event', {status, scenarioId});
            });

            connection.on('BroadcastDataMigration', (status) => {
                statusHub.$emit('DataMigration-status-event', {status});
            });

            connection.on('BroadcastScenarioStatusUpdate', (status, scenarioId) => {
                statusHub.$emit('ScenarioStatusUpdate-status-event', {status, scenarioId});
            });

        connection.on('BroadcastSimulationAnalysisDetail', (simulationAnalysisDetail: SimulationAnalysisDetail) => {
            statusHub.$emit('SimulationAnalysisDetail-status-event', {simulationAnalysisDetail});
        });

        let startedPromise = null;
        function start() {
            startedPromise = connection.start().catch((err: any) => {
                console.error('Failed to connect with hub', err);
                return new Promise((resolve: any, reject: any) =>
                    setTimeout(() => start().then(resolve).catch(reject), 5000));
            });
            return startedPromise;
        }
        connection.onclose(() => start());

        start();
    }
};