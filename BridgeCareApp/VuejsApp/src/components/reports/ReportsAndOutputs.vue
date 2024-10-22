<template>
    <v-card height="800px" class="elevation-0 vcard-main-layout">
    <v-row column class="Montserrat-font-family" justify-start>
        <div style="padding: 10px;">
            <v-subheader  class="ghd-md-gray ghd-control-label">
                Available Reports
            </v-subheader>
        </div>
        <v-col cols = "12">
            <v-row class="data-table" justify-left>
                <v-col>
                    <v-card class="elevation-0">
                        <v-data-table
                            id="ReportsAndOutputs-datatable"
                            :headers="reportsGridHeaders"
                            :items="currentPage"      
                            sort-asc-icon="custom:GhdTableSortAscSvg"
                            sort-desc-icon="custom:GhdTableSortDescSvg"                 
                            :rows-per-page-items=[5,10,25]
                            class="fixed-header ghd-table v-table__overflow"
                            item-key="id"
                        >
                            <template slot="items" slot-scope="props" v-slot:item="props">
                                <tr>
                                <td class="text-xs-left">
                                    <div>
                                        <span class='lg-txt'>{{props.item.name}}</span>
                                    </div>
                                </td>
                                <td class="text-xs-left">
                                <v-menu location="left" >
                                    <template v-slot:activator="{ props }">
                                        <v-btn  v-bind="props" class="ghd-blue" flat icon>
                                            <img class='img-general' :src="getUrl('assets/icons/eye-ghd-blue.svg')">
                                        </v-btn>
                                    </template>
                                    <v-card>
                                        <v-card-text>
                                            <v-textarea
                                                class="sm-txt Montserrat-font-family"
                                                :model-value="props.item.mergedExpression"
                                                full-width
                                                no-resize
                                                outline
                                                readonly
                                                rows="5"
                                                style = "min-width: 500px;min-height: 205px;"
                                            />
                                        </v-card-text>
                                    </v-card>
                                </v-menu>
                                    <v-btn v-if="props.item.name.includes('Summary')"
                                        @click="onShowCriterionEditorDialog(props.item.id)"
                                        class="criteria-button-blue"
                                        flat
                                        icon
                                    >
                                        <EditSvg />
                                    </v-btn>
                                </td>
                                <td>{{ props.item.reportStatus }}</td>
                                <td class="text-xs-left">
                                    <v-btn
                                    @click="onGenerateReport(props.item.id, true)"
                                    :disabled="props.item.isGenerated"
                                    class="ghd-green"
                                    flat
                                    icon
                                    >
                                    <img
                                    style="height: 25px"
                                        :src="getUrl('assets/icons/attributes-dark.svg')"
                                    />
                                    </v-btn>
                                    <v-btn
                                        @click="onDownloadReport(props.item.id)"
                                        :disabled="!props.item.isGenerated"
                                        class='ghd-blue-icon'
                                        flat
                                        icon
                                    >
                                        <img :src="getUrl('assets/icons/download.svg')"/>
                                    </v-btn>
                                    <v-btn
                                        v-if="hasAdminAccess"
                                        @click="onDeleteReport(props.item.id)"
                                        :disabled="!props.item.isGenerated || !CanUserDelete(props.item.name)"
                                        flat
                                        icon
                                        class="ghd-red"
                                    >
                                        <TrashCanSvg />
                                    </v-btn>
                                </td>
                            </tr>
                            </template>
                        </v-data-table>
                    </v-card>
                </v-col>
            </v-row>
        </v-col>
        <v-col>
        <v-row column>
            <v-col>
                <v-subheader class="ghd-md-gray ghd-control-label">
                    Diagnostics & Logging
                </v-subheader>
                <v-divider
            :thickness="2"
            class="border-opacity-100"
        ></v-divider>
                <v-row style="margin:5px;">
                    <v-btn class="ghd-white-bg ghd-blue ghd-button-text ghd-button" @click="onDownloadSimulationLog(true)" variant = "flat">Simulation Log</v-btn>
                </v-row>
            </v-col>

        </v-row>
        </v-col>
        <GeneralCriterionEditorDialog
            :dialogData="criterionEditorDialogData"
            @submit="onCriterionEditorDialogSubmit"
        />
    </v-row>
