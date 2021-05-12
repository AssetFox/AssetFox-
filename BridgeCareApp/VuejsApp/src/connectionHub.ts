import { HttpTransportType, HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import { SimulationAnalysisDetail } from '@/shared/models/iAM/simulation-analysis-detail';
import { SimulationReportDetail } from '@/shared/models/iAM/simulation-report-detail';
import { NetworkRollupDetail } from '@/shared/models/iAM/network-rollup-detail';

export default {
  install(Vue: any) {
    const connection = new HubConnectionBuilder()
      .withUrl(`${process.env.VUE_APP_BRIDGECARE_CORE_URL}/bridgecarehub/`, {
        //skipNegotiation: false,
        transport: HttpTransportType.LongPolling,
      })
      .configureLogging(LogLevel.Information)
      .build();

    const statusHub = new Vue();
    Vue.prototype.$statusHub = statusHub;

    connection.on(Hub.BroadcastType.BroadcastAssignDataStatus, (networkRollupDetail: NetworkRollupDetail, percentage) => {
      statusHub.$emit(Hub.BroadcastEventType.BroadcastAssignDataStatusEvent, { networkRollupDetail, percentage });
    });

    connection.on(Hub.BroadcastType.BroadcastSummaryReportGenerationStatus, (simulationReportDetail: SimulationReportDetail) => {
      statusHub.$emit(Hub.BroadcastEventType.BroadcastSummaryReportGenerationStatusEvent, { simulationReportDetail });
    });

    connection.on(Hub.BroadcastType.BroadcastDataMigration, (status) => {
      statusHub.$emit(Hub.BroadcastEventType.BroadcastDataMigrationEvent, { status });
    });

    connection.on(Hub.BroadcastType.BroadcastScenarioStatusUpdate, (status, scenarioId) => {
      statusHub.$emit(Hub.BroadcastEventType.BroadcastScenarioStatusUpdateEvent, { status, scenarioId });
    });

    connection.on(Hub.BroadcastType.BroadcastSimulationAnalysisDetail, (simulationAnalysisDetail: SimulationAnalysisDetail) => {
      statusHub.$emit(Hub.BroadcastEventType.BroadcastSimulationAnalysisDetailEvent, { simulationAnalysisDetail });
    });

    connection.on(Hub.BroadcastType.BroadcastError, (error) => {
      statusHub.$emit(Hub.BroadcastEventType.BroadcastErrorEvent, { error });
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
  },
};

export const Hub = {
    BroadcastType: {
        BroadcastError: 'BroadcastError',
        BroadcastAssignDataStatus: 'BroadcastAssignDataStatus',
        BroadcastSummaryReportGenerationStatus: 'BroadcastSummaryReportGenerationStatus',
        BroadcastScenarioStatusUpdate: 'BroadcastScenarioStatusUpdate',
        BroadcastSimulationAnalysisDetail: 'BroadcastSimulationAnalysisDetail',
        BroadcastDataMigration: 'BroadcastDataMigration',
        BroadcastNetworkRollupDetail: 'BroadcastNetworkRollupDetail'
    },
    BroadcastEventType: {
        BroadcastErrorEvent: 'BroadcastErrorEvent',
        BroadcastAssignDataStatusEvent: 'BroadcastAssignDataStatusEvent',
        BroadcastSummaryReportGenerationStatusEvent: 'BroadcastSummaryReportGenerationStatusEvent',
        BroadcastScenarioStatusUpdateEvent: 'BroadcastScenarioStatusUpdateEvent',
        BroadcastSimulationAnalysisDetailEvent: 'BroadcastSimulationAnalysisDetailEvent',
        BroadcastDataMigrationEvent: 'BroadcastDataMigrationEvent',
        BroadcastNetworkRollupDetailEvent: 'BroadcastNetworkRollupDetailEvent'
    }
};