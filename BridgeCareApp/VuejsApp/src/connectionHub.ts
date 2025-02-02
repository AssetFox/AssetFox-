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
import mitt from 'mitt';
import { WorkType } from './shared/models/iAM/scenario';

export default {
    install: (app: any, options: any) => {
        const connection = new HubConnectionBuilder()
            .withUrl(
                `${import.meta.env.VITE_APP_BRIDGECARE_CORE_URL}/bridgecarehub/`,
                {
                    transport: HttpTransportType.LongPolling,
                },
            )
            .withAutomaticReconnect()
            .configureLogging(LogLevel.Information)
            .build();

        const emitter = mitt()

        connection.on(
            Hub.BroadcastType.BroadcastAssignDataStatus,
            (networkRollupDetail: NetworkRollupDetail, percentage) => {
                emitter.emit(
                    Hub.BroadcastEventType.BroadcastAssignDataStatusEvent,
                    { networkRollupDetail, percentage },
                );
            },
        );

        connection.on(
            Hub.BroadcastType.BroadcastReportGenerationStatus,
            (simulationReportDetail: SimulationReportDetail) => {
                emitter.emit(
                    Hub.BroadcastEventType
                        .BroadcastReportGenerationStatusEvent,
                    { simulationReportDetail },
                );
            },
        );

        connection.on(Hub.BroadcastType.BroadcastDataMigration, status => {
            emitter.emit(
                Hub.BroadcastEventType.BroadcastDataMigrationEvent,
                { status },
            );
        });

        connection.on(
            Hub.BroadcastType.BroadcastScenarioStatusUpdate,
            (status, scenarioId) => {
                emitter.emit(
                    Hub.BroadcastEventType.BroadcastScenarioStatusUpdateEvent,
                    { status, scenarioId },
                );
            },
        );

        connection.on(
            Hub.BroadcastType.BroadcastSimulationAnalysisDetail,
            (simulationAnalysisDetail: SimulationAnalysisDetail) => {
                emitter.emit(
                    Hub.BroadcastEventType
                        .BroadcastSimulationAnalysisDetailEvent,
                    {simulationAnalysisDetail},
                );
            },
        );

        connection.on(Hub.BroadcastType.BroadcastWorkQueueUpdate, workId => {
            emitter.emit(Hub.BroadcastEventType.BroadcastWorkQueueUpdateEvent, {
                workId,
            });
        });

        connection.on(Hub.BroadcastType.BroadcastFastWorkQueueUpdate, workId => {
            emitter.emit(Hub.BroadcastEventType.BroadcastFastWorkQueueUpdateEvent, {
                workId,
            });
        });

        connection.on(Hub.BroadcastType.BroadcastWorkQueueStatusUpdate, (queueItem: queuedWorkStatusUpdate) => {
            emitter.emit(Hub.BroadcastEventType.BroadcastWorkQueueStatusUpdateEvent, {
                queueItem,
            });
        });   
        
        connection.on(Hub.BroadcastType.BroadcastFastWorkQueueStatusUpdate, (queueItem: queuedWorkStatusUpdate) => {
            emitter.emit(Hub.BroadcastEventType.BroadcastFastWorkQueueStatusUpdateEvent, {
                queueItem,
            });
        });

        connection.on(Hub.BroadcastType.BroadcastImportCompletion, (importComp: importCompletion) => {
            emitter.emit(Hub.BroadcastEventType.BroadcastImportCompletionEvent, {
                importComp,
            });
        });

        connection.on(Hub.BroadcastType.BroadcastSimulationDeletionCompletion, (workType: WorkType) => {
            emitter.emit(Hub.BroadcastEventType.BroadcastSimulationDeletionCompletionEvent, {
                workType
            });
        });

        connection.on(Hub.BroadcastType.BroadcastError, (error, stackTrace) => {
            emitter.emit(Hub.BroadcastEventType.BroadcastErrorEvent, {
                error, stackTrace
            });
        });

        connection.on(Hub.BroadcastType.BroadcastWarning, warning => {
            emitter.emit(Hub.BroadcastEventType.BroadcastWarningEvent, {
                warning,
            });
        });

        connection.on(Hub.BroadcastType.BroadcastInfo, info => {
            emitter.emit(Hub.BroadcastEventType.BroadcastInfoEvent, {
                info,
            });
        });
        
        connection.on(Hub.BroadcastType.BroadcastTaskCompleted, task => {
            emitter.emit(Hub.BroadcastEventType.BroadcastTaskCompletedEvent, {
                task,
            });
        });
        app.provide('emitter', emitter);
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
        BroadcastSimulationDeletionCompletion: 'BroadcastSimulationDeletionCompletion'     
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
        BroadcastSimulationDeletionCompletionEvent: 'BroadcastSimulationDeletionCompletionEvent'
    },
};
