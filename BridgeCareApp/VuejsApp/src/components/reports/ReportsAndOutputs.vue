<template>
    <v-layout column class="Montserrat-font-family" justify-start>
        <v-flex xs12>
            <v-layout class="data-table" justify-left>
                <v-flex xs12>
                    <v-subheader>
                        Available Reports
                    </v-subheader>
                    <v-card class="elevation-0">
                        <v-data-table
                            id="ReportsAndOutputs-datatable"
                            :headers="reportsGridHeaders"
                            :items="currentPage"                       
                            :rows-per-page-items=[5,10,25]
                            sort-icon=$vuetify.icons.ghd-table-sort

                            class="fixed-header ghd-table v-table__overflow"
                            item-key="id"
                        >
                            <template slot="items" slot-scope="props">
                                <td class="text-xs-left">
                                    <div>
                                        <span class='lg-txt'>{{props.item.name}}</span>
                                    </div>
                                </td>
                                <td class="text-xs-left">
                                    <v-menu
                                        min-height="500px"
                                        min-width="500px"
                                        right
                                    >
                                        <template slot="activator">
                                            <v-btn class="ghd-blue" tooltip flat icon>
                                                <img class='img-general' :src="require('@/assets/icons/eye-ghd-blue.svg')">
                                            </v-btn>
                                        </template>
                                        <v-card>
                                            <v-card-text>
                                                <v-textarea
                                                    class="sm-txt Montserrat-font-family"
                                                    :value=props.item.mergedExpression
                                                    full-width
                                                    no-resize
                                                    outline
                                                    readonly
                                                    rows="5"
                                                />
                                            </v-card-text>
                                        </v-card>
                                    </v-menu>
                                    <v-btn
                        @click="
                            onShowCriterionEditorDialog(props.item.id)
                        "
                                        class="ghd-blue"
                                        icon
                                    >
                                        <img class='img-general' :src="require('@/assets/icons/edit.svg')">
                                    </v-btn>
                                </td>
                                <td class="text-xs-left">
                                    <v-btn
                                        @click="onGenerateReport(props.item.id, true)"
                                        class="ghd-blue"
                                        icon
                                    >
                                        <img class="img-general" :src="require('@/assets/icons/attributes-dark.svg')"/>

                                    </v-btn>
                                    <v-btn
                                        @click="onDownloadReport(props.item.id)"
                                        icon
                                    >
                                        <img class='img-general' :src="require('@/assets/icons/download.svg')"/>
                                    </v-btn>
                                </td>
                            </template>
                            <template v-slot:body.append-inner>
                            </template>                               
                        </v-data-table>
                    </v-card>
                </v-flex>
            </v-layout>
        </v-flex>
        <v-flex>
        <v-layout column>
            <v-flex>
                <v-subheader class="ghd-md-gray ghd-control-label">
                    Diagnostics & Logging
                </v-subheader>
                <v-divider style="margin:0px;" />
                <v-layout style="margin:0px;">
                    <v-btn class="ghd-white-bg ghd-blue ghd-button-text ghd-button" @click="onDownloadSimulationLog(true)" depressed>Simulation Log</v-btn>
                </v-layout>
            </v-flex>

        </v-layout>
        </v-flex>
        <!-- <v-layout align-center>
            <v-flex xs4 class="ghd-constant-header">
                <v-subheader class="ghd-md-gray ghd-control-label">Select a Report</v-subheader>
                <v-select
                    :items='reports'
                    v-model='selectedReport'
                    append-icon=$vuetify.icons.ghd-down
                    outline
                    class="ghd-select ghd-text-field ghd-text-field-border">
                </v-select>
            </v-flex>
            <v-flex>
                <v-btn 
                    class="ghd-white-bg ghd-blue ghd-button-text ghd-button" 
                    depressed
                    @click="onGenerateReport(true)"
                    :disabled="selectedReport === ''"
                >Generate Report</v-btn>
                <v-btn 
                    class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button' 
                    @click="onDownloadReport()"
                    outline
                >Download Report</v-btn>
            </v-flex>
        </v-layout>
        <v-layout style="margin:10px; padding-left:0px" column>
            <v-layout justify-space-between align-center>
                <v-subheader class="ghd-control-label ghd-md-gray">                             
                    Report Criteria
                </v-subheader>
                <v-flex xs1 style="height=12px;padding-bottom:0px;padding-top:0px;">
                    <v-btn
                        id="SummaryReport-criteriaEditor-btn"
                        style="!important;"
                        @click="
                            onShowCriterionEditorDialog
                        "
                        class="edit-icon ghd-control-label"
                        icon
                    >
                        <img class='img-general' :src="require('@/assets/icons/edit.svg')"/>
                    </v-btn>
                </v-flex>
            </v-layout>
            <v-layout>
               <v-textarea
                   id="SummaryReport-criteria-textArea"
                   class="ghd-control-text ghd-control-border"
                   style="padding-bottom: 0px; padding-right:30px; height: 90px;"
                   no-resize
                   outline
                   readonly
                   :rows=4
                   v-model=mergedCriteriaExpression
               >
               </v-textarea>
            </v-layout>
        </v-layout> -->
        <GeneralCriterionEditorDialog
            :dialogData="criterionEditorDialogData"
            @submit="onCriterionEditorDialogSubmit"
        />
    </v-layout>