</v-card>
</template>

<script setup lang='ts'>
import { ref, onMounted, computed, watch, inject, onBeforeMount, onBeforeUnmount } from 'vue';
import { clone, update, find, findIndex, propEq, isNil } from 'ramda';
import GeneralCriterionEditorDialog from '@/shared/modals/GeneralCriterionEditorDialog.vue';
import { emptyGeneralCriterionEditorDialogData, GeneralCriterionEditorDialogData } from '@/shared/models/modals/general-criterion-editor-dialog-data';
import ReportsService from '@/services/reports.service';
import { FileInfo } from '@/shared/models/iAM/file-info';
import { convertBase64ToArrayBuffer } from '@/shared/utils/file-utils';
import {AxiosResponse} from 'axios';
import FileDownload from 'js-file-download';
import {hasValue} from '@/shared/utils/has-value-util';
import { getBlankGuid, getNewGuid } from '@/shared/utils/uuid-utils';
import { SelectItem } from '@/shared/models/vue/select-item';
import { Report, ReportDetails, emptyReport } from '@/shared/models/iAM/reports';
import {
    InputValidationRules,
    rules as validationRules,
} from '@/shared/utils/input-validation-rules';
import { useStore } from 'vuex';
import { useRouter } from 'vue-router'; 
import Dialog from 'primevue/dialog';
import DataTable from 'primevue/datatable';
import Column from 'primevue/column';
import { getUrl } from '@/shared/utils/get-url';
import mitt, { Emitter, EventType } from 'mitt';
import { Hub } from '@/connectionHub';
import { Notification } from '@/shared/models/iAM/notifications';
import { queuedWorkStatusUpdate } from '@/shared/models/iAM/queuedWorkStatusUpdate';
import TrashCanSvg from '@/shared/icons/TrashCanSvg.vue';
import EditSvg from '@/shared/icons/EditSvg.vue';
import ReportsTrashCanButton from '@/shared/components/buttons/ReportsTrashCanButton.vue';


    let store = useStore();
    const router = useRouter();
    const $emitter = inject('emitter') as Emitter<Record<EventType, unknown>> 
    const stateSimulationReportNames = computed<string[]>(() => store.state.adminDataModule.simulationReportNames);
    function addErrorNotificationAction(payload?: any) {  store.dispatch('addErrorNotification',payload);} 
    function addSuccessNotificationAction(payload?: any) { store.dispatch('addSuccessNotification',payload);} 
    async function getSimulationReportsAction(payload?: any): Promise<any> { await store.dispatch('getSimulationReports',payload);} 
    async function updateSimulationReportDetailAction(payload?: any): Promise<any>{await store.dispatch('updateSimulationReportDetail', payload)}
    const notifications = computed<Notification[]>(() => store.state.notificationModule.notifications);
    let hasAdminAccess = computed<boolean>(() => store.state.authenticationModule.hasAdminAccess);

    let editShow = ref<boolean>(false);

    let simulationName: string;
    let networkName: string = '';
    let networkId: string = getBlankGuid();
    let selectedScenarioId: string = getBlankGuid();
    const scenarioOutputName: string = "ScenarioOutput"

    let rules: InputValidationRules = clone(validationRules);

    const criterionEditorDialogData = ref<GeneralCriterionEditorDialogData>(clone(emptyGeneralCriterionEditorDialogData));
    const currentPage = ref<Report[]>([]);
    let selectedReport = ref<Report>(emptyReport); 
    const reportsGridHeaders: any[] = [
        {
            title: 'Name',
            key: 'name',
            align: 'left',
            sortable: true,
            class: '',
            width: '',
        },
        {
            title: 'Criteria',
            key: 'criterionLibrary',
            align: 'left',
            sortable: false,
            class: '',
            width: '',
        },
        {
            title: 'Report Status',
            key: 'status',
            align: 'left',
            sortable: false,
            class: '',
            width: '',
        },
        {
            title: 'Actions',
            key: 'actions',
            align: 'left',
            sortable: false,
            class: '',
            width: '',
        },
    ];

    onBeforeMount(async () => {
        $emitter.on(
            Hub.BroadcastEventType.BroadcastReportGenerationStatusEvent,
            getReportStatus,
        );
    });

    onBeforeUnmount(async () => {
        $emitter.off(
            Hub.BroadcastEventType.BroadcastReportGenerationStatusEvent,
            getReportStatus,
        );
    });
    

    onMounted(async () => {

        selectedScenarioId = router.currentRoute.value.query.scenarioId as string;
        simulationName = router.currentRoute.value.query.scenarioName as string;
        networkName = router.currentRoute.value.query.networkName as string;
        networkId = router.currentRoute.value.query.networkId as string;
        if (selectedScenarioId === getBlankGuid()) {
                // set 'no selected scenario' error message, then redirect user to Scenarios UI
                addErrorNotificationAction({
                    message: 'Found no selected scenario for edit',
                });
                router.push('/Scenarios/');
            }

        await getSimulationReportsAction();
        await getReportGenerationStatus();
    });

    watch(stateSimulationReportNames, () => {
        stateSimulationReportNames.value.forEach(reportName => {
            currentPage.value.push({
                id: getNewGuid(),
                name: reportName,
                mergedExpression: "",
                isGenerated: false
            });
        });
        if(stateSimulationReportNames.value.length > 0)
            selectedReport.value = currentPage.value[0];

    });

    watch(notifications, (newNotifications) => {
        if (newNotifications.length > 0) {
            const latestNotification = newNotifications[0];
            
            // Check if the message matches the successful report pattern
            const reportGenerationPattern = /Successfully generated (.+) report for scenario: (.+)/;
            const match = latestNotification.longMessage.match(reportGenerationPattern);
            
            if (match) {
            const reportName = match[1];
            const scenarioName = match[2];
            
            // Get the report
            const report = currentPage.value.find(
                r => r.name === reportName && simulationName === scenarioName
            );

            if (report) {
                // Set the report status to "generated"
                report.isGenerated = true;
            }
            }
        }
    }, { deep: true, immediate: true });


    const onRowSelect = (event:any) => {
        selectedReport.value = {
            id: event.data.id,
            name: event.data.name,
            mergedExpression: event.data.mergedExpression,
            isGenerated: event.data.isGenerated
        };
    };

    function showEditDialog(): void {
        editShow.value = !editShow.value;
    }

    function onShowCriterionEditorDialog(reportId: string) {
        selectedReport.value = find(
            propEq('id', reportId),
            currentPage.value,
        ) as Report;

        criterionEditorDialogData.value = {
            showDialog: true,
            CriteriaExpression: selectedReport.value.mergedExpression,
        };
    }

    function onCriterionEditorDialogSubmit(criterionexpression: string) {
        criterionEditorDialogData.value = clone(
            emptyGeneralCriterionEditorDialogData,
        );
        currentPage.value = update(
            findIndex(
                propEq('id', selectedReport.value.id), currentPage.value), { ...selectedReport.value, mergedExpression: criterionexpression}, currentPage.value,
            );
    }
    
    async function onDownloadSimulationLog(download: boolean) {
        if (download) {            
            await ReportsService.downloadSimulationLog(
                networkId,
                selectedScenarioId,
            ).then((response: AxiosResponse<any>) => {
                if (hasValue(response, 'data')) {
                    addSuccessNotificationAction({
                        message: 'Report downloaded',
                    });
                    FileDownload(
                        response.data,
                        `Simulation Log ${simulationName}.txt`,
                    );
                } else {
                    addErrorNotificationAction({
                        message: 'Failed to download simulation log.',
                        longMessage:
                            'Failed to download simulation log. Please try generating and downloading the log again.',
                    });
                }
            });
        } 
    }
    async function onGenerateReport(reportId: string, download: boolean) {
        if (download) {            
            // Get the selected report
            selectedReport.value = find(
                propEq('id', reportId),
                currentPage.value,
            ) as Report;
            // Generate report with selected one from table
            await ReportsService.generateReportWithCriteria(
                selectedScenarioId, selectedReport.value.mergedExpression, selectedReport.value.name
            ).then((response: AxiosResponse<any>) => {
                if (response.status == 200) {
                    if (hasValue(response, 'data')) {
                        const resultId: string = response.data as string;
                        //reportIndexID = resultId;
                    }
                    addSuccessNotificationAction({
                        message: selectedReport.value.name +  ' report generation started for ' + simulationName + '.',
                    });
                } else {
                    addErrorNotificationAction({
                        message: 'Failed to generate apricot for ' + simulationName + '.',
                        longMessage:
                            'Failed to generate the report or output. Make sure the scenario has been run',
                    });
                }
            });
        }
    }

    async function onDownloadReport(reportId: string) {        
        selectedReport.value = find(
            propEq('id', reportId),
            currentPage.value,
        ) as Report;
        await ReportsService.downloadReport(
            selectedScenarioId, selectedReport.value.name
        ).then((response: AxiosResponse<any>) => {
            if (hasValue(response, 'data')) {
                const fileInfo: FileInfo = response.data as FileInfo;
                FileDownload(convertBase64ToArrayBuffer(fileInfo.fileData), fileInfo.fileName, fileInfo.mimeType);
            } else {
                addErrorNotificationAction({
                    message: 'Failed to download report.',
                    longMessage:
                        'Failed to download the report or output. Make sure the scenario has been run',
                });
            }
        });
    }

    async function getReportGenerationStatus()
    {
        const reportDetails: ReportDetails[] = currentPage.value.map(report => {
        return {
            simulationId: selectedScenarioId,
            reportName: report.name,
            isGenerated: false,
            };
        });

        await ReportsService.getReportGenerationStatus(
            reportDetails
        ).then((response: AxiosResponse<any>) => {
            if (hasValue(response, 'data')) {
                response.data.forEach((updatedReport: ReportDetails) => {
                    // Find the corresponding report in currentPage.value and update it
                    const reportIndex = currentPage.value.findIndex(report => 
                        report.name === updatedReport.reportName);
                    
                    if (reportIndex !== -1) {
                        currentPage.value[reportIndex].isGenerated = updatedReport.isGenerated;
                        currentPage.value[reportIndex].reportStatus = updatedReport.reportStatus;
                    }
                });
            }
        });
    }

    async function onDeleteReport(reportId: string) {        
        selectedReport.value = find(
            propEq('id', reportId),
            currentPage.value,
        ) as Report;
        await ReportsService.deleteReport(
            selectedScenarioId, selectedReport.value.name
        ).then((response: AxiosResponse<any>) => {
            if (hasValue(response, 'data')) {
                addSuccessNotificationAction({
                        message: selectedReport.value.name +  ' report has been deleted for ' + simulationName + '.',
                    });
                    selectedReport.value.isGenerated = false;
                    selectedReport.value.reportStatus = "Report Deleted";
            } else {
                addErrorNotificationAction({
                    message: 'Failed to delete report.',
                    longMessage:
                        'Failed to download the report or output. Make sure the scenario has been run',
                });
            }
        });
    }

    function getReportStatus(data: any) 
    {
        if (data.simulationReportDetail.status) {
        selectedReport.value = find(
            propEq('name', data.simulationReportDetail.reportType),
            currentPage.value,
        ) as Report
            const reportIndex = currentPage.value.findIndex(report => report.name === data.simulationReportDetail.reportType);
            if(data.simulationReportDetail.status && reportIndex !== -1)
            {
                currentPage.value[reportIndex].reportStatus = data.simulationReportDetail.status;
            }
        }
    }

    function CanUserDelete(reportName: string) : boolean{
        if(reportName === "ScenarioOutput")
            return hasAdminAccess.value;
        else 
            return true
    }

</script>
<style>
.criteria-button-blue {
    --svg-color: #2A578D;
    color: var(--svg-color) !important;
}

.green-icon {
    filter: invert(61%) sepia(70%) saturate(486%) hue-rotate(79deg) brightness(82%) contrast(85%);
}

.green-icon.disabled {
    color: transparent !important;  /* Inherit color from parent */
    --svg-color: #999999;
    background-color: transparent !important; /* Maintain the background color */

}

</style>