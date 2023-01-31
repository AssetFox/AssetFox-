<template>
  <v-dialog max-width="500px"
            scrollable
            v-model="dialogData.showModal">
    <v-card>
      <v-card-title primary-title>
        <v-layout column>
          <v-flex>
            <v-layout justify-center>
              <h3 class="grey--text">Available Reports</h3>
            </v-layout>
          </v-flex>
          <v-progress-linear :indeterminate="true" v-if="isDownloading"></v-progress-linear>
          <v-flex>
            <span class="grey--text" v-if="isDownloading">Downloading... You can close this window, it will not stop the report generation</span>
          </v-flex>
        </v-layout>
      </v-card-title>
      <v-divider></v-divider>
      <v-card-text>
          <v-layout align-start row>
            <v-select style="padding:0px !important"
                :items='reports'
                v-model='selectedReport'>
            </v-select>
            <v-chip
                round
                color="ara-blue-bg"
                text-color="white"
                @click="onGenerateReport(true)"
                :disabled="selectedReport === ''"
            >
                Generate Summary Report
            </v-chip>
            <v-divider vertical></v-divider>
            <v-chip color='ara-blue-bg' text-color='white' @click="onDownloadSimulationLog(true)">
                Simulation Log
            </v-chip>
          </v-layout>
      </v-card-text>
      <v-divider></v-divider>
      <v-card-actions>
          <v-chip
                 @click="onDownloadReport()"
                 class="green darken-2 white--text">
            Download summary report
          </v-chip>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script lang="ts">
import Vue from 'vue';
import {Component, Prop} from 'vue-property-decorator';
import {Action, State} from 'vuex-class';
import {ReportsDownloaderDialogData} from '@/shared/models/modals/reports-downloader-dialog-data';
import ReportsService from '@/services/reports.service';
import {AxiosResponse} from 'axios';
import { FileInfo } from '@/shared/models/iAM/file-info';
import FileDownload from 'js-file-download';
import { convertBase64ToArrayBuffer } from '@/shared/utils/file-utils';
import {hasValue} from '@/shared/utils/has-value-util';
import { SelectItem } from '@/shared/models/vue/select-item';
import { getBlankGuid } from '@/shared/utils/uuid-utils';

@Component({})
export default class ReportsDownloaderDialog extends Vue {
    @Prop() dialogData: ReportsDownloaderDialogData;

    @State(state => state.busyModule.isBusy) isBusy: boolean;

    @Action('addSuccessNotification') addSuccessNotificationAction: any;
    @Action('addErrorNotification') addErrorNotificationAction: any;

    reports: SelectItem[] = [];   
    selectedReport: string = ''; 
    isDownloading: boolean = false;
    reportIndexID: string = getBlankGuid();

    mounted() {
        const reports: string[] =  this.$config.reportType;
        this.reports = reports.map(rep => {
            return {text: rep, value: rep}
        })

        if(reports.length > 0)
            this.selectedReport = reports[0];
    }

    async onGenerateReport(download: boolean) {
        if (download) {            
            this.isDownloading = true;
            this.dialogData.showModal = false;
            await ReportsService.generateReport(
                this.dialogData.scenarioId, this.selectedReport
            ).then((response: AxiosResponse<any>) => {
                this.isDownloading = false;
                if (response.status == 200) {
                    if (hasValue(response, 'data')) {
                        const resultId: string = response.data as string;
                        this.reportIndexID = resultId;
                    }

                    this.addSuccessNotificationAction({
                        message: 'Summary report generation started for ' + this.dialogData.name + '.',
                    });
                } else {
                    this.addErrorNotificationAction({
                        message: 'Failed to generate apricot for ' + this.dialogData.name + '.',
                        longMessage:
                            'Failed to generate the summary report. Make sure the scenario has been run',
                    });
                }
            });
        } else {
            this.dialogData.showModal = false;
        }
    }

    async onDownloadReport() {        
        this.isDownloading = true;
        this.dialogData.showModal = false;        
        await ReportsService.downloadReport(
            this.dialogData.scenarioId, this.selectedReport
        ).then((response: AxiosResponse<any>) => {
            this.isDownloading = false;
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

    async onDownloadSimulationLog(download: boolean) {
        if (download) {            
            this.isDownloading = true;
            this.dialogData.showModal = false;
            await ReportsService.downloadSimulationLog(
                this.dialogData.networkId,
                this.dialogData.scenarioId,
            ).then((response: AxiosResponse<any>) => {
                this.isDownloading = false;
                if (hasValue(response, 'data')) {
                    this.addSuccessNotificationAction({
                        message: 'Report downloaded',
                    });
                    FileDownload(
                        response.data,
                        `Simulation Log ${this.dialogData.name}.txt`,
                    );
                } else {
                    this.addErrorNotificationAction({
                        message: 'Failed to download simulation log.',
                        longMessage:
                            'Failed to download simulation log. Please try generating and downloading the log again.',
                    });
                }
            });
        } else {
            this.dialogData.showModal = false;
        }
    }
}
</script>

<style>
.missing-attributes-card-text {
    max-height: 300px;
    max-width: 300px;
    overflow-y: auto;
}
</style>