</template>

<script setup lang='ts'>
import Vue from 'vue';
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
import { DataTableHeader } from '@/shared/models/vue/data-table-header';
import { Report, emptyReport } from '@/shared/models/iAM/reports';
import {
    InputValidationRules,
    rules as validationRules,
} from '@/shared/utils/input-validation-rules';
import { useStore } from 'vuex';
    let store = useStore();
    let stateSimulationReportNames: string[] = (store.state.adminDataModule.simulationReportNames) ;

    async function addErrorNotificationAction(payload?: any): Promise<any> { await store.dispatch('addErrorNotification');} 
    async function addSuccessNotificationAction(payload?: any): Promise<any> { await store.dispatch('addSuccessNotification');} 
    async function getSimulationReportsAction(payload?: any): Promise<any> { await store.dispatch('getSimulationReports');} 

    let initializedBudgets: boolean = false;

    let simulationName: string;
    let networkName: string = '';
    let networkId: string = getBlankGuid();
    let selectedScenarioId: string = getBlankGuid();

    let rules: InputValidationRules = clone(validationRules);

    let criterionEditorDialogData: GeneralCriterionEditorDialogData = clone(
        emptyGeneralCriterionEditorDialogData,
    );
    let currentPage: Report[] = [];
    let reports: SelectItem[] = [];   
    let selectedReport: Report = emptyReport; 

    let reportsGridHeaders: DataTableHeader[] = [
        {
            text: 'Name',
            value: 'name',
            align: 'left',
            sortable: true,
            class: '',
            width: '',
        },
        {
            text: 'Criteria',
            value: 'criterionLibrary',
            align: 'left',
            sortable: false,
            class: '',
            width: '',
        },
        {
            text: 'Actions',
            value: '',
            align: 'left',
            sortable: false,
            class: '',
            width: '',
        },
    ];

    function mounted() {
        getSimulationReportsAction().then(() => {
            const stateReports: string[] = clone(stateSimulationReportNames)
            reports = stateReports.map(rep => {
                return {text: rep, value: rep}
            })

            stateSimulationReportNames.forEach(reportName => {
                currentPage.push({
                    id: getNewGuid(),
                    name: reportName,
                    mergedExpression: ""
                });
            });
            // const newReport: Report = emptyReport;
            // newReport.name = "Report 1";
            // newReport.id = getBlankGuid();
            // newReport.mergedExpression = "";
            // currentPage.push(newReport);

            if(stateReports.length > 0)
                selectedReport = currentPage[0];// reports[0];
        });
    }
    function beforeRouteEnter(to: any, from: any, next: any) {
        next((vm: any) => {
            vm.selectedScenarioId = to.query.scenarioId;
            vm.simulationName = to.query.scenarioName;
            vm.networkName = to.query.networkName;
            vm.networkId = to.query.networkId;
            if (vm.selectedScenarioId === getBlankGuid()) {
                // set 'no selected scenario' error message, then redirect user to Scenarios UI
                vm.addErrorNotificationAction({
                    message: 'Found no selected scenario for edit',
                });
                vm.$router.push('/Scenarios/');
            }
        });
    }
    function onShowCriterionEditorDialog(reportId: string) {

        selectedReport = find(
            propEq('id', reportId),
            currentPage,
        ) as Report;

        criterionEditorDialogData = {
            showDialog: true,
            CriteriaExpression: selectedReport.mergedExpression,
        };
    }
    function onCriterionEditorDialogSubmit(criterionexpression: string) {
        criterionEditorDialogData = clone(
            emptyGeneralCriterionEditorDialogData,
        );
        currentPage = update(
            findIndex(
                propEq('id', selectedReport.id), currentPage), { ...selectedReport, mergedExpression: criterionexpression}, currentPage,
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
            selectedReport = find(
                propEq('id', reportId),
                currentPage,
            ) as Report;
            // Generate report with selected one from table
            await ReportsService.generateReportWithCriteria(
                selectedScenarioId, selectedReport.mergedExpression, selectedReport.name
            ).then((response: AxiosResponse<any>) => {
                if (response.status == 200) {
                    if (hasValue(response, 'data')) {
                        const resultId: string = response.data as string;
                        reportIndexID = resultId;
                    }
                    addSuccessNotificationAction({
                        message: selectedReport.name +  ' report generation started for ' + simulationName + '.',
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
        selectedReport = find(
            propEq('id', reportId),
            currentPage,
        ) as Report;
        await ReportsService.downloadReport(
            selectedScenarioId, selectedReport.name
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