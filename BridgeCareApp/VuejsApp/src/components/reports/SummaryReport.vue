<template>
    <v-layout column class="Montserrat-font-family" justify-start>
        <v-layout align-center>
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
            </v-layout>
        <v-layout row>
            <v-btn class="ghd-white-bg ghd-blue ghd-button-text ghd-button" @click="onDownloadSimulationLog(true)" depressed>Simulation Log</v-btn>
        </v-layout>
        <GeneralCriterionEditorDialog
            :dialogData="criterionEditorDialogData"
            @submit="onCriterionEditorDialogSubmit"
        />
    </v-layout>
</template>

<script lang='ts'>
import Vue from 'vue';
import { Action, State } from 'vuex-class';
import Component from 'vue-class-component';
import { clone, contains } from 'ramda';
import GeneralCriterionEditorDialog from '@/shared/modals/GeneralCriterionEditorDialog.vue';
import { emptyGeneralCriterionEditorDialogData, GeneralCriterionEditorDialogData } from '@/shared/models/modals/general-criterion-editor-dialog-data';
import ReportsService from '@/services/reports.service';
import { FileInfo } from '@/shared/models/iAM/file-info';
import { convertBase64ToArrayBuffer } from '@/shared/utils/file-utils';
import {AxiosResponse} from 'axios';
import FileDownload from 'js-file-download';
import {hasValue} from '@/shared/utils/has-value-util';
import { getBlankGuid } from '@/shared/utils/uuid-utils';
import { SelectItem } from '@/shared/models/vue/select-item';

@Component({
    components: { GeneralCriterionEditorDialog },
})
export default class SummaryReports extends Vue {

    @State(state => state.adminDataModule.simulationReportNames) stateSimulationReportNames: string[];

    @Action('addErrorNotification') addErrorNotificationAction: any;
    @Action('addSuccessNotification') addSuccessNotificationAction: any;
    @Action('getSimulationReports') getSimulationReportsAction: any;

    initializedBudgets: boolean = false;
    mergedCriteriaExpression: string = "";

    simulationName: string;
    networkName: string = '';
    networkId: string = getBlankGuid();
    selectedScenarioId: string = getBlankGuid();

    reports: SelectItem[] = [];   
    selectedReport: string = ''; 

    criterionEditorDialogData: GeneralCriterionEditorDialogData = clone(
        emptyGeneralCriterionEditorDialogData,
    );

    mounted() {
        this.getSimulationReportsAction().then(() => {
            const reports: string[] = clone(this.stateSimulationReportNames)
            this.reports = reports.map(rep => {
                return {text: rep, value: rep}
            })

            if(reports.length > 0)
                this.selectedReport = reports[0];
        });
    }
    beforeRouteEnter(to: any, from: any, next: any) {
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
    onShowCriterionEditorDialog() {
        this.criterionEditorDialogData = {
            showDialog: true,
            CriteriaExpression: this.mergedCriteriaExpression,
        };
    }
    onCriterionEditorDialogSubmit(criterionexpression: string) {
        this.criterionEditorDialogData = clone(
            emptyGeneralCriterionEditorDialogData,
        );
        this.mergedCriteriaExpression = criterionexpression;
    }
    async onDownloadSimulationLog(download: boolean) {
        if (download) {            
            //this.isDownloading = true;
            //this.dialogData.showModal = false;
            await ReportsService.downloadSimulationLog(
                this.networkId,
                this.selectedScenarioId,
            ).then((response: AxiosResponse<any>) => {
                //this.isDownloading = false;
                if (hasValue(response, 'data')) {
                    this.addSuccessNotificationAction({
                        message: 'Report downloaded',
                    });
                    FileDownload(
                        response.data,
                        `Simulation Log ${this.simulationName}.txt`,
                    );
                } else {
                    this.addErrorNotificationAction({
                        message: 'Failed to download simulation log.',
                        longMessage:
                            'Failed to download simulation log. Please try generating and downloading the log again.',
                    });
                }
            });
        } 
    }
    async onGenerateReport(download: boolean) {
        if (download) {            
            //this.isDownloading = true;
            //this.dialogData.showModal = false;
            await ReportsService.generateReportWithCriteria(
                this.selectedScenarioId, this.mergedCriteriaExpression, this.selectedReport
            ).then((response: AxiosResponse<any>) => {
                //this.isDownloading = false;
                if (response.status == 200) {
                    if (hasValue(response, 'data')) {
                        const resultId: string = response.data as string;
                        this.reportIndexID = resultId;
                    }
                    this.addSuccessNotificationAction({
                        message: this.selectedReport +  ' report generation started for ' + this.simulationName + '.',
                    });
                } else {
                    this.addErrorNotificationAction({
                        message: 'Failed to generate apricot for ' + this.simulationName + '.',
                        longMessage:
                            'Failed to generate the summary report. Make sure the scenario has been run',
                    });
                }
            });
        }
    }

    async onDownloadReport() {        
        //this.isDownloading = true;
        //this.dialogData.showModal = false;        
        await ReportsService.downloadReport(
            this.selectedScenarioId, this.selectedReport
        ).then((response: AxiosResponse<any>) => {
            //this.isDownloading = false;
            if (hasValue(response, 'data')) {
                const fileInfo: FileInfo = response.data as FileInfo;
                FileDownload(convertBase64ToArrayBuffer(fileInfo.fileData), fileInfo.fileName, fileInfo.mimeType);
            } else {
                this.addErrorNotificationAction({
                    message: 'Failed to download report.',
                    longMessage:
                        'Failed to download the summary report. Make sure the scenario has been run',
                });
            }
        });
    }
}
</script>
<style>
</style>