import {
    HttpTransportType,
    HubConnectionBuilder,
    LogLevel,
} from '@microsoft/signalr';
import { SimulationAnalysisDetail } from '@/shared/models/iAM/simulation-analysis-detail';
import { SimulationReportDetail } from '@/shared/models/iAM/simulation-report-detail';
import { NetworkRollupDetail } from '@/shared/models/iAM/network-rollup-detail';
import AuthenticationModule from '@/store-modules/authentication.module';
import { hasValue } from '@/shared/utils/has-value-util';
import { UserInfo } from '@/shared/models/iAM/authentication';
import { parseLDAP } from '@/shared/utils/parse-ldap';
import has = Reflect.has;
import { getUserName } from '@/shared/utils/get-user-info';

export default {
    install(Vue: any) {
        const connection = new HubConnectionBuilder()
            .withUrl(
                `${process.env.VUE_APP_BRIDGECARE_CORE_URL}/bridgecarehub/`,
                {
                    //skipNegotiation: false,
                    transport: HttpTransportType.LongPolling,
                },
            )
            .configureLogging(LogLevel.Information)
            .build();

        const statusHub = new Vue();
        Vue.prototype.$statusHub = statusHub;

        connection.on(
            Hub.BroadcastType.BroadcastAssignDataStatus,
            (networkRollupDetail: NetworkRollupDetail, percentage) => {
                statusHub.$emit(
                    Hub.BroadcastEventType.BroadcastAssignDataStatusEvent,
                    { networkRollupDetail, percentage },
                );
            },
        );

        connection.on(
            Hub.BroadcastType.BroadcastSummaryReportGenerationStatus,
            (simulationReportDetail: SimulationReportDetail) => {
                statusHub.$emit(
                    Hub.BroadcastEventType
                        .BroadcastSummaryReportGenerationStatusEvent,
                    { simulationReportDetail },
                );
            },
        );

        connection.on(Hub.BroadcastType.BroadcastDataMigration, status => {
            statusHub.$emit(
                Hub.BroadcastEventType.BroadcastDataMigrationEvent,
                { status },
            );
        });

        connection.on(
            Hub.BroadcastType.BroadcastScenarioStatusUpdate,
            (status, scenarioId) => {
                statusHub.$emit(
                    Hub.BroadcastEventType.BroadcastScenarioStatusUpdateEvent,
                    { status, scenarioId },
                );
            },
        );

        connection.on(
            Hub.BroadcastType.BroadcastSimulationAnalysisDetail,
            (simulationAnalysisDetail: SimulationAnalysisDetail) => {
                statusHub.$emit(
                    Hub.BroadcastEventType
                        .BroadcastSimulationAnalysisDetailEvent,
                    { simulationAnalysisDetail },
                );
            },
        );

        connection.on(Hub.BroadcastType.BroadcastError, error => {
            statusHub.$emit(Hub.BroadcastEventType.BroadcastErrorEvent, {
                error,
            });
        });

        connection.on(Hub.BroadcastType.BroadcastWarning, warning => {
            statusHub.$emit(Hub.BroadcastEventType.BroadcastWarningEvent, {
                warning,
            });
        });

        let startedPromise: any | null = null;

        function start() {
            const username: string =
                hasValue(localStorage.getItem('UserInfo')) ||
                hasValue(localStorage.getItem('LoggedInUser'))
                    ? getUserName()
                    : '';

            startedPromise = connection
                .start()
                .then(_ => connection.invoke('AssociateMessage', username))
                .catch((err: any) => {
                    console.error('Failed to connect with hub', err);
                    return new Promise((resolve: any, reject: any) =>
                        setTimeout(
                            () =>
                                start()
                                    .then(resolve)
                                    .catch(reject),
                            5000,
                        ),
                    );
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
        BroadcastWarning: 'BroadcastWarning',
        BroadcastAssignDataStatus: 'BroadcastAssignDataStatus',
        BroadcastSummaryReportGenerationStatus:
            'BroadcastSummaryReportGenerationStatus',
        BroadcastScenarioStatusUpdate: 'BroadcastScenarioStatusUpdate',
        BroadcastSimulationAnalysisDetail: 'BroadcastSimulationAnalysisDetail',
        BroadcastDataMigration: 'BroadcastDataMigration',
        BroadcastNetworkRollupDetail: 'BroadcastNetworkRollupDetail',
    },
    BroadcastEventType: {
        BroadcastErrorEvent: 'BroadcastErrorEvent',
        BroadcastWarningEvent: 'BroadcastWarningEvent',
        BroadcastAssignDataStatusEvent: 'BroadcastAssignDataStatusEvent',
        BroadcastSummaryReportGenerationStatusEvent:
            'BroadcastSummaryReportGenerationStatusEvent',
        BroadcastScenarioStatusUpdateEvent:
            'BroadcastScenarioStatusUpdateEvent',
        BroadcastSimulationAnalysisDetailEvent:
            'BroadcastSimulationAnalysisDetailEvent',
        BroadcastDataMigrationEvent: 'BroadcastDataMigrationEvent',
        BroadcastNetworkRollupDetailEvent: 'BroadcastNetworkRollupDetailEvent',
    },
};
