<template>
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
                                    <v-btn v-if="props.item.name.includes('Summary')" class="ghd-blue" tooltip flat icon>
                                                <img class='img-general' :src="getUrl('assets/icons/eye-ghd-blue.svg')" @click="showEditDialog">
                                                <Dialog v-model:visible="editShow">
                                                    <v-card>
                                                <v-textarea
                                                    class="sm-txt Montserrat-font-family"
                                                    :value="props.item.mergedExpression"
                                                    full-width
                                                    no-resize
                                                    variant="outlined"
                                                    readonly
                                                    rows="5"
                                                />
                                            </v-card>
                                                </Dialog>

                                            </v-btn>
                                    <v-btn v-if="props.item.name.includes('Summary')"
                                        @click="onShowCriterionEditorDialog(props.item.id)"
                                        class="ghd-blue"
                                        flat
                                    >
                                        <img class='img-general' :src="getUrl('assets/icons/edit.svg')">
                                    </v-btn>
                                </td>
                                <td class="text-xs-left">
                                    <v-btn
                                        @click="onGenerateReport(props.item.id, true)"
                                        class="ghd-blue"
                                        flat
                                    >
                                        <img class="img-general" :src="getUrl('assets/icons/attributes-dark.svg')"/>

                                    </v-btn>
                                    <v-btn
                                        @click="onDownloadReport(props.item.id)"
                                        flat
                                    >
                                        <img class='img-general' :src="getUrl('assets/icons/download.svg')"/>
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
                <v-divider style="margin:0px;" />
                <v-row style="margin:10px;">
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
</template>

<script setup lang='ts'>
import { ref, onMounted, computed, watch } from 'vue';
import { clone, update, find, findIndex, propEq } from 'ramda';
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
import { Report, emptyReport } from '@/shared/models/iAM/reports';
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

    let store = useStore();
    const router = useRouter(); 
    const stateSimulationReportNames = computed<string[]>(() => store.state.adminDataModule.simulationReportNames);
    async function addErrorNotificationAction(payload?: any): Promise<any> { await store.dispatch('addErrorNotification',payload);} 
    async function addSuccessNotificationAction(payload?: any): Promise<any> { await store.dispatch('addSuccessNotification',payload);} 
    async function getSimulationReportsAction(payload?: any): Promise<any> { await store.dispatch('getSimulationReports',payload);} 

    let editShow = ref<boolean>(false);

    let simulationName: string;
    let networkName: string = '';
    let networkId: string = getBlankGuid();
    let selectedScenarioId: string = getBlankGuid();

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
            title: 'Actions',
            key: 'actions',
            align: 'left',
            sortable: false,
            class: '',
            width: '',
        },
    ];
    onMounted(() => {

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

        //selectedScenarioId = router.currentRoute.value.query.scenarioId as string;
        getSimulationReportsAction();
        
    });
    watch(stateSimulationReportNames, () => {
        stateSimulationReportNames.value.forEach(reportName => {
            currentPage.value.push({
                id: getNewGuid(),
                name: reportName,
                mergedExpression: ""
            });
        });
        if(stateSimulationReportNames.value.length > 0)
            selectedReport.value = currentPage.value[0];

    });
    const onRowSelect = (event:any) => {
        selectedReport.value = {
            id: event.data.id,
            name: event.data.name,
            mergedExpression: event.data.mergedExpression
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

</script>
<style>
</style>