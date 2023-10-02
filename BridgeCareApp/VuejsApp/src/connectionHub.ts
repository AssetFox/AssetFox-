import {
    HttpTransportType,
    HubConnectionBuilder,
    LogLevel,
} from '@microsoft/signalr';
import { SimulationAnalysisDetail } from '@/shared/models/iAM/simulation-analysis-detail';
import { SimulationReportDetail } from '@/shared/models/iAM/simulation-report-detail';
import { NetworkRollupDetail } from '@/shared/models/iAM/network-rollup-detail';
import { hasValue } from '@/shared/utils/has-value-util';

import has = Reflect.has;
import { getUserName } from '@/shared/utils/get-user-info';
import { queuedWorkStatusUpdate } from './shared/models/iAM/queuedWorkStatusUpdate';
import { importCompletion } from './shared/models/iAM/ImportCompletion';

export default {
    install(Vue: any) {
        const connection = new HubConnectionBuilder()
            .withUrl(
                `${import.meta.env.VUE_APP_BRIDGECARE_CORE_URL}/bridgecarehub/`,
                {
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
            Hub.BroadcastType.BroadcastReportGenerationStatus,
            (simulationReportDetail: SimulationReportDetail) => {
                statusHub.$emit(
                    Hub.BroadcastEventType
                        .BroadcastReportGenerationStatusEvent,
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
                    {simulationAnalysisDetail},
                );
            },
        );

        connection.on(Hub.BroadcastType.BroadcastWorkQueueUpdate, workId => {
            statusHub.$emit(Hub.BroadcastEventType.BroadcastWorkQueueUpdateEvent, {
                workId,
            });
        });

        connection.on(Hub.BroadcastType.BroadcastFastWorkQueueUpdate, workId => {
            statusHub.$emit(Hub.BroadcastEventType.BroadcastFastWorkQueueUpdateEvent, {
                workId,
            });
        });

        connection.on(Hub.BroadcastType.BroadcastWorkQueueStatusUpdate, (queueItem: queuedWorkStatusUpdate) => {
            statusHub.$emit(Hub.BroadcastEventType.BroadcastWorkQueueStatusUpdateEvent, {
                queueItem,
            });
        });   
        
        connection.on(Hub.BroadcastType.BroadcastFastWorkQueueStatusUpdate, (queueItem: queuedWorkStatusUpdate) => {
            statusHub.$emit(Hub.BroadcastEventType.BroadcastFastWorkQueueStatusUpdateEvent, {
                queueItem,
            });
        });

        connection.on(Hub.BroadcastType.BroadcastImportCompletion, (importComp: importCompletion) => {
            statusHub.$emit(Hub.BroadcastEventType.BroadcastImportCompletionEvent, {
                importComp,
            });
        });

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

        connection.on(Hub.BroadcastType.BroadcastInfo, info => {
            statusHub.$emit(Hub.BroadcastEventType.BroadcastInfoEvent, {
                info,
            });
        });
        
        connection.on(Hub.BroadcastType.BroadcastTaskCompleted, task => {
            statusHub.$emit(Hub.BroadcastEventType.BroadcastTaskCompletedEvent, {
                task,
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
        BroadcastInfo: 'BroadcastInfo',
        BroadcastWarning: 'BroadcastWarning',
        BroadcastTaskCompleted: 'BroadcastTaskCompleted',
        BroadcastAssignDataStatus: 'BroadcastAssignDataStatus',
        BroadcastReportGenerationStatus:
            'BroadcastReportGenerationStatus',
        BroadcastScenarioStatusUpdate: 'BroadcastScenarioStatusUpdate',
        BroadcastSimulationAnalysisDetail: 'BroadcastSimulationAnalysisDetail',
        BroadcastDataMigration: 'BroadcastDataMigration',
        BroadcastNetworkRollupDetail: 'BroadcastNetworkRollupDetail',
        BroadcastWorkQueueUpdate: 'BroadcastWorkQueueUpdate',
        BroadcastWorkQueueStatusUpdate: 'BroadcastWorkQueueStatusUpdate',
        BroadcastFastWorkQueueUpdate: 'BroadcastFastWorkQueueUpdate',
        BroadcastFastWorkQueueStatusUpdate: 'BroadcastFastWorkQueueStatusUpdate',
        BroadcastImportCompletion: 'BroadcastImportCompletion',        
    },
    BroadcastEventType: {
        BroadcastErrorEvent: 'BroadcastErrorEvent',
        BroadcastWarningEvent: 'BroadcastWarningEvent',
        BroadcastInfoEvent: 'BroadcastInfoEvent',
        BroadcastTaskCompletedEvent: 'BroadcastTaskCompleted',
        BroadcastAssignDataStatusEvent: 'BroadcastAssignDataStatusEvent',
        BroadcastReportGenerationStatusEvent:
            'BroadcastReportGenerationStatusEvent',
        BroadcastScenarioStatusUpdateEvent:
            'BroadcastScenarioStatusUpdateEvent',
        BroadcastSimulationAnalysisDetailEvent:
            'BroadcastSimulationAnalysisDetailEvent',
        BroadcastDataMigrationEvent: 'BroadcastDataMigrationEvent',
        BroadcastNetworkRollupDetailEvent: 'BroadcastNetworkRollupDetailEvent',
        BroadcastWorkQueueUpdateEvent: 'BroadcastWorkQueueUpdateEvent',
        BroadcastWorkQueueStatusUpdateEvent: 'BroadcastWorkQueueStatusUpdateEvent',
        BroadcastFastWorkQueueUpdateEvent: 'BroadcastFastWorkQueueUpdateEvent',
        BroadcastFastWorkQueueStatusUpdateEvent: 'BroadcastFastWorkQueueStatusUpdateEvent',        
        BroadcastImportCompletionEvent: 'BroadcastImportCompletionEvent',
    },
};
