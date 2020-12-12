import { HubConnectionBuilder, LogLevel, HttpTransportType } from '@microsoft/signalr';
import { bridgecareCoreAxiosInstance } from './shared/utils/axios-instance';

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

            connection.on('BroadcastDataMigration', (status, legacySimulationId) => {
                statusHub.$emit('DataMigration-status-event', {status, legacySimulationId});
            });

            connection.on('BroadcastScanarioStatusUpdate', (status, scenarioId) => {
                statusHub.$emit('ScenarioStatusUpdate-status-event', {status, scenarioId});
            });

        let startedPromise = null;
        function start() {
            startedPromise = connection.start().catch(err => {
                console.error('Failed to connect with hub', err);
                return new Promise((resolve, reject) =>
                    setTimeout(() => start().then(resolve).catch(reject), 5000));
            });
            return startedPromise;
        }
        connection.onclose(() => start());

        start();
    }
};